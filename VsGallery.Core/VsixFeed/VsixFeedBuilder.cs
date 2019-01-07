﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

using VsGallery.Core.FileHelpers;
using VsGallery.Core.Threading;
using VsGallery.Core.Vsix;


namespace VsGallery.Core.VsixFeed
{
    public class VsixFeedBuilderResults
    {
        public SyndicationFeed Feed { get; }

        public VsixFeedBuilderResults(SyndicationFeed feed)
        {
            Feed = feed;
        }
    }

    public class VsixFeedBuilderProgressArgs : BackgroundProgressArgs
    {
        public string Filename { get; }

        public VsixFeedBuilderProgressArgs(int currentStep, int totalSteps, string filename) : base(currentStep, totalSteps)
        {
            Filename = filename;
        }
    }

    public class VsixFeedBuilder : AsyncBackgroundProcessor<VsixFeedBuilderResults, VsixFeedBuilderProgressArgs, string>
    {
        private readonly IStorageConfiguration _configStorage;
        private readonly IGalleryConfiguration _configGallery;

        public VsixFeedBuilder(IStorageConfiguration storageConfig, IGalleryConfiguration galleryConfig)
        {
            _configStorage = storageConfig;
            _configGallery = galleryConfig;
        }

        protected override Func<VsixFeedBuilderResults> CreateAsyncProcess(string vsixStoragePath, CancellationToken cancellationToken)
        {
            return () => Run(vsixStoragePath);
        }

        private VsixFeedBuilderResults Run(string vsixStoragePath)
        {
            var packageFactory = new VsixPackageFactory();

            var feed = CreateAtomFeed(Path.Combine(vsixStoragePath, "atom.xml"), packageFactory.LoadAll(vsixStoragePath));

            return new VsixFeedBuilderResults(feed);
        }

        private SyndicationFeed CreateAtomFeed(string atomFeedFilePath, IEnumerable<IVsixPackage> packages)
        {
            var rootDirectory = new Uri(Path.GetDirectoryName(atomFeedFilePath) + "\\");
            Directory.CreateDirectory(rootDirectory.AbsolutePath);

            var imageRoot = Path.Combine(rootDirectory.LocalPath, "VSIXData");
            Directory.CreateDirectory(imageRoot);

            var feed = CreateNewFeed();
            AddPackages(feed, rootDirectory, imageRoot, packages);
            AtomFeedHelper.WriteAtomFeed(atomFeedFilePath, feed);

            return feed;
        }

        private SyndicationFeed CreateNewFeed()
        {
            // Create a new feed and set the GUID
            return new SyndicationFeed
            {
                Id = (_configGallery.Guid ?? Guid.NewGuid().ToString("D")).ToUpper(),
                Title = new TextSyndicationContent(_configGallery.Title),
                Generator = "VsixPrivateGallery (https://github.com/sverrirs/vsgallery)",
                Description = new TextSyndicationContent(_configGallery.Description, TextSyndicationContentKind.Plaintext),
                LastUpdatedTime = DateTimeOffset.Now
            };
        }

        private void AddPackages(SyndicationFeed feed,
                                            Uri root,
                                            string rootStoragePath,
                                            IEnumerable<IVsixPackage> packages)
        {
            // See https://msdn.microsoft.com/en-us/library/hh266717.aspx
            var items = new List<SyndicationItem>();

            var orderedPackages = packages
                .OrderBy(pkg => pkg.DisplayName)
                .ThenBy(pkg => pkg.Id)
                .ToArray();

            OnBackgroundProgress(new VsixFeedBuilderProgressArgs(0, orderedPackages.Length, "Starting"));

            for (var pkgIdx = 0; pkgIdx < orderedPackages.Length; pkgIdx++)
            {
                var pkg = orderedPackages[pkgIdx];

                // Possibly normalize the file name for the package, 
                // visual studio does not like special characters in the file download urls so it is best to clear these out
                pkg.File = FileCleaner.NormalizePackageFileName(pkg.File);
                var pkgFileName = Path.GetFileName(pkg.File);

                OnBackgroundProgress(new VsixFeedBuilderProgressArgs(pkgIdx + 1, orderedPackages.Length, pkgFileName));

                // Schema: https://msdn.microsoft.com/en-us/library/dd393754(v=vs.100).aspx
                // Extension Schema: https://msdn.microsoft.com/en-us/library/dd393700(v=vs.100).aspx
                var item = new SyndicationItem
                {
                    Id = pkg.Id,
                    Title = new TextSyndicationContent(pkg.DisplayName),
                    Summary = new TextSyndicationContent(pkg.Description),
                    PublishDate = new DateTimeOffset(File.GetLastWriteTimeUtc(pkg.File)),
                    LastUpdatedTime = new DateTimeOffset(File.GetLastWriteTimeUtc(pkg.File))
                };

                // Create the subfolder for the package contents on the local system, ensure that the path exists
                var packageFolderPath = Path.Combine(rootStoragePath, pkg.Id);
                Directory.CreateDirectory(packageFolderPath);

                // Create a mapping between the vsix id and the file name by placing a simple file in the root
                FileCounter.SetIdToVsixFile(pkg.Id, pkgFileName, _configStorage);

                item.Authors.Add(new SyndicationPerson { Name = pkg.Publisher });

                // If the configuration specifies that the downloads should be tracked then route the downloads
                // through the DownloadModule for tracking, if tracking is off then serve the static files directly 
                // from the file-system
                item.Content = SyndicationContent.CreateUrlContent(_configGallery.TrackDownloads 
                                                                   ? new Uri($"../api/download/{pkg.Id}/{pkgFileName}", UriKind.Relative) 
                                                                   : root.MakeRelativeUri(new Uri(pkg.File)), "application/octet-stream");

                // Only use the first category found, the developer should put the most important category first anyways
                var category = pkg.Categories.FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
                if( category != null )
                    item.Categories.Add(new SyndicationCategory(category.Trim().ToTitleCase()));

                if (pkg.TrySaveIcon(packageFolderPath, out Uri iconUri))
                    item.Links.Add(new SyndicationLink(root.MakeRelativeUri(iconUri), "icon", "", "", 0));

                if (pkg.TrySavePreviewImage(packageFolderPath, out Uri previewUri))
                    item.Links.Add(new SyndicationLink(root.MakeRelativeUri(previewUri), "previewimage", "", "", 0));

                // Displays the "More Info" link if set
                if (pkg.MoreInfoUrl.IsUrl())
                    item.Links.Add(new SyndicationLink(new Uri(pkg.MoreInfoUrl), "alternate", "", "", 0)); // Must be set to "alternate", rest does not need to be set

                // Displays the "Release Notes" link if set
                if (pkg.ReleaseNotes.IsUrl())
                    item.Links.Add(new SyndicationLink(new Uri(pkg.ReleaseNotes), "releasenotes", "", "", 0));

                // Add the extensions to the item
                AddExtensions(item, pkg);

                // Now save the item 
                items.Add(item);
            }
            OnBackgroundProgress(new VsixFeedBuilderProgressArgs(orderedPackages.Length, orderedPackages.Length, "All done"));

            // Save all the items into the feed object
            feed.Items = items;
        }

        private void AddExtensions(SyndicationItem item, IVsixPackage pkg)
        {
            var ns = XNamespace.Get("http://schemas.microsoft.com/developer/vsx-syndication-schema/2010");
            var xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");

            // If enabled then fetch the download count for the file, otherwise don't specify it
            var dlCountElement = new XElement(ns + "DownloadCount", _configGallery.TrackDownloads 
                                    ? (object) FileCounter.GetDownloadCount(pkg.Id, _configStorage) 
                                    : new XAttribute(xsi + "nil", "true"));

            // Default is not set
            var ratingElement = new XElement(ns + "Rating", new XAttribute(xsi + "nil", "true"));
            var ratingCountElement = new XElement(ns + "RatingCount", new XAttribute(xsi + "nil", "true"));

            // If ratings are enabled then attempt to resolve them
            if (_configGallery.TrackRatings)
            {
                // Only apply ratings if there are any
                var rating = FileCounter.GetRating(pkg.Id, _configStorage);
                if (rating != null && rating.Item2 > 0)
                {
                    ratingElement = new XElement(ns + "Rating", rating.Item1);
                    ratingCountElement = new XElement(ns + "RatingCount", rating.Item2);
                }
            }
            
            var content = new XElement( ns + "Vsix",
                new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                new XElement(ns + "Id", pkg.Id),
                new XElement(ns + "Version", pkg.Version),
                new XElement(ns + "References"),
                ratingElement,
                ratingCountElement,
                dlCountElement);

            using (var stringReader = new StringReader(content.ToString()))
            using (var reader = XmlReader.Create(stringReader))
            {
                item.ElementExtensions.Add(reader);
            }
        }
    }
}