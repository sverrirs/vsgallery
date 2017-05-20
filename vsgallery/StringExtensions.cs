using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vsgallery
{
    public static class StringExtensions
    {
        public static string ToTitleCase(this string value)
        {
            return new CultureInfo("en-US", false).TextInfo.ToTitleCase(value);
        }
    }
}
