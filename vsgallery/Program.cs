using System;
using System.IO;
using Topshelf;
using vsgallery.Webserver;

namespace vsgallery
{
    class Program
    {
        static int Main(string[] args)
        {
            // Ensure global single instance

            // Load the ini configuration file
            IConfiguration config = new Configuration(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini"));

            HostFactory.Run(x =>
            {
                x.Service<SelfHost>(s =>
                {
                    s.ConstructUsing(name => new SelfHost(config));
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                x.RunAsLocalSystem();
                x.SetDescription(config.Install.Description ??
                    "Self-Hosted Private Gallery for Visual Studio VSIX files");
                x.SetDisplayName(config.Install.DisplayName ?? 
                    "Private Gallery Self-Host Service");
                x.SetServiceName(config.Install.ServiceName ?? 
                    "Private-Gallery-SelfHost");
            });

            return 0;
        }

    }
}
