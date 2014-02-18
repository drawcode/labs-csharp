using System;
using System.Collections;
using System.Collections.Generic;

public static class DictionaryExtensions {

    public static void Merge<TKey, TValue>(this IDictionary<TKey, TValue> dict, IDictionary<TKey, TValue> dictNew) {
        if (dict != null && dictNew != null) {
            foreach (KeyValuePair<TKey, TValue> pair in dictNew) {
                dict.Set(pair.Key, pair.Value);
            }
        }
    }

    public static void Merge(this IDictionary<string, object> dict, IDictionary<string, object> dictNew) {
        if (dict != null && dictNew != null) {
            foreach (KeyValuePair<string, object> pair in dictNew) {
                dict.Set(pair.Key, pair.Value);
            }
        }
    }

    public static void Set(this IDictionary<string, object> dict, string key, object val) {
        if (dict != null) {
            if (dict.ContainsKey(key)) {
                dict[key] = val;
            }
            else {
                dict.Add(key, val);
            }
        }
    }

    public static void Set<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue val) {
        if (dict != null) {
            if (dict.ContainsKey(key)) {
                dict[key] = val;
            }
            else {
                dict.Add(key, val);
            }
        }
    }

    public static object Get<T>(this IDictionary<string, object> dict, string key) {
        if (dict != null) {
            if (dict.ContainsKey(key)) {
                return dict[key];
            }
        }

        return null;
    }

    public static T Get<T>(this IDictionary<string, T> dict, string key) {
        if (dict != null) {
            if (dict.ContainsKey(key)) {
                return (T)dict[key];
            }
        }

        return default(T);
    }

    public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) {
        if (dict != null) {
            if (dict.ContainsKey(key)) {
                return dict[key];
            }
        }

        return default(TValue);
    }
}
