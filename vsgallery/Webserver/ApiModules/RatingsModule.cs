using System;
using System.IO;
using Nancy;
using Nancy.Extensions;
using vsgallery.FileHelpers;

namespace vsgallery.Webserver.ApiModules
{
    public class RatingsModule : NancyModule
    {
        private readonly IStorageConfiguration _configuration;

        public RatingsModule(IStorageConfiguration configuration)
        {
            _configuration = configuration;

            Get["/api/ratings/{id}"] = HandleGetRatingsRequest;
            Post["/api/ratings/{id}"] = HandleSetRatingsRequest;
            Put["/api/ratings/{id}"] = HandleSetRatingsRequest;
        }

        private Response HandleGetRatingsRequest(dynamic parameters)
        {
            string vsixId = parameters.id;
            var rating = FileCounter.GetRating(vsixId, _configuration);
            return Response.AsText("{\"rating\": " + rating.Item1 + ", \"count\": " + rating.Item2 + "}", "application/json");
        }

        private Response HandleSetRatingsRequest(dynamic parameters)
        {
            string vsixId = parameters.id;
            float rawRating;
            if (!float.TryParse(Request.Body.AsString(), out rawRating))
                return new Response {StatusCode = HttpStatusCode.BadRequest};

            // Upperbound the rating
            float rating = Math.Min(5, Math.Max(0, rawRating));

            // Record the rating
            if( !FileCounter.SetRating(vsixId, rating, _configuration) )
                return new Response { StatusCode = HttpStatusCode.InternalServerError };

            // Trigger a re-creation of the ATOM feed by touching the main file if possible
            var vsixFilePath = FileCounter.GetVsixFileFromId(vsixId, _configuration);
            if(vsixFilePath != null && File.Exists(vsixFilePath))
            {
                File.SetLastWriteTime(vsixFilePath, DateTime.Now);
            }

            // All ok
            return new Response { StatusCode = HttpStatusCode.OK };
        }
    }
}
