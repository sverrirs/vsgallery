using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

using VsGallery.Core;
using VsGallery.Core.FileHelpers;
using VsGallery.WebService.Http;

namespace VsGallery.WebService.Controllers
{
    public class DownloadController : ApiController
    {
        private IStorageConfiguration _configuration;

        public DownloadController()
        {
            _configuration = WebService.Configuration.Instance.Storage;
        }

        /// <summary>
        /// Retrieves a particular VSIX package by its VSIX ID and the VSIX Name.
        /// </summary>
        /// <param name="vsixId"></param>
        /// <param name="vsixName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/download/{vsixId}/{vsixName}")]
        public IHttpActionResult Get(string vsixId, string vsixName)
        {
            string vsixFilePath = Path.Combine(_configuration.VsixStorageDirectory, vsixName);

            // Count the download
            bool refreshFile = FileCounter.SetDownloadCount(vsixId, _configuration);

            // Re-process the file
            if (refreshFile && File.Exists(vsixFilePath))
            {
                // Schedule the ATOM feed to be re-created by touching the VSIX file in question
                File.SetLastWriteTime(vsixFilePath, DateTime.Now);
            }

            return new FileResult(vsixFilePath);
        }
    }
}