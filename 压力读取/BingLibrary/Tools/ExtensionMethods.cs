using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BingLibrary.hjb.tools
{
    public static class ExtensionMethods
    {
        public static T Do<T>(this T t, Action<T> action)
        {
            action(t);
            return t;
        }

        public static int ToInt(this string str)
        {
            int n = 0;
            return (!int.TryParse(str, out n)) ? 0 : n;
        }

        public static string[] ToStrArray(this string str)
        {
            return str?.Substring(0, str.Length - 2).Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        }

    }
}
