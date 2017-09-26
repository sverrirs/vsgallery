using System;
using System.IO;
using Nancy.Hosting.Self;
using vsgallery.VsixFeed;

namespace vsgallery.Webserver
{
    public class SelfHost
    {
        private NancyHost _nancyHost;
        private readonly IConfiguration _config;
        private string _vsixStoragePath;
        private VsixStorageWatcher _vsixStorageWatcher;
        private readonly SelfHostBootstrapper _bootstrapper;

        public SelfHost(IConfiguration configuration)
        {
            _config = configuration;

            // Create the nancy bootstrapper manually
            _bootstrapper = new SelfHostBootstrapper(configuration);            
        }

        public void Start()
        {
            Nancy.Hosting.Self.HostConfiguration nancyConfig = new Nancy.Hosting.Self.HostConfiguration()
            {
                UrlReservations = new UrlReservations() { CreateAutomatically = true}
            };

            _nancyHost = new NancyHost(_bootstrapper, nancyConfig, new Uri(
                $"http{(_config.Hosting.UseSSL ? "s" : "")}://{_config.Hosting.HostName}:{_config.Hosting.Port}"));

            // Create the VSIX feed
            _vsixStoragePath = Path.Combine(Environment.CurrentDirectory, _config.Storage.VsixStorageDirectory);
            _vsixStorageWatcher = new VsixStorageWatcher(_vsixStoragePath, _config.Storage, _config.Gallery);
            _vsixStorageWatcher.Start();
            _nancyHost.Start();
        }

        public void Stop()
        {
            _vsixStorageWatcher.Stop();
            _nancyHost.Stop();
        }

    }
}
