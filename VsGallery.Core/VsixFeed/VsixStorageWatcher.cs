using System;
using System.IO;


namespace VsGallery.Core.VsixFeed
{
    /// <summary>
    /// Responsible for watching the VSIX storage directory for changes and re-generating the feed file 
    /// if files are added or deleted
    /// </summary>
    public class VsixStorageWatcher
    {
        private readonly string _vsixDirectory;
        private readonly VsixFeedBuilder _feedBuilder;
        private FileSystemWatcher _fsWatcher;
        private readonly object _lock = new object();
        private bool _isRunning = false;

        public VsixStorageWatcher(IStorageConfiguration storageConfig, IGalleryConfiguration galleryConfig)
        {
            _vsixDirectory = Path.Combine(Environment.CurrentDirectory, storageConfig.VsixStorageDirectory);
            _feedBuilder = new VsixFeedBuilder(storageConfig, galleryConfig);
            _feedBuilder.BackgroundProgress += FeedBuilderOnBackgroundProgress;
        }

        private void FeedBuilderOnBackgroundProgress(object sender, VsixFeedBuilderProgressArgs e)
        {
            Console.WriteLine($"Reading VSIX : {e.Filename} : {e.CurrentStep} of {e.TotalSteps}");
        }

        public void Start()
        {
            _fsWatcher = new FileSystemWatcher
            {
                Path = _vsixDirectory,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
                Filter = "*.vsix"
            };
            _fsWatcher.Created += OnDirectoryChanged;
            _fsWatcher.Deleted += OnDirectoryChanged;
            _fsWatcher.Changed += OnDirectoryChanged;
            _fsWatcher.Renamed += OnDirectoryChanged;
            _fsWatcher.EnableRaisingEvents = true;
            RunFeedBuilder();
        }

        private void OnDirectoryChanged(object sender, FileSystemEventArgs e)
        {
            if (e.Name == null || !e.Name.EndsWith(".vsix"))
                return;

            RunFeedBuilder();
        }

        private void RunFeedBuilder()
        {
            lock (_lock)
            {
                if (_isRunning)
                    return;

                _isRunning = true;
                _fsWatcher.EnableRaisingEvents = false;

                // Run once
                _feedBuilder.RunAsync(_vsixDirectory).ContinueWith(task =>
                {
                    lock (_lock)
                    {
                        _isRunning = false;
                        _fsWatcher.EnableRaisingEvents = true;
                    }
                });
            }
        }

        public void Stop()
        {
            _fsWatcher.EnableRaisingEvents = false;
            _fsWatcher.Changed -= OnDirectoryChanged;
            _fsWatcher.Dispose();
            _fsWatcher = null;
        }
    }
}
