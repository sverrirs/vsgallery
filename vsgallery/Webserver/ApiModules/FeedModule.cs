using System;
using System.IO;
using Nancy;
using Newtonsoft.Json;
using vsgallery.FileHelpers;

namespace vsgallery.Webserver.ApiModules
{
    public class FeedModule : NancyModule
    {
        private readonly IConfiguration _configuration;

        public FeedModule(IConfiguration configuration)
        {
            _configuration = configuration;

            Get["/api/json"] = HandleJsonFeedRequest;
        }

        private Response HandleJsonFeedRequest(dynamic parameters)
        {
            var atomFilePath = Path.Combine(Environment.CurrentDirectory, _configuration.Storage.VsixStorageDirectory, "atom.xml");
            var feed = AtomFeedHelper.LoadAtomFeed(atomFilePath);

            // Return the feed or no content error if nothing is found
            return feed == null 
                ? new Response {StatusCode = HttpStatusCode.NoContent} 
                : Response.AsText(JsonConvert.SerializeObject(feed), "application/json");
        }
    }
}
