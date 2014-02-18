using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Newtonsoft.Json;

public static class ObjectExtensions {

    public static string ToJSON(this object val) {
        return val.ToJSON(false);
    }

    public static string ToJSON(this object val, bool indented) {
        return JsonConvert.SerializeObject(val, indented ? Formatting.Indented : Formatting.None).FilterJSON();
    }

}
