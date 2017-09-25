using System;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace vsgallery.Vsix
{
    public abstract class VsixPackage : IVsixPackage
    {
        protected VsixPackage(string file) => File = file;

        public abstract string Description { get; }

        public abstract string DisplayName { get; }

        public string File { get; set; }

        public abstract string Id { get; }

        public abstract string Publisher { get; }

        public abstract string Version { get; }

        public abstract IVsProduct[] SupportedProducts { get; }

        public abstract string License { get; }

        public abstract string ReleaseNotes { get; }

        public abstract string MoreInfoUrl { get; }

        public abstract string[] Categories { get; }

        #region Ratings and download count handling

        

        #endregion

        #region Icon and Preview Image Handling

        protected abstract string IconName { get; }

        protected abstract string PreviewImageName { get; }

        public bool TrySaveIcon(string destinationFolder, out Uri relativeUri)
        {
            return TrySaveEntry(destinationFolder, IconName, out relativeUri);
        }

        public bool TrySavePreviewImage(string destinationFolder, out Uri relativeUri)
        {
            return TrySaveEntry(destinationFolder, PreviewImageName, out relativeUri);
        }

        private static string NormalizeZipEntryName(string value)
        {
            value = value.Replace('\\', '/');
            value = Regex.Replace(value, "[/]{2,}", "/");
            return value;
        }

        private bool TrySaveEntry(string destinationFolder, string entryName, out Uri relativeUri)
        {
            relativeUri = null;

            if (string.IsNullOrEmpty(entryName))
                return false;

            entryName = NormalizeZipEntryName(entryName);

            using (var zip = ZipFile.OpenRead(File))
            {
                var entry = zip.GetEntry(entryName);
                // Entry could be null because spaces in filenames in a VSIX are encoded to '%20', so retry with encoded entryName
                if (entry == null)
                {
                    entryName = Uri.EscapeUriString(entryName);
                    entry = zip.GetEntry(entryName);
                }
                if (entry != null)
                {
                    var itemPath = Path.Combine(destinationFolder, entryName);

                    // If the icon is stored in a path then create this path
                    var itemDirectoryPath = Path.GetDirectoryName(itemPath);
                    if( itemDirectoryPath != null )
                        Directory.CreateDirectory(itemDirectoryPath);

                    entry.ExtractToFile(new Uri(itemPath).LocalPath, true);

                    relativeUri = new Uri(itemPath);
                    return true;
                }

                return false;
            }
        }

        #endregion
    }

    public class VsProduct : IVsProduct
    {

        public string Version { get; }
        public string Edition { get; }

        public VsProduct(string version, string edition)
        {
            Version = version;
            Edition = edition;
        }
    }
}
