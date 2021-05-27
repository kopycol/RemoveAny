using RemoveAny;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RemoveAnyTests
{
    public class RemoveAnyTests
    {
        [Theory]
        [MemberData(nameof(RemoveAnyStringData))]
        [MemberData(nameof(RemoveAnyIntData))]
        [MemberData(nameof(RemoveAnyDtData))]
        [MemberData(nameof(RemoveAnyClAData))]
        private void RemoveAnyTest<T, S>(
            IList<T> list,
            S sample,
            int deltaCount,
            Func<T, S, bool> predicate = null)
        {
            var coount0 = list.Count;
            if (predicate == null && typeof(T) == typeof(S))
            {
                predicate = predicate ?? new Func<T, S, bool>((x, y) => x == default && y == default || x.Equals(y));
            }
            var removed = list.RemoveAny(sample, predicate);
            if (deltaCount >= 0)
            {
                Assert.Equal(list.Count, coount0 - deltaCount);
                Assert.Equal(removed, deltaCount);
            }
            var found = list.FirstOrDefault(i => predicate(i, sample));
            Assert.Equal(default, found);
        }

        static List<string> listStr1 => new List<string>() { "aa", "bb", "cc", "cc", "bb", "aa" };

        public static TheoryData<IList<string>, string, int, Func<string, string, bool>> RemoveAnyStringData() =>
            new TheoryData<IList<string>, string, int, Func<string, string, bool>>
            {
                { listStr1, "bb", 2, IntegrationHelpers.IsEqOrd},
                { listStr1, "bb", 2, IntegrationHelpers.IsEqOrd},
                { listStr1, "cc", 2, IntegrationHelpers.IsEqOrd},
                { listStr1, "aa", 2, IntegrationHelpers.IsEqOrd },
                { listStr1, "xx", 0, IntegrationHelpers.IsEqOrd },
                { new List<string>(), "bb", 0, IntegrationHelpers.IsEqOrd },
                { listStr1, (string)null, 0, IntegrationHelpers.IsEqOrd }, // FAIL 
            };

        static List<int> listInt1 => new List<int>() { 11, 22, 33, 33, 22, 22, 11 };

        public static TheoryData<IList<int>, int, int, Func<int, int, bool>> RemoveAnyIntData() =>
            new TheoryData<IList<int>, int, int, Func<int, int, bool>>
            {
                { listInt1, 22, 3, IntegrationHelpers.IsEq},
                { listInt1, 33, 2, IntegrationHelpers.IsEq },
                { listInt1, 11, 2, IntegrationHelpers.IsEq },
                { listInt1, 0, 0, IntegrationHelpers.IsEq },
                { new List<int>(), 22, 0, IntegrationHelpers.IsEq },
            };

        static List<DateTime> listDt1 => new List<DateTime>() {
            DateTime.MinValue.AddDays(0),
            DateTime.MinValue.AddDays(1),
            DateTime.MinValue.AddDays(2),
            DateTime.MinValue.AddDays(2),
            DateTime.MinValue.AddDays(1),
            DateTime.MinValue.AddDays(0),
        };

        public static TheoryData<IList<DateTime>, DateTime, int> RemoveAnyDtData() =>
            new TheoryData<IList<DateTime>, DateTime, int>
            {
                { listDt1, DateTime.MinValue.AddDays(1), 2},
                { listDt1, DateTime.MinValue.AddDays(2), 2},
                { listDt1, DateTime.MinValue.AddDays(0), 2},
                { listDt1, DateTime.MinValue.AddDays(9), 0},
                { new List<DateTime>(), DateTime.MinValue.AddDays(1), 0},
            };

        static List<ClA> ListClA => new List<ClA>()
        {
            new ClA("aa"),
            new ClA("bb"),
            new ClA("cc"),
            new ClA("cc"),
            new ClA("bb"),
            new ClA("aa"),
        };

        public static TheoryData<IList<ClA>, string, int, Func<ClA, string, bool>> RemoveAnyClAData() =>
            new TheoryData<IList<ClA>, string, int, Func<ClA, string, bool>>
            {
                { ListClA, "bb", 2, ClA.IsKey},
                { ListClA, "cc", 2, ClA.IsKey},
                { ListClA, "aa", 2, ClA.IsKey },
                { ListClA, "xx", 0, ClA.IsKey },
                { new List<ClA>(), "bb", 0, ClA.IsKey },
                // { ListClA, null, 0, ClA.IsKey },
            };


        public class ClA
        {
            public static bool IsKey(ClA instance, string key)
                => string.Equals(key, instance?.Key, StringComparison.Ordinal);

            public readonly string Key;

            public ClA(string key)
            {
                this.Key = key;
            }
        }
    }
}
