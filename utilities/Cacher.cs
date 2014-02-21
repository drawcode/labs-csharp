using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace Labs.Utility {

    public class Cacher {
        private static volatile Cacher instance;
        private static System.Object syncRoot = new System.Object();

        public static Cacher Instance {
            get {
                if (instance == null) {
                    lock (syncRoot) {
                        if (instance == null)
                            instance = new Cacher();
                    }
                }

                return instance;
            }
            set {
                instance = value;
            }
        }

        private MemoryCache cache;

        public Cacher() {
            cache = MemoryCache.Default;
        }

        // CHECK

        public static bool Has(string key) {
            return Instance.has(key);
        }

        public bool has(string key) {
            if (cache.Contains(key)) {
                return true;
            }
            return false;
        }

        // DELETE

        public static bool Delete(string key) {
            return Instance.delete(key);
        }

        public static bool DeleteLike(string key) {
            return Instance.deleteObjectLike(key);
        }

        public bool delete(string key) {
            return deleteObject(key);
        }

        public bool deleteObject(string key) {
            if (has(key)) {
                cache.Remove(key);
                return true;
            }

            return false;
        }

        public bool deleteAll() {
            foreach (KeyValuePair<bool, KeyValuePair<string, object>> item
                in cache.ToDictionary(u => u.Key != "")) {
                cache.Remove(item.Value.Key);
            }

            return true;
        }

        public bool deleteObjectLike(string key) {
            try {
                foreach (KeyValuePair<int, KeyValuePair<string, object>> item
                    in cache.ToDictionary(u => u.Key.IndexOf(key))) {
                    cache.Remove(item.Value.Key);
                }

                return true;
            }
            catch (Exception e) {
                Web.Log("e:", e);
                return false;
            }
        }

        // GET

        public static object Get(string key) {
            return Instance.get(key);
        }

        public object get(string key) {
            return getObject(key);
        }

        public object getObject(string key) {
            object obj = null;

            if (has(key)) {
                obj = cache.Get(key);
            }

            return obj;
        }

        // SET

        public static void Set(string key, object obj) {
            Instance.set(key, obj);
        }

        public void set(string key, object obj) {
            setObject(key, obj);
        }

        public void setObject(string key, object obj) {
            if (obj == null || string.IsNullOrEmpty(key))
                return;

            CacheItemPolicy policy = new CacheItemPolicy();
            policy.SlidingExpiration = TimeSpan.FromDays(90);
            cache.Set(key, obj, policy);
        }

        // HELPERS

        // DICTIONARY

        public static Dictionary<T, U> GetCachedDictionary<T, U>(string key, bool clearCache) {
            return Instance.getCachedDictionary<T, U>(key, clearCache);
        }

        public Dictionary<T, U> getCachedDictionary<T, U>(string key, bool clearCache) {
            if (Cacher.Has(key) || clearCache) {
                return Cacher.Get(key) as Dictionary<T, U>;
            }
            return null;
        }

        // OBJ

        public static T GetCached<T>(string key, bool clearCache) {
            return Instance.getCached<T>(key, clearCache);
        }

        public T getCached<T>(string key, bool clearCache) {
            if (Cacher.Has(key) || clearCache) {
                return (T)Cacher.Get(key);
            }
            return default(T);
        }

        // LIST

        public static List<T> GetCachedList<T>(string key, bool clearCache) {
            return Instance.getCachedList<T>(key, clearCache);
        }

        public List<T> getCachedList<T>(string key, bool clearCache) {
            if (Cacher.Has(key) || clearCache) {
                return Cacher.Get(key) as List<T>;
            }
            return null;
        }

        // OBJECTS

        public T GetCachedObject<T>(string key, bool clearCache) {
            return Instance.getCachedObject<T>(key, clearCache);
        }

        public T getCachedObject<T>(string key, bool clearCache) {
            if (Cacher.Has(key) || clearCache) {
                return (T)Cacher.Get(key);
            }
            return default(T);
        }
    }
}
