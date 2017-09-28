namespace VsGallery.Core
{
    public interface IStorageConfiguration
    {
        string VsixStorageDirectory { get; }
        string UploadDirectory { get; }
        string UploadMaxFileSize { get; }

    }

    public interface IGalleryConfiguration
    {
        string Guid { get; }
        string Title { get; }
        string Description { get; }
        bool TrackDownloads { get; }
        bool TrackRatings { get; }
    }
}
