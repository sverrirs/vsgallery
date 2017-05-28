using System;
using System.CodeDom;
using System.ComponentModel;
using System.IO;
using System.Net.Http.Headers;
using IniParser;
using IniParser.Model;

namespace vsgallery
{
    public abstract class BaseConfiguration
    {
        private KeyDataCollection Data { get; }

        protected BaseConfiguration(KeyDataCollection data)
        {
            Data = data;
        }

        protected T Get<T>(string keyName) => Get(keyName, default(T));

        protected T Get<T>(string keyName, T defaultValue)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(keyName) || !Data.ContainsKey(keyName))
                    return defaultValue;

                // Extract the raw data
                var keyValue = Data[keyName];

                // Don't do any conversion for null value!
                if (keyValue == null)
                    return defaultValue;

                var converted = Convert.ChangeType(Data[keyName], typeof(T));
                return converted is T ? (T)converted : defaultValue;
            }
            catch( Exception ex)
            {
                throw new FormatException("Could not convert value for key '" + keyName + "' in the config.ini file. Please correct your configuration and run the program again.", ex);
            }
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
        public string Guid => Get<string>(nameof(Guid));
        public string Title => Get<string>(nameof(Title));
        public string Description => Get<string>(nameof(Description));
        public bool TrackDownloads => Get<bool>(nameof(TrackDownloads));
        public bool TrackRatings => Get<bool>(nameof(TrackRatings));
    }


    public class InstallConfiguration : BaseConfiguration, IInstallConfiguration
    {
        public string Guid => Get<string>(nameof(Guid));
        public int Priority => Get<int>(nameof(Priority));
        public string DisplayName => Get<string>(nameof(DisplayName));
        public string Description => Get<string>(nameof(Description));
        public string ServiceName => Get<string>(nameof(ServiceName));

        public InstallConfiguration(KeyDataCollection data) : base(data) { }
    }

    public class StorageConfiguration : BaseConfiguration, IStorageConfiguration
    {
        public string VsixStorageDirectory => Get<string>(nameof(VsixStorageDirectory));
        public string UploadDirectory => Get<string>(nameof(UploadDirectory));
        public string UploadMaxFileSize => Get<string>(nameof(UploadMaxFileSize));

        public StorageConfiguration(KeyDataCollection data) : base(data) { }
    }


    public class HostConfiguration : BaseConfiguration, IHostConfiguration
    {
        public string HostName => Get<string>(nameof(HostName));

        public int Port => Get<int>(nameof(Port));

        public bool UseSSL => Get<bool>(nameof(UseSSL));

        public bool Diagnostics => Get<bool>(nameof(Diagnostics));

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
        string UploadDirectory { get; }
        string UploadMaxFileSize { get; }

    }

    public interface IHostConfiguration
    {
        string HostName { get; }
        int Port { get; }
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
