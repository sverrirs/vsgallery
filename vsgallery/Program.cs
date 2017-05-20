using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;
using Topshelf;
using VISXPrivateGallery.Webserver;

namespace VISXPrivateGallery
{
    class Program
    {
        static int Main(string[] args)
        {
            // Ensure global single instance

            // Load the ini configuration file
            IConfiguration config = new Configuration("config.ini");

            HostFactory.Run(x =>
            {
                x.Service<SelfHost>(s =>
                {
                    s.ConstructUsing(name => new SelfHost(config));
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                x.RunAsLocalSystem();
                x.SetDescription("Self-Hosted Private Gallery for Visual Studio VISX files");
                x.SetDisplayName("Private Gallery Self-Host Service");
                x.SetServiceName("Private-Gallery-SelfHost");
            });

            return 0;
        }

    }
}
