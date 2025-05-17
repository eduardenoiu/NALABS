using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;

namespace RCM.Extensions
{
    public static class DataTableExtensions
    {
        public static void ToJson(this System.Data.DataTable dt, string filePath)
        {
            List<Dictionary<string, object>> lst = new List<Dictionary<string, object>>();
            Dictionary<string, object> item;
            foreach (DataRow row in dt.Rows)
            {
                item = new Dictionary<string, object>();
                foreach (DataColumn col in dt.Columns)
                {
                    item.Add(col.ColumnName, (Convert.IsDBNull(row[col]) ? null : row[col]));
                }

                lst.Add(item);
            }

            var json = JsonConvert.SerializeObject(new { Rows = lst.ToArray(), Formatting.Indented });

            if (Environment.GetEnvironmentVariable("CI") == "true")
            {
                Console.Write(json);
            }
            else
            {
                File.WriteAllText(filePath, json);
            }
        }

        public static System.Data.DataTable ToDataTable<T>(this List<T> items)
        {
            System.Data.DataTable dt = new System.Data.DataTable(typeof(T).Name);
            object[] objPropertyValues;
            int i;

            PropertyInfo[] objProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in objProperties)
            {
                dt.Columns.Add(prop.Name, GetBaseType(prop.PropertyType));
            }

            foreach (T item in items)
            {
                objPropertyValues = new object[objProperties.Length];

                for (i = 0; i < objProperties.Length; i++)
                {
                    objPropertyValues[i] = objProperties[i].GetValue(item, null);
                }

                dt.Rows.Add(objPropertyValues);
            }

            return dt;
        }

        private static Type GetBaseType(Type objType)
        {
            return (objType != null && objType.IsValueType &&
                objType.IsGenericType && objType.GetGenericTypeDefinition() == typeof(Nullable<>)) ? Nullable.GetUnderlyingType(objType) : objType;
        }
    }
}
