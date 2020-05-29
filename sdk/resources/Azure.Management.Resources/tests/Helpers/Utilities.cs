using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Azure.Management.Resources.Tests
{
    public static class Utilities
    {
        /// <summary>
        /// Equality comparison for locations returned by resource management
        /// </summary>
        /// <param name="expected">The expected location</param>
        /// <param name="actual">The actual location returned by resource management</param>
        /// <returns>true if the locations are equivalent, otherwise false</returns>
        public static bool LocationsAreEqual(string expected, string actual)
        {
            bool result = string.Equals(expected, actual, System.StringComparison.OrdinalIgnoreCase);
            if (!result && !string.IsNullOrEmpty(expected))
            {
                string normalizedLocation = expected.ToLower().Replace(" ", null);
                result = string.Equals(normalizedLocation, actual, StringComparison.OrdinalIgnoreCase);
            }

            return result;
        }

        public static Dictionary<string, object> DeserializeJson(this string json)
        {
            Dictionary<string, object> result = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            ArrayList mark = new ArrayList();

            foreach (var item in result)
            {
                if (item.Value.GetType() == typeof(System.Collections.ArrayList))
                {
                    mark.Add(item.Key);
                    //ArrayList list = (ArrayList)result[item.Key];
                    //result[item.Key] = list.ToArray();
                }
            }
            if (mark != null)
            {
                foreach (string item in mark)
                {
                    ArrayList list = (ArrayList)result[item];
                    result[item] = list.ToArray();
                }
            }

            return result;
        }
    }
}
