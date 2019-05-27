using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CraftBot.Helper
{
    public static class RandomItem
    {
        public static Random Random { get; } = new Random();

        public static T GetRandomItem<T>(this IEnumerable<T> ts) => ts.ElementAt(Random.Next(0, ts.Count() - 1));

        public static T GetRandomItem<T>(this IList<T> ts) => ts[Random.Next(0, ts.Count() - 1)];

        public static T GetRandomItem<T>(this List<T> ts) => GetRandomItem<T>(ts);
    }
}