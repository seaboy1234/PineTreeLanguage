using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Interpreter.Extensions
{
    public delegate bool Predicate<T1, T2>(T1 item1, T2 item2);

    public static class EnumerableExtensions
    {
        public static bool All<T>(this IEnumerable<T> collection, Predicate<int, T> predicate)
        {
            int index = 0;
            foreach (T item in collection)
            {
                if (!predicate(index++, item))
                {
                    return false;
                }
            }
            return true;
        }

        public static IEnumerable<T> Each<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (T item in collection)
            {
                action(item);
                yield return item;
            }
        }

        public static IEnumerable<T> Each<T>(this IEnumerable<T> collection, Action<int, T> action)
        {
            int index = 0;
            foreach (T item in collection)
            {
                action(index++, item);
                yield return item;
            }
        }
    }
}