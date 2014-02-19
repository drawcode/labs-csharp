using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json;

using Labs.Utility;

public static class ObjectExtensions {

    public static string ToJSON(this object val) {
        return val.ToJSON(false);
    }

    public static string ToJSON(this object val, bool indented) {
        return JsonConvert.SerializeObject(val, indented ? Formatting.Indented : Formatting.None).FilterJSON();
    }

    public static T FromJSON<T>(this string val) {
        if (!string.IsNullOrEmpty(val)) {
            try {
                return JsonConvert.DeserializeObject<T>(val.FilterJSON());
            }
            catch (Exception e) {
                Web.Log("FromJSON:e:", e);
                return default(T);
            }
        }
        else {
            return default(T);
        }
    }
    public static string FilterJSON(this string val) {
        if (string.IsNullOrEmpty(val))
            return val;
        return val.Replace("\\\"", "\"").TrimStart('"').TrimEnd('"');
    }

    public static object ConvertJSON(this string val) {
        if (val.StartsWith("{") || val.StartsWith("[")) {
            try {

                if (val.TrimStart().StartsWith("{")) {
                    return val.FilterJSON().FromJSON<Dictionary<string, object>>();
                }
                else if (val.TrimStart().StartsWith("[")) {
                    return val.FilterJSON().FromJSON<List<object>>();
                }

            }
            catch (Exception e) {
                Web.Log("ERROR parsing attribute:", e);
            }
        }

        return val;
    }
}
