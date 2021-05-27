using System;
using System.Collections.Generic;
using System.Linq;

namespace RemoveAny
{
    public static class IntegrationHelpers
    {
        public static int RemoveAny<T, S>(this IList<T> list, S sample, Func<T, S, bool> predicate)
        {
            if (list == null)
            {
                return 0;
            }

            var delPos = list
                .Select((x, i) => new { x, i })
                .Where(p => predicate(p.x, sample))
                .Select(p => p.i)
                .ToList();

            switch (delPos.Count)
            {
                case 0: break;
                case 1:
                    list.RemoveAt(delPos[0]);
                    break;
                default:
                    var deleted = 0;
                    for (int i = 0; i < delPos.Count; i++)
                    {
                        var p = delPos[i] - deleted++;
                        list.RemoveAt(p);
                    } // for rez
                    break;
            }
            return delPos.Count;
        }

        public static bool IsEqOrd(this string str1, string str2) =>
            string.Equals(str1, str2, StringComparison.Ordinal);

        public static int RemoveAny<T>(this IList<T> list, T sample)
            => RemoveAny(list, sample, Eq);

        static bool Eq<T>(T x, T y)
            => x == default && y == default || x.Equals(y);

        public static int RemoveAny(this IList<string> list, string sample)
            => RemoveAny(list, sample, IsEqOrd);

        public static int RemoveAny(this IList<int> list, int sample)
            => RemoveAny(list, sample, IsEq);

        public static bool IsEq(this int val1, int val2) =>
            int.Equals(val1, val2);

    }
}
