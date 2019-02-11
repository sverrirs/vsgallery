﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using VsGallery.Core;
using VsGallery.Core.FileHelpers;

namespace VsGallery.WebService.Controllers
{
    public class UploadController : ApiController
    {
        private IStorageConfiguration _configuration;

        public UploadController()
        {
            _configuration = WebService.Configuration.Instance.Storage;
        }


        /// <summary>
        /// This endpoint accepts form-data uploads of one or more .vsix files to the hosting service.
        /// </summary>
        [HttpPost]
        [HttpPut]
        public IHttpActionResult AddOrUpdateVsix()
        {
            // Get the uploaded image from the Files collection
            var requestFiles = HttpContext.Current.Request.Files;
            var httpFiles = Enumerable.Range(0, requestFiles.Count).Select(i => requestFiles[i]).ToList();


            if (!httpFiles.Any())
            {
                return BadRequest("No Files");
            }

            // Validate the uploaded image(optional)

            // Get the complete file path
            foreach(var httpFile in httpFiles)
            {
                var fileSavePath = Path.Combine(_configuration.UploadDirectory, httpFile.FileName);

                // Save the uploaded file to "UploadedFiles" folder
                httpFile.SaveAs(fileSavePath);

                var newFileName = httpFile.FileName.Split(new[] {".vsix"}, StringSplitOptions.None)[0] +
                                  DateTime.UtcNow.ToString("yyyyMMdd").Replace("-", "");

                var iteration = Directory.EnumerateFiles(_configuration.VsixStorageDirectory)
                                    .Count(x => x.Split('\\').Last().StartsWith(newFileName,
                                        StringComparison.OrdinalIgnoreCase)) + 1;

                // Now move all the files uploaded to the main storage directory
                File.Move(fileSavePath,
                    Path.Combine(_configuration.VsixStorageDirectory, $"{newFileName}{iteration}.vsix"));
            }

            return Ok();
        }
    }
}
