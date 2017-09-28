using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using VsGallery.Core;
using VsGallery.Core.FileHelpers;
using VsGallery.WebService.Models;

namespace VsGallery.WebService.Controllers
{

    public class RatingsController : ApiController
    {
        private IStorageConfiguration _configuration;

        public RatingsController()
        {
            _configuration = WebService.Configuration.Instance.Storage;
        }

        /// <summary>
        /// Retrieves the rating value and vote count for a particular VSIX package by its VSIX ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetVsixRating(string id)
        {
            var rating = FileCounter.GetRating(id, _configuration);

            var popularity = new PackagePopularity() { Rating = rating.Item1, Count = rating.Item2 };
            return Ok(popularity);
        }


        /// <summary>
        /// Submitting rating values for a particular VSIX package by its VSIX ID. 
        /// The post payload should be just raw string and contain a single floating point value in the range between [0.0, 5.0].
        /// </summary>
        /// <param name="vsixId"></param>
        /// <param name="ratingString"></param>
        /// <returns></returns>
        [HttpPut]
        [HttpPost]
        public IHttpActionResult UpdateVsixRating(string vsixId, [FromBody] string ratingString)
        {
            if (!float.TryParse(ratingString, out float rawRating))
            {
                return BadRequest();
            }

            // Upperbound the rating
            float rating = Math.Min(5, Math.Max(0, rawRating));

            // Record the rating
            if (!FileCounter.SetRating(vsixId, rating, _configuration))
            {
                return InternalServerError();
            }

            // Trigger a re-creation of the ATOM feed by touching the main file if possible
            var vsixFilePath = FileCounter.GetVsixFileFromId(vsixId, _configuration);
            if (vsixFilePath != null && File.Exists(vsixFilePath))
            {
                File.SetLastWriteTime(vsixFilePath, DateTime.Now);
            }

            // All ok
            return Ok();
        }
    }
}
