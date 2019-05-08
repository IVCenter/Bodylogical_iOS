using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.ComponentModel;

/// <summary>
/// Referred to from https://github.com/sinbad/UnityCsvUtil/blob/master/CsvUtil.cs
/// </summary>
public static class CSVParser {
    private static readonly char[] delimiters = { ',', ';', '\t' };

    /// <summary>
    /// Loads the csv.
    /// </summary>
    /// <returns>The list of objects with type T.</returns>
    /// <typeparam name="T">The type to load.</typeparam>
    /// <param name="str">csv string. First line is header.</param>
    public static List<T> LoadCsv<T>(string str) where T : new() {
        List<T> objs = new List<T>();
        using (StringReader reader = new StringReader(str)) {
            string[] headers = reader.ReadLine().Split(delimiters);
            FieldInfo[] fields = typeof(T).GetFields();
            Dictionary<string, FieldInfo> headerFieldDict = GenerateDict(headers, fields);

            while (reader.Peek() != -1) {
                string[] objects = reader.ReadLine().Split(delimiters);
                T obj = new T();
                for (int i = 0; i < headers.Length; i++) {
                    FieldInfo field = headerFieldDict[headers[i]];
                    string value = objects[i];
                    if (field.FieldType == typeof(string)) {
                        field.SetValue(obj, value.Trim());
                    } else {
                        TypeConverter converter = TypeDescriptor.GetConverter(field.GetType());
                        object convertedVal = converter.ConvertFromInvariantString(value.Trim());
                        field.SetValue(obj, convertedVal);
                    }
                }
                objs.Add(obj);
            }
        }
        return objs;
    }

    private static Dictionary<string, FieldInfo> GenerateDict(string[] headers, FieldInfo[] fields) {
        Dictionary<string, FieldInfo> dict = new Dictionary<string, FieldInfo>();
        foreach (string header in headers) {
            foreach (FieldInfo field in fields) {
                if (string.Compare(header.Trim(), field.Name, true) == 0) {
                    dict[header] = field;
                    break;
                }
            }
        }
        return dict;
    }
}
