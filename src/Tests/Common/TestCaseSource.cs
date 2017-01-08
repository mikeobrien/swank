using System;
using System.Collections.Generic;
using System.Linq;
using Swank.Extensions;

namespace Tests.Common
{
    public class TestCaseSource
    {
        public static object[][] Create(Action<CaseDsl> config)
        {
            return Create(0, config);
        }

        public static object[][] Create(int parameters, Action<CaseDsl> config)
        {
            var cases = new List<object[]>();
            config(new CaseDsl(cases, parameters));
            return cases.ToArray();
        }

        public class CaseDsl
        {
            private readonly List<object[]> _cases;
            private readonly int _parameters;

            public CaseDsl(List<object[]> cases, int parameters)
            {
                _cases = cases;
                _parameters = parameters;
            }

            public CaseDsl Add<T>(params object[] parameters)
            {
                return Add(typeof(T).AsList().Concat(parameters).ToArray());
            }

            public CaseDsl Add(params object[] parameters)
            {
                if (_parameters > 0)
                    Array.Resize(ref parameters, _parameters);
                _cases.Add(parameters);
                return this;
            }
        }
    }
}
