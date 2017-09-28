using System;
using System.Configuration;
using System.Web;

using VsGallery.Core;


namespace VsGallery.WebService
{
    public class StorageConfiguration : ConfigurationElement, IStorageConfiguration
    {
        private HttpServerUtility _server;

        public StorageConfiguration()
        {
            _server = HttpContext.Current.Server;
        }

        const string vsixStorageDirectoryPropertyName = "VsixStorageDirectory";
        [ConfigurationProperty(vsixStorageDirectoryPropertyName, DefaultValue = "~/App_Data/VsixStorage", IsRequired = true)]
        public string VsixStorageDirectory
        {
            get
            {
                var path = this[vsixStorageDirectoryPropertyName] as string;
                return _server.MapPath(path);
            }
            set
            {
                this[vsixStorageDirectoryPropertyName] = value;
            }
        }

        const string vsixUploadDirectoryPropertyName = "UploadDirectory";
        [ConfigurationProperty(vsixUploadDirectoryPropertyName, DefaultValue = "~/App_Data/VsixUploads", IsRequired = true)]
        public string UploadDirectory
        {
            get
            {
                var path = this[vsixUploadDirectoryPropertyName] as string;
                return _server.MapPath(path);
            }
            set
            {
                this[vsixUploadDirectoryPropertyName] = value;
            }
        }

        const string uploadMaxFileSizePropertyName = "UploadMaxFileSize";
        [ConfigurationProperty(uploadMaxFileSizePropertyName, DefaultValue = "IIS Configured", IsRequired = false)]
        public string UploadMaxFileSize
        {
            get
            {
                return this[uploadMaxFileSizePropertyName] as string;
            }
            set
            {
                this[uploadMaxFileSizePropertyName] = value;
            }
        }
    }

    public class GalleryConfiguration : ConfigurationElement, IGalleryConfiguration
    {
        const string guidPropertyName = "Guid";
        [ConfigurationProperty(guidPropertyName, DefaultValue = "{54F7969C-F561-4B16-9D6B-7325EDE0A3C1}", IsRequired = true)]
        public string Guid
        {
            get
            {
                return (string) this[guidPropertyName];
            }
            set
            {
                this[guidPropertyName] = value;
            }
        }

        const string titlePropertyName = "Title";
        [ConfigurationProperty(titlePropertyName, DefaultValue = "Super Productive Gallery", IsRequired = true)]
        public string Title
        {
            get
            {
                return this[titlePropertyName] as string;
            }
            set
            {
                this[titlePropertyName] = value;
            }
        }

        const string descriptionPropertyName = "Description";
        [ConfigurationProperty(descriptionPropertyName, DefaultValue = "Super Productive Gallery Description", IsRequired = false)]
        public string Description
        {
            get
            {
                return this[descriptionPropertyName] as string;
            }
            set
            {
                this[descriptionPropertyName] = value;
            }
        }

        const string trackDownloadsPropertyName = "TrackDownloads";
        [ConfigurationProperty(trackDownloadsPropertyName, DefaultValue = "true", IsRequired = false)]
        public Boolean TrackDownloads
        {
            get
            {
                return (Boolean) this[trackDownloadsPropertyName];
            }
            set
            {
                this[trackDownloadsPropertyName] = value;
            }
        }

        const string trackRatingsPropertyName = "TrackRatings";
        [ConfigurationProperty(trackRatingsPropertyName, DefaultValue = "true", IsRequired = false)]
        public bool TrackRatings
        {
            get
            {
                return (Boolean)this[trackRatingsPropertyName];
            }
            set
            {
                this[trackRatingsPropertyName] = value;
            }
        }
    }

    public class Configuration : ConfigurationSection, IConfiguration
    {
        public static IConfiguration Instance
        {
            get
            {
                return System.Configuration.ConfigurationManager.GetSection("VsGallerySettings") as Configuration;
            }
        }


        const string storageConfigurationElementName = "Storage";

        [ConfigurationProperty(storageConfigurationElementName)]
        public StorageConfiguration Storage
        {
            get
            {
                return (StorageConfiguration)this[storageConfigurationElementName];
            }
            set
            {
                value = (StorageConfiguration)this[storageConfigurationElementName];
            }
        }


        const string galleryConfigurationElementName = "Gallery";

        [ConfigurationProperty(galleryConfigurationElementName)]
        public GalleryConfiguration Gallery
        {
            get
            {
                return (GalleryConfiguration)this[galleryConfigurationElementName];
            }
            set
            {
                value = (GalleryConfiguration)this[galleryConfigurationElementName];
            }
        }

        IStorageConfiguration IConfiguration.Storage => this.Storage;

        IGalleryConfiguration IConfiguration.Gallery => this.Gallery;
    }

    public interface IConfiguration
    {
        IStorageConfiguration Storage { get; }
        IGalleryConfiguration Gallery { get; }
    }
}