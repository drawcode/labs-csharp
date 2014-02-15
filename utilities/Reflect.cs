using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Reflection;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Utilities;

namespace Labs.Utility {
    public static class Reflect {

        public static PropertyInfo[] GetProperties(object obj) {
            PropertyInfo[] props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            return props;
        }

        public static void CopyTo<T>(this T source, ref T dest) {
            Clone<T>(source, dest);
        }
        
        public static T CopyTo<T>(this T source, T dest) {
            return Clone<T>(source, dest);
        }
        
        public static void Clone<T>(T source, ref T dest) {
            Clone(source, dest);
        }

        public static T Clone<T>(T source, T dest) {

            if (source == null || dest == null)
                throw new Exception("Source or/and Destination Objects are null");

            Type typeDest = dest.GetType();
            Type typeSrc = source.GetType();

            PropertyInfo[] srcProps = typeSrc.GetProperties();
            foreach (PropertyInfo srcProp in srcProps) {
                if (!srcProp.CanRead) {
                    continue;
                }
                PropertyInfo targetProperty = typeDest.GetProperty(srcProp.Name);
                if (targetProperty == null) {
                    continue;
                }
                if (!targetProperty.CanWrite) {
                    continue;
                }
                if ((targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0) {
                    continue;
                }
                if (!targetProperty.PropertyType.IsAssignableFrom(srcProp.PropertyType)) {
                    continue;
                }

                targetProperty.SetValue(dest, srcProp.GetValue(source, null), null);
            }

            return dest;
        }

        public static T ObjectSyncFieldValues<T>(T source, T dest) {

            foreach (FieldInfo field in source.GetType().GetFields()) {
                if (field != null) {

                    object val = field.GetValue(source);
                    if (val != null) {
                        string valString = Convert.ToString(val);
                        if (!string.IsNullOrEmpty(valString)) {
                            field.SetValue(dest, val);
                        }
                    }
                }
            }

            foreach (PropertyInfo prop in source.GetType().GetProperties()) {
                if (prop != null) {
                    object val = prop.GetValue(source);
                    if (val != null) {
                        string valString = Convert.ToString(val);
                        if (!string.IsNullOrEmpty(valString)) {
                            prop.SetValue(dest, val, null);
                        }
                    }
                }
            }

            return dest;

        }

        public static void ObjectSyncFieldValues<T>(T source, ref T dest) {
            ObjectSyncFieldValues<T>(source, dest);
        }
                
        public static void ObjectSyncFieldValuesSerialize<T>(T source, ref T dest) {

            string objData = JsonConvert.SerializeObject(source);
            source = JsonConvert.DeserializeObject<T>(objData);

            ObjectSyncFieldValues<T>(source, ref dest);
        }

        public static T GetFieldValue<T>(object obj, string fieldName) {
            return (T)GetFieldValue(obj, fieldName);
        }

        public static object GetFieldValue(object obj, string fieldName) {
            ////Debug.Log("GetFieldValue:obj.GetType():" + obj.GetType());

            bool hasGet = false;

            foreach (FieldInfo field in fieldName.Split('.').Select(s => obj.GetType().GetField(s))) {
                if (field != null) {
                    if (obj != null) {
                        obj = field.GetValue(obj);
                        hasGet = true;
                    }
                }
            }

            if (!hasGet) {
                foreach (PropertyInfo prop in obj.GetType().GetProperties()) {
                    if (prop.Name == fieldName) {
                        obj = prop.GetValue(obj, null);
                    }
                }
            }

            return obj;
        }

        public static void SetFieldValue(object obj, string fieldName, object fieldValue) {
            foreach (FieldInfo field in fieldName.Split('.').Select(s => obj.GetType().GetField(s))) {
                if (field != null) {
                    field.SetValue(obj, fieldValue);
                }
            }
            
            foreach (PropertyInfo prop in obj.GetType().GetProperties()) {
                if (prop.Name == fieldName) {
                    prop.SetValue(obj, fieldValue, null);
                }
            }
        }
    }    
}
