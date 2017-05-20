using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Nancy;
using Nancy.Responses;
using Newtonsoft.Json;
using VISXPrivateGallery.FileHelpers;

namespace VISXPrivateGallery.Webserver.Modules
{
    public class FeedModule : NancyModule
    {
        private readonly IConfiguration _configuration;

        public FeedModule(IConfiguration configuration)
        {
            _configuration = configuration;

            Get["/json"] = HandleJsonFeedRequest;
        }

        private Response HandleJsonFeedRequest(dynamic parameters)
        {
            var atomFilePath = Path.Combine(Environment.CurrentDirectory, _configuration.Storage.VsixStorageDirectory, "atom.xml");
            var feed = FeedLoader.LoadAtomFeed(atomFilePath);

            // Return the feed or no content error if nothing is found
            return feed == null 
                ? new Response {StatusCode = HttpStatusCode.NoContent} 
                : Response.AsText(JsonConvert.SerializeObject(feed), "application/json");
        }
    }
}
