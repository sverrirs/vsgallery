using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nancy;

namespace vsgallery.Webserver.ApiModules
{
    public class UploadModule : NancyModule
    {
        private readonly IConfiguration _configuration;

        public UploadModule(IConfiguration configuration)
        {
            _configuration = configuration;

            Post["/api/upload", true] = HandleExtensionUploadRequestAsync;
            Put["/api/upload", true] = HandleExtensionUploadRequestAsync;
        }

        private async Task<dynamic> HandleExtensionUploadRequestAsync(dynamic parameters, CancellationToken cancelToken)
        {
            // Resolve the paths needed
            var vsixStorageDirPath = Path.Combine(Environment.CurrentDirectory, _configuration.Storage.VsixStorageDirectory);
            var vsixUploadDirPath = Path.Combine(Environment.CurrentDirectory, _configuration.Storage.UploadDirectory);

            // If needed we create the directory paths 
            Directory.CreateDirectory(vsixStorageDirPath);
            Directory.CreateDirectory(vsixUploadDirPath);

            // Create a parallel parent task that manages all the sub tasks 
            // For every file that was sent in we save it to our download folder
            var allTasks = Request.Files.Select(rFile => SaveUploadFileAsync(vsixUploadDirPath, rFile)).ToList();

            // Wait for all to finish
            await Task.WhenAll(allTasks);

            // If any tasks are faulted then error out
            if (allTasks.Any(x => x.Status != TaskStatus.RanToCompletion))
                return new Response() {StatusCode = HttpStatusCode.InternalServerError};

            // So everything went ok
            // then move all of the uploaded files into the main VSIX folder
            foreach (Task<string> saveTask in allTasks)
            {
                // Now move all the files uploaded to the main storage directory
                File.Move(saveTask.Result, Path.Combine(vsixStorageDirPath, Path.GetFileName(saveTask.Result)));
            }

            // When everything is done, simply return OK
            return new Response { StatusCode = HttpStatusCode.OK };
        }

        private async Task<string> SaveUploadFileAsync(string vsixUploadDirPath, HttpFile rFile)
        {
            var filePath = Path.Combine(vsixUploadDirPath, rFile.Name);
            using (FileStream destStream = File.Create(filePath))
            {
                await rFile.Value.CopyToAsync(destStream);
            }

            return filePath;
        }
    }
}
