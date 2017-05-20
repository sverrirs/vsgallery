using Nancy;

namespace vsgallery.Webserver.Modules
{
    public class UploadModule : NancyModule
    {
        private readonly IConfiguration _configuration;

        public UploadModule(IConfiguration configuration)
        {
            _configuration = configuration;

            Post["/upload"] = HandleExtensionUploadRequest;
            Put["/upload"] = HandleExtensionUploadRequest;
        }

        private Response HandleExtensionUploadRequest(dynamic parameters)
        {
            // TODO: Parse the submitted file attachment and write to the vsix folder!
            return new Response {StatusCode = HttpStatusCode.NotImplemented};
        }
    }
}
