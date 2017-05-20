using System;
using System.IO;
using Nancy;
using Nancy.Responses;
using vsgallery.FileHelpers;

namespace vsgallery.Webserver.Modules
{
    public class DownloadModule: NancyModule
    {
        private readonly IConfiguration _configuration;
        public DownloadModule(IConfiguration configuration)
        {
            _configuration = configuration;

            // Download route for the vsix files
            Get["download/{id}/{vsix}"] = HandleDownloadRequest;
        }

        private Response HandleDownloadRequest(dynamic parameters)
        {
            string vsixName = parameters.vsix;
            string vsixId = parameters.id;
            string vsixFilePath = Path.Combine(Environment.CurrentDirectory, _configuration.Storage.VsixStorageDirectory, vsixName);

            // Count the download
            bool refreshFile = FileCounter.SetDownloadCount(vsixId, vsixName, _configuration);

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
