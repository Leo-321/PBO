using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokemonBattleOnline
{
    public static class extensions
    {
        public static void Add<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var i in items) collection.Add(i);
        }

        public static void Remove<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (T i in items) collection.Remove(i);
        }

        public static TValue ValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue)
        {
            TValue r;
            return key == null || !dict.TryGetValue(key, out r) ? defaultValue : r;
        }

        public static TValue ValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            return ValueOrDefault(dict, key, default(TValue));
        }

        public static TKey KeyOf<TKey, TValue>(this IDictionary<TKey, TValue> dict, TValue value)
        {
            foreach (var p in dict)
                if (p.Value.Equals(value)) return p.Key;
            return default(TKey);
        }

        public static T ValueOrDefault<T>(this IList<T> list, int index)
        {
            return 0 <= index && index < list.Count ? list[index] : default(T);
        }

        public static T ValueOrDefault<T>(this T[] array, int index)
        {
            return 0 <= index && index < array.Length ? array[index] : default(T);
        }

        public static T[] SubArray<T>(this T[] array, int offset, int count)
        {
            T[] r = new T[count];
            Array.Copy(array, offset, r, 0, count);
            return r;
        }

        public static T[] SubArray<T>(this T[] array, int offset)
        {
            return SubArray(array, offset, array.Length - offset);
        }

        public static bool ArrayEquals<T>(this T[] a, T[] b)
        {
            return (b != null && a.Length == b.Length && a.SequenceEqual(b));
        }

        public static void Append(this StringBuilder sb, params object[] args)
        {
            foreach (var o in args) sb.Append(o);
        }

        public static void AppendLine(this StringBuilder sb, params object[] args)
        {
            Append(sb, args);
            sb.AppendLine();
        }

        public static Type[] SubClasses(this Type type)
        {
            var types = type.Assembly.GetTypes();
            var stypes = new List<Type>(types.Length);
            if (type.IsInterface)
            {
                foreach (var t in types)
                    if (t.GetInterfaces().Contains(type)) stypes.Add(t);
            }
            else
                foreach (var t in types)
                    if (t.IsSubclassOf(type)) stypes.Add(t);
            return stypes.ToArray();
        }

        public static string LineBreak(this string s)
        {
            //used only within WPF and .txt file in Windows, do not use Environment.NewLine
            return s + "\r\n";
        }
    }
}
