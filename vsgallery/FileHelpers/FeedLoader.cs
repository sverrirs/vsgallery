using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace VISXPrivateGallery.FileHelpers
{
    public static class FeedLoader
    {
        public static SyndicationFeed LoadAtomFeed(string atomFeedFilePath)
        {
            try
            {
                using (var stream = File.OpenRead(atomFeedFilePath))
                using (var reader = XmlReader.Create(stream))
                {
                    return SyndicationFeed.Load(reader);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
