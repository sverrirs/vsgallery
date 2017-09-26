using System;
using System.IO;
using Nancy;
using Nancy.Helpers;
using Nancy.Responses;
using vsgallery.FileHelpers;

namespace vsgallery.Webserver.ApiModules
{
    public class DownloadModule: NancyModule
    {
        private readonly IStorageConfiguration _configuration;
        public DownloadModule(IStorageConfiguration configuration)
        {
            _configuration = configuration;

            // Download route for the vsix files
            Get["/api/download/{id}/{vsix}"] = HandleDownloadRequest;
        }

        private Response HandleDownloadRequest(dynamic parameters)
        {
            string vsixName = HttpUtility.UrlDecode(parameters.vsix);
            string vsixId = parameters.id;
            string vsixFilePath = Path.Combine(Environment.CurrentDirectory, _configuration.VsixStorageDirectory, vsixName);

            // Count the download
            bool refreshFile = FileCounter.SetDownloadCount(vsixId, _configuration);

            // Serve the requested VSIX file back to the caller
            Response fileResponse = null;
            var vsixFile = new FileStream(vsixFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            using (var response = new StreamResponse(() => vsixFile, "application/octet-stream"))
            {
                fileResponse = response.AsAttachment(vsixName);
            }

            // Re-process the file
            if (refreshFile && File.Exists(vsixFilePath))
            {
                // Schedule the ATOM feed to be re-created by touching the VSIX file in question
                File.SetLastWriteTime(vsixFilePath, DateTime.Now);
            }

            return fileResponse;
        }
    }
}
