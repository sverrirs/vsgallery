using System;

namespace VISXPrivateGallery.Vsix
{
    public interface IVsixPackage
    {
        string Description { get; }
        string DisplayName { get; }
        string File { get; }
        string Id { get; }
        string Publisher { get; }
        string Version { get; }
        IVsProduct[] SupportedProducts { get; }

        string License { get; }

        string ReleaseNotes { get; }

        string MoreInfoUrl { get; }

        string[] Categories { get; }

        bool TrySaveIcon(string destinationFolder, out Uri uri);

        bool TrySavePreviewImage(string destinationFolder, out Uri uri);
    }

    public interface IVsProduct
    {
        string Version { get; }
        string Edition { get; }
    }
}
