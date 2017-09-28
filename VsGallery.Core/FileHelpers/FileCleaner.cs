using System.IO;
using System.Text.RegularExpressions;


namespace VsGallery.Core.FileHelpers
{
    public static class FileCleaner
    {
        private static Regex regExFileNameNormalizer = new Regex(@"([^a-zA-z0-9\\\/\.\:\s])*", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static string NormalizePackageFileName(string uncleanFileName)
        {
            var cleanedFileName = regExFileNameNormalizer.Replace(uncleanFileName, "");

            // Rename the file only if needed and it hasn't been done already
            if (cleanedFileName.Equals(uncleanFileName) || File.Exists(cleanedFileName))
                return cleanedFileName;

            File.Move(uncleanFileName, cleanedFileName);
            File.Delete(uncleanFileName);

            return cleanedFileName;
        }
    }
}
