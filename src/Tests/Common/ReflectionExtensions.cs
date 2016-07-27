using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Swank.Extensions;

namespace Tests.Common
{
    public static class ReflectionExtensions
    {
        public static bool InNamespace<T>(this Type type)
        {
            return type.Namespace == typeof(T).Namespace ||
                type.Namespace.StartsWith(typeof(T).Namespace + ".");
        }

        public static T SetProperty<T>(this T instance,
            Expression<Func<T, object>> property, object value)
        {
            return instance.SetProperty(property.GetPropertyName(), value);
        }

        public static T SetProperty<T>(this T instance, string name, object value)
        {
            typeof(T).GetProperty(name).SetValue(instance, value, null);
            return instance;
        }

        public static string GetPropertyName<T, TResult>(
            this Expression<Func<T, TResult>> expression)
        {
            var propertyInfo = expression.GetPropertyInfo();
            return propertyInfo?.Name;
        }

        public static PropertyInfo GetPropertyInfo<T, TResult>(
            this Expression<Func<T, TResult>> expression)
        {
            var memberExpression = expression.GetMemberExpression();
            var propertyInfo = memberExpression.Member as PropertyInfo;
            return propertyInfo;
        }

        public static MemberExpression GetMemberExpression<T, TResult>(
            this Expression<Func<T, TResult>> expression)
        {
            MemberExpression memberExpression = null;
            if (expression.Body.NodeType == ExpressionType.Convert)
            {
                var body = (UnaryExpression)expression.Body;
                memberExpression = body.Operand as MemberExpression;
            }
            else if (expression.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = expression.Body as MemberExpression;
            }
            if (memberExpression == null)
            {
                throw new ArgumentException("Not a member access", nameof(expression));
            }
            return memberExpression;
        }

        public static MethodInfo GetMethodInfo<T, TResult>(
            this Expression<Func<T, TResult>> expression)
        {
            MethodCallExpression callExpression = null;
            if (expression.Body.NodeType == ExpressionType.Call)
                callExpression = expression.Body as MethodCallExpression;
            if (callExpression == null)
                throw new ArgumentException("Not a method.", nameof(expression));
            return callExpression.Method;
        }

        public static bool HasAttribute<T>(this MethodInfo method)
        {
            return method.GetCustomAttributes(typeof(T), true).Any();
        }
    }
}
