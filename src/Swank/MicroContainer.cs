using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Swank.Extensions;

namespace Swank
{
    public class RegistrationDsl
    {
        private readonly List<Func<Type, object>> _factories;
        private readonly Dictionary<Type, object> _registrations;

        public RegistrationDsl(List<Func<Type, object>> factories,
            Dictionary<Type, object> registrations)
        {
            _factories = factories;
            _registrations = registrations;
        }

        public RegistrationDsl RegisterFactory(Func<Type, object> factory)
        {
            _factories.Add(factory);
            return this;
        }

        public RegistrationDsl Register(object instance)
        {
            _registrations.Add(instance.GetType(), instance);
            return this;
        }

        public RegistrationDsl Register<TPlugin>(object instance)
        {
            _registrations.Add(typeof(TPlugin), instance);
            return this;
        }

        public RegistrationDsl Register<TPlugin>(Type type)
        {
            _registrations.Add(typeof(TPlugin), type);
            return this;
        }

        public RegistrationDsl Register<TPlugin, TConcrete>()
        {
            _registrations.Add(typeof(TPlugin), typeof(TConcrete));
            return this;
        }
    }

    public class MicroContainer
    {
        private readonly Dictionary<Type, object> _registrations = 
            new Dictionary<Type, object>();
        private readonly List<Func<Type, object>> _factories = 
            new List<Func<Type, object>>();
        private readonly List<object> _cache = new List<object>();

        private MicroContainer(Action<RegistrationDsl> configure)
        {
            configure?.Invoke(new RegistrationDsl(_factories, _registrations));
        }

        public static MicroContainer Create(Action<RegistrationDsl> configure = null)
        {
            return new MicroContainer(configure);
        }

        public T GetInstance<T>(params object[] instances)
        {
            return (T)GetInstance(typeof(T), instances, new List<Type>());
        }

        public T CreateInstance<T>(params object[] instances)
        {
            return (T)GetInstance(typeof(T), instances, new List<Type>(), false);
        }

        private object GetInstance(Type type, object[] instances, List<Type> ancestors, bool cached = true)
        {
            var instance = instances.FirstOrDefault(x => x.GetType() == type) ??
                (cached ? _cache.FirstOrDefault(x => x.GetType() == type) : null) ??
                _registrations.FirstOrDefault(x => x.Key == type && !(x.Value is Type)).Value;
            if (instance != null) return instance;

            var constructor = type
                .GetConstructors(BindingFlags.Instance | BindingFlags.Public)
                .Select(x => new
                {
                    Constructor = x,
                    Parameters = x.GetParameters()
                })
                .OrderByDescending(x => x.Parameters.Length)
                .FirstOrDefault();

            if (constructor == null) throw new InvalidOperationException(
                $"{type.FullName} is not registered." + (ancestors.Any() ? "\r\n" +
                    ancestors.Concat(type).Select(x => x.FullName).Join(" -->\r\n") : ""));

            var newInstance = constructor.Constructor.Invoke(constructor.Parameters
                .Select(x =>
                {
                    var typeRegistration = _registrations
                        .FirstOrDefault(y => y.Value is Type && 
                            y.Key == x.ParameterType).Value;
                    var parameterType = (Type)typeRegistration ?? x.ParameterType;
                    return _factories.Select(y => y(parameterType)).FirstOrDefault() ?? 
                        GetInstance(parameterType, instances, new List<Type>(ancestors) { type });
                }).ToArray());

            _cache.Add(newInstance);

            return newInstance;
        }
    }
}
