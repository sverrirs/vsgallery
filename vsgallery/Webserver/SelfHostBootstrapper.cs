using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.Diagnostics;
using Nancy.TinyIoc;

namespace VISXPrivateGallery.Webserver
{
    public class SelfHostBootstrapper : DefaultNancyBootstrapper
    {
        private readonly IConfiguration _configuration;
        private TinyIoCContainer _container;

        public SelfHostBootstrapper(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        protected override void ConfigureApplicationContainer(TinyIoCContainer container)
        {
            _container = container;

            base.ConfigureApplicationContainer(container);

            container.Register<IConfiguration>(_configuration).WithStrongReference();
        }

        protected override DiagnosticsConfiguration DiagnosticsConfiguration
        {
            get
            {
                // If diagnostics are enabled then use them
                if (_configuration.Hosting.Diagnostics)
                    return new DiagnosticsConfiguration {Password = @"pw"};

                // By default use the base configuration
                return base.DiagnosticsConfiguration;
            }
        }

        protected override void ConfigureConventions(NancyConventions conventions)
        {
            // Resolve the storage path from the IoC container
            conventions.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("feeds", _configuration.Storage.VsixStorageDirectory )
            );

            base.ConfigureConventions(conventions);
        }
    }
}
