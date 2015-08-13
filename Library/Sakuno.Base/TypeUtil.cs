using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Sakuno
{
    public static class TypeUtil
    {
        public static Dictionary<string, string> DumpAsDictionary(object rpObject)
        {
            return TypeDescriptor.GetProperties(rpObject).OfType<PropertyDescriptor>().ToDictionary(r => r.Name, r => r.GetValue(rpObject).ToString());
        }

        public static string DumpAsQueryString(object rpObject) => DumpAsEncodedUrl(rpObject);
        public static string DumpAsEncodedUrl(object rpObject)
        {
            var rBuilder = new StringBuilder(100);
            var rProperties = TypeDescriptor.GetProperties(rpObject).OfType<PropertyDescriptor>().OrderBy(r => r.Name).ToArray();
            for (int i = 0; i < rProperties.Length; i++)
            {
                var rProperty = rProperties[i];

                var rName = Uri.EscapeDataString(rProperty.Name);
                var rValue = rProperty.GetValue(rpObject);
                var rClassName = rValue == null ? string.Empty : TypeDescriptor.GetClassName(rValue);

                if (rClassName.StartsWith("<>f__AnonymousType"))
                    rBuilder.Append(DumpNestedObject(rValue, rName));
                else
                {
                    var rValueData = Uri.EscapeDataString(rProperty.GetValue(rpObject).ToString());
                    rBuilder.AppendFormat("{0}={1}", rName, rValueData);
                }

                if (i != rProperties.Length - 1)
                    rBuilder.Append('&');
            }

            return rBuilder.ToString();
        }

        static string DumpNestedObject(object rpObject, string rpName)
        {
            var rBuilder = new StringBuilder(100);
            var rProperties = TypeDescriptor.GetProperties(rpObject);
            for (int i = 0; i < rProperties.Count; i++)
            {
                var rProperty = rProperties[i];

                var rName = Uri.EscapeDataString(rProperty.Name);
                var rValue = Uri.EscapeDataString(rProperty.GetValue(rpObject).ToString());
                rBuilder.AppendFormat("{0}[{1}]={2}", rpName, rName, rValue);

                if (i != rProperties.Count - 1)
                    rBuilder.Append('&');
            }

            return rBuilder.ToString();
        }
    }
}
