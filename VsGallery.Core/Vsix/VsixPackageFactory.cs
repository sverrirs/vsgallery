using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

using VsGallery.Core.Vsix.Schemas;


namespace VsGallery.Core.Vsix
{
    public sealed class VsixPackageFactory
    {
        public const string Manifest = "extension.vsixmanifest";

        private readonly XmlSerializer mPackageManifestSerializer = new XmlSerializer(typeof(PackageManifest));
        private readonly XmlSerializer mVsixManifestSerializer = new XmlSerializer(typeof(Schemas.Vsix));

        public IVsixPackage LoadFromFile(string file)
        {
            using (var zipFile = ZipFile.OpenRead(file))
            {
                var manifestEntry = zipFile.Entries.FirstOrDefault(entry => entry.FullName == Manifest);
                if (manifestEntry == null)
                    return null;

                return CreateFromZipManifest(file, manifestEntry);
            }
        }

        public IEnumerable<IVsixPackage> LoadAll(string directory, bool onlyNewest = true)
        {
            var packages = Directory
                .EnumerateFiles(directory, "*.vsix", SearchOption.AllDirectories)
                .Select(LoadFromFile);

            return onlyNewest ? packages.OnlyMostRecentVersions() : packages;
        }

        private IVsixPackage CreateFromZipManifest(string file, ZipArchiveEntry manifestEntry)
        {
            var packageManifest = TryGetPackageManifest(manifestEntry);
            if(packageManifest != null )
                return new VsixWithPackageManifest(file, packageManifest);

            var vsixManifest = TryGetVsixManifest(manifestEntry);
            if (vsixManifest != null)
                return new VsixWithVsixManifest(file, vsixManifest);

            return null;
            //return ef.Error($"{file}\\{Entry.Manifest} is not a valid package manifest file.");
        }

        private PackageManifest TryGetPackageManifest(ZipArchiveEntry manifestEntry)
        {
            using (var reader = manifestEntry.Open())
            using (var xmlReader = XmlReader.Create(reader, new XmlReaderSettings
            {
                Schemas = SchemaSets.PackageManifest
            }))
            {
                if (!mPackageManifestSerializer.CanDeserialize(xmlReader))
                {
                    return null;
                }

                return (PackageManifest)mPackageManifestSerializer.Deserialize(xmlReader);
            }
        }

        private Schemas.Vsix TryGetVsixManifest(ZipArchiveEntry manifestEntry)
        {
            using (var reader = manifestEntry.Open())
            using (var xmlReader = XmlReader.Create(reader, new XmlReaderSettings
            {
                Schemas = SchemaSets.VsixManifest
            }))
            {
                if (!mVsixManifestSerializer.CanDeserialize(xmlReader))
                {
                    return null;
                }

                return (Schemas.Vsix)mVsixManifestSerializer.Deserialize(xmlReader);
            }
        }
    }
}
