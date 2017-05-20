using System.IO;
using IniParser;
using IniParser.Model;

namespace VISXPrivateGallery
{
    public abstract class BaseConfiguration
    {
        protected KeyDataCollection Data { get; }

        protected BaseConfiguration(KeyDataCollection data)
        {
            Data = data;
        }
    }

    public class Configuration : IConfiguration
    {
        public IHostConfiguration Hosting { get; }

        public IStorageConfiguration Storage { get; }

        public IInstallConfiguration Install { get; }

        public IGalleryConfiguration Gallery { get; }


        public Configuration(string configFilePath)
        {
            // Load the configuration data
            var configData = new FileIniDataParser().ReadFile(configFilePath);

            // Set the section retrieval
            Hosting = new HostConfiguration(configData[nameof(Hosting)]);
            Storage = new StorageConfiguration(configData[nameof(Storage)]);
            Install = new InstallConfiguration(configData[nameof(Install)]);
            Gallery = new GalleryConfiguration(configData[nameof(Gallery)]);

            // Ensure that directories exist
            if (!Directory.Exists(Storage.VsixStorageDirectory))
                Directory.CreateDirectory(Storage.VsixStorageDirectory);
        }
    }

    public class GalleryConfiguration : BaseConfiguration, IGalleryConfiguration
    {
        public GalleryConfiguration(KeyDataCollection data) : base(data){}
        public string Guid => Data[nameof(Guid)];
        public string Title => Data[nameof(Title)];
        public string Description => Data[nameof(Description)];
        public bool TrackDownloads => bool.Parse(Data[nameof(TrackDownloads)]);
        public bool TrackRatings => bool.Parse(Data[nameof(TrackRatings)]);
    }


    public class InstallConfiguration : BaseConfiguration, IInstallConfiguration
    {
        public string Guid => Data[nameof(Guid)];
        public int Priority => int.Parse(Data[nameof(Priority)]);
        public string DisplayName => Data[nameof(DisplayName)];
        public string Description => Data[nameof(Description)];
        public string ServiceName => Data[nameof(ServiceName)];

        public InstallConfiguration(KeyDataCollection data) : base(data) { }
    }

    public class StorageConfiguration : BaseConfiguration, IStorageConfiguration
    {
        public string VsixStorageDirectory => Data[nameof(VsixStorageDirectory)];

        public StorageConfiguration(KeyDataCollection data) : base(data) { }
    }


    public class HostConfiguration : BaseConfiguration, IHostConfiguration
    {
        public string HostName => Data[nameof(HostName)];

        public string Port => Data[nameof(Port)];

        public bool UseSSL => bool.Parse(Data[nameof(UseSSL)]);

        public bool Diagnostics => bool.Parse(Data[nameof(Diagnostics)]);

        public HostConfiguration(KeyDataCollection data) : base(data) { }
    }


    public interface IConfiguration
    {
        IHostConfiguration Hosting { get; }

        IStorageConfiguration Storage { get; }

        IInstallConfiguration Install { get; }

        IGalleryConfiguration Gallery { get; }
    }

    public interface IStorageConfiguration
    {
        string VsixStorageDirectory { get; }
    }

    public interface IHostConfiguration
    {
        string HostName { get; }

        string Port { get; }

        bool UseSSL { get; }

        bool Diagnostics { get; }
    }

    public interface IInstallConfiguration
    {
        string Guid { get; }
        int Priority { get; }
        string DisplayName { get; }
        string Description { get; }
        string ServiceName { get; }
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
