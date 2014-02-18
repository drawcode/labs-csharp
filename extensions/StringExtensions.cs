using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

using Labs.Utility;

public static class StringExtensions {

    public static string ToPascalCase(this string val) {
        string output = "";
        char[] chars = val.ToCharArray();
        bool firstUpper = true;
        bool upperNext = false;
        foreach(char ch in chars){
            if (char.IsUpper(ch)) {
                if (firstUpper) {
                    firstUpper = false;
                    output += ch.ToString().ToLower();

                }
                else if (ch == '-' || ch == '_') {
                    // skip but cap next
                    upperNext = true;
                }
                else {
                    if (upperNext) {
                        upperNext = false;
                        output += ch.ToString().ToUpper();
                    }
                }
            }
            else {
                output += ch.ToString();
            }
        }
        return output;
    }

    public static string ToTitleCase(this string val) {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(val);
    }

    public static string ToDelimitedUnderscore(this string val) {
        return val.ToDelimited("_");
    }

    public static string ToDelimitedDashed(this string val) {
        return val.ToDelimited("-");
    }

    public static string ToSlug(this string val) {
        return val.Replace(",","_").ToDelimitedDashed().Replace(" ","");
    }

    public static string ToDelimited(this string val, string delimiter = "-") {
        string output = "";
        char[] chars = val.ToCharArray();
        foreach (char ch in chars) {
            if (char.IsUpper(ch)) {
                if (output.Length > 0) {
                    output += delimiter;
                }
                output += ch.ToString().ToLower();
            }
            else {
                output += ch.ToString();
            }
        }
        return output;
    }

    public static string ToNonDelimited(this string val, string replaceDelimeter = " ", string delimiter = "-") {
        if (val == null) {
            return null;
        }
        val = val.Replace(delimiter, replaceDelimeter);
        return val;
    }
        
    public static string ChangePathParamValue(this string path, string param, string paramvalue) {
        return Paths.ChangePathParamValue(path, param, paramvalue);
    }
    
    public static string ChangePathParamFilterValue(this string path, string param, string paramvalue) {
        return Paths.ChangePathParamFilterValue(path, param, paramvalue);
    }

    public static string GetPathParamValue(string path, string param) {
        return Paths.GetPathParamValue(path, param);
    }
    
    public static string GetPathParamFilterValue(string path, string param) {
        return Paths.GetPathParamFilterValue(path, param);
    }
        
    public static string FilterToUrlFormat(this string input) {
        return Paths.FilterToUrlFormat(input);
    }
    
    public static string StripToAlphanumerics(this string input) {
        string output = "";
		output = Regex.Replace(input, @"[^\w\s]|", "");
		//.replace(/\s+/g, " ");
		return output;
    }
    
    public static bool IsRegexMatch(this string input, string pattern) {
        if (string.IsNullOrEmpty(input)) {
            return false;
        }

        //^[A-Z][a-z]*( [A-Z][a-z]*)*$
        if (Regex.IsMatch(input, pattern)) {
            return true;
        }
        return false;
    }
    
    public static bool EndsWithSlash(this string input) {
        return input.IsRegexMatch("/$");
    }
    
    public static Match RegexMatch(this string input, string pattern) {
        return Regex.Match(input, pattern);
    }
    
    public static MatchCollection RegexMatches(this string input, string pattern) {
        return Regex.Matches(input, pattern);
    }

    public static string pathForDocumentsResource(string file) {
        return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), file);
    }

    public static string Substring(this string str, string StartString, string EndString) {
        if (str.Contains(StartString)) {
            int iStart = str.IndexOf(StartString) + StartString.Length;
            int iEnd = str.IndexOf(EndString, iStart);
            return str.Substring(iStart, (iEnd - iStart));
        }
        return null;
    }

    public static string ToHashMD5(this string val) {
        return Cryptos.HashMD5(val);
    }

    public static string ToJSON(this string val, object obj) {
        return JsonConvert.SerializeObject(obj).FilterJSON();
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
