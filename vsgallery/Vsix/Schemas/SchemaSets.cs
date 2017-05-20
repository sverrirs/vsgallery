using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;

namespace VISXPrivateGallery.Vsix.Schemas
{
    public static class SchemaSets
    {
        private static readonly Lazy<XmlSchemaSet> mAtomSchemaSet =
            new Lazy<XmlSchemaSet>(() => _LoadSchemaSetFromSubdirectory("Atom"));

        private static readonly Lazy<XmlSchemaSet> mPackageManifestSchemaSet =
                    new Lazy<XmlSchemaSet>(() => _LoadSchemaSetFromSubdirectory("PackageManifest"));

        private static readonly Lazy<XmlSchemaSet> mVsixManifestSchemaSet =
            new Lazy<XmlSchemaSet>(() => _LoadSchemaSetFromSubdirectory("VsixManifest"));

        public static XmlSchemaSet Atom => mAtomSchemaSet.Value;
        public static XmlSchemaSet PackageManifest => mPackageManifestSchemaSet.Value;
        public static XmlSchemaSet VsixManifest => mVsixManifestSchemaSet.Value;

        private static IEnumerable<string> _GetResourceNamesIn(string subdirectory)
        {
            var rootType = typeof(SchemaSets);
            var rootNamespace = $"{rootType.Namespace}.{subdirectory}";

            return rootType
                .Assembly.GetManifestResourceNames()
                .Where(name => name.StartsWith(rootNamespace) &&
                               name.EndsWith(".xsd", StringComparison.OrdinalIgnoreCase));
        }

        private static XmlSchemaSet _LoadSchemaSetFromSubdirectory(string subdirectory)
        {
            var schemaSet = new XmlSchemaSet();
            var rootType = typeof(SchemaSets);

            var resourceNames = _GetResourceNamesIn(subdirectory);

            foreach (var resource in resourceNames)
            {
                using (var stream = rootType.Assembly.GetManifestResourceStream(resource))
                {
                    schemaSet.Add(XmlSchema.Read(stream, (sender, args) => { }));
                }
            }

            return schemaSet;
        }
    }
}
