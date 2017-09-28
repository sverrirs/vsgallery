using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using VsGallery.Core;
using VsGallery.Core.VsixFeed;


namespace VsGallery.WebService
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private VsixStorageWatcher _vsixStorageWatcher;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var config = Configuration.Instance;
            Initialize(config.Storage);
            _vsixStorageWatcher = new VsixStorageWatcher(config.Storage, config.Gallery);
            _vsixStorageWatcher.Start();
        }

        private void Initialize(IStorageConfiguration storageConfig)
        {
            // Ensure that directories exist
            if (!Directory.Exists(storageConfig.VsixStorageDirectory))
            {
                Directory.CreateDirectory(storageConfig.VsixStorageDirectory);
            }

            if (!Directory.Exists(storageConfig.UploadDirectory))
            {
                Directory.CreateDirectory(storageConfig.UploadDirectory);
            }
        }

        protected void Application_End()
        {
            _vsixStorageWatcher.Stop();
        }
    }
}
