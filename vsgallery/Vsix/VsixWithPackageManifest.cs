using System.Linq;
using vsgallery.Vsix.Schemas;

namespace vsgallery.Vsix
{
    public sealed class VsixWithPackageManifest : VsixPackage
    {
        private readonly PackageManifest _manifest;

        public VsixWithPackageManifest(string file, PackageManifest manifest) : base(file)
        {
            _manifest = manifest;
        }

        public override string Description => _manifest.Metadata.Description;
        public override string DisplayName => _manifest.Metadata.DisplayName;
        public override string Id => _manifest.Metadata.Identity.Id;
        public override string Publisher => _manifest.Metadata.Identity.Publisher;
        public override string Version => _manifest.Metadata.Identity.Version;
        public override string License => _manifest.Metadata.License;

        public override string ReleaseNotes => _manifest.Metadata.ReleaseNotes;

        public override string MoreInfoUrl => _manifest.Metadata.MoreInfo;

        public override string[] Categories => _manifest.Metadata.Tags?.Split(new[] {',', ';'}) ?? new string[0];

        protected override string IconName => _manifest.Metadata.Icon;
        protected override string PreviewImageName => _manifest.Metadata.PreviewImage;

        public override IVsProduct[] SupportedProducts => _manifest.Installation.InstallationTarget.Select(x =>
        {
            return new VsProduct(x.Version, x.Id);
        }).Cast<IVsProduct>().ToArray();
    }
}
