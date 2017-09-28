using System.Globalization;


namespace VsGallery.Core
{
    public static class StringExtensions
    {
        public static string ToTitleCase(this string value)
        {
            return new CultureInfo("en-US", false).TextInfo.ToTitleCase(value);
        }
    }
}
