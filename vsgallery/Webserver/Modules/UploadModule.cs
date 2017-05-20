using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy;

namespace VISXPrivateGallery.Webserver.Modules
{
    public class UploadModule : NancyModule
    {
        private readonly IConfiguration _configuration;

        public UploadModule(IConfiguration configuration)
        {
            _configuration = configuration;

            Get["/upload"] = parameters =>
            {
                return Response.AsText("Upload form", "text/html");
            };

            Post["/upload/save"] = parameters =>
            {
                return Response.AsJson("ok");
            };
        }
    }
}
