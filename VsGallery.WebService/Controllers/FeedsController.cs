using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Syndication;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using System.Xml;
using System.Xml.Linq;

using VsGallery.Core;
using VsGallery.Core.FileHelpers;
using VsGallery.WebService.Http;

namespace VsGallery.WebService.Controllers
{

    public class FeedsController : ApiController
    {
        private IStorageConfiguration _configuration;

        public FeedsController()
        {
            _configuration = VsGallery.WebService.Configuration.Instance.Storage;
        }

        /// <summary>
        /// JSON feed for the entire package catalog. Same data that is being fed through the atom feed but just in a handier JSON format.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/json")]
        public IHttpActionResult GetAsJson()
        {
            var feed = LoadFeed();
            return Ok(JsonConvert.SerializeObject(feed));
        }

        /// <summary>
        /// return additional feed vsix metadata, which is inside a feed description like e.g. preview image
        /// </summary>
        /// <param name="vsixId"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("feeds/VSIXData/{vsixId}/{*file}")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IHttpActionResult GetVsixData(string vsixId, string file)
        {
            var filePath = Path.Combine(_configuration.VsixStorageDirectory, "VSIXData", vsixId, file);
            return new FileResult(filePath);
        }

        /// <summary>
        /// This is the main entry point for the VSIX feed and serves up the Syndicate-Feed compatible Atom file 
        /// containing all available extensions on the server.
        /// This is the URL endpoint that should be used in Visual Studio.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("feeds/atom.xml")]
        [Route("feeds/atom", Name = "AtomFeed")]
        public IHttpActionResult GetAtomFeed()
        {
            return FormatFeedAs("application/atom+xml", feed => new Atom10FeedFormatter(feed));
        }

        /// <summary>
        /// The same feed just as RSS 2.0 Feed
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("feeds/rss.xml")]
        [Route("feeds/rss", Name = "RssFeed")]
        public IHttpActionResult GetRssFeed()
        {
            return FormatFeedAs("application/rss+xml", feed => new Rss20FeedFormatter(feed));
        }

        private SyndicationFeed LoadFeed()
        {
            var atomFilePath = Path.Combine(_configuration.VsixStorageDirectory, "atom.xml");
            var feed = AtomFeedHelper.LoadAtomFeed(atomFilePath);
            return feed;
        }

        private IHttpActionResult FormatFeedAs<T>(string mediaType, Func<SyndicationFeed, T> createFeedFormatter) where T : SyndicationFeedFormatter
        {
            var feed = LoadFeed();
            var formatter = createFeedFormatter(feed);

            var xml = ConvertToXml(formatter);

            //forcing to send back response in Xml format
            var resp = new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(xml, Encoding.UTF8, mediaType) };

            return ResponseMessage(resp);
        }

        private string ConvertToXml(SyndicationFeedFormatter feedFormatter)
        {
            string xmlOutput;
            var xmlWriterSettings = new XmlWriterSettings { Encoding = Encoding.UTF8,  };

            using (var memoryStream = new MemoryStream())
            {
                using (var xmlWriter = XmlWriter.Create(memoryStream, xmlWriterSettings))
                {
                    feedFormatter.WriteTo(xmlWriter);
                    xmlWriter.Flush();
                }
                using (var streamReader = new StreamReader(memoryStream))
                {
                    memoryStream.Position = 0;
                    xmlOutput = streamReader.ReadToEnd();
                }
            }

            return xmlOutput;
        }
    }
}
