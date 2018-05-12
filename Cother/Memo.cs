using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cother
{
    public static class Memo
    {
        private static Dictionary<Object, Dictionary<Object, Object>> cache = new Dictionary<object, Dictionary<object, object>>();
        public static TReturnValue Memoize<TArgument, TReturnValue>(TArgument argument, Func<TArgument, TReturnValue> pureFunction) {
            if (!cache.ContainsKey(pureFunction))
            {
                cache.Add(pureFunction, new Dictionary<object, object>());
            }
            var memo = cache[pureFunction];
            if (!memo.ContainsKey(argument))
            {
                memo[argument] = pureFunction(argument);
            }
            return (TReturnValue)memo[argument];
        }
    }
}
