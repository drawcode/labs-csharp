using System;
using System.Collections.Generic;

public static class ListExtensions {

    public static void Shuffle<T>(this IList<T> list) {
        var randomNumber = new Random(DateTime.Now.Millisecond);
        var n = list.Count;
        while (n > 1) {
            n--;
            var k = randomNumber.Next(n + 1);
            var value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static void Set<T>(this List<T> list, T val) {
        if (list != null) {
            if (list.Contains(val)) {

                // T current = list.Find(u => u == val);
            }
            else {
                list.Add(val);
            }
        }
    }

    public static T[] RemoveAt<T>(this T[] source, int index) {
        T[] dest = new T[source.Length - 1];
        if (index > 0)
            Array.Copy(source, 0, dest, 0, index);

        if (index < source.Length - 1)
            Array.Copy(source, index + 1, dest, index, source.Length - index - 1);

        return dest;
    }
}
