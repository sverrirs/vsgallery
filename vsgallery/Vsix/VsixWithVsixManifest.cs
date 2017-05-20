namespace vsgallery.Vsix
{
    public sealed class VsixWithVsixManifest : VsixPackage
    {
        private readonly Schemas.Vsix _manifest;

        public VsixWithVsixManifest(string file, Schemas.Vsix manifest) : base(file)
        {
            _manifest = manifest;
        }

        public override string Description => _manifest.Identifier.Description;
        public override string DisplayName => _manifest.Identifier.Name;
        public override string Id => _manifest.Identifier.Id;
        public override string Publisher => _manifest.Identifier.Author;
        public override string Version => _manifest.Identifier.Version;
        public override string License => _manifest.Identifier.License;

        public override string MoreInfoUrl => _manifest.Identifier.MoreInfoUrl;

        public override string[] Categories => new string[0];

        public override string ReleaseNotes => _manifest.Identifier.ReleaseNotes;

        protected override string IconName => _manifest.Identifier.Icon;
        protected override string PreviewImageName => _manifest.Identifier.PreviewImage;

        public override IVsProduct[] SupportedProducts => new IVsProduct[0];
    }
}
