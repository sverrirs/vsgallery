using System.IO;
using System.ServiceModel.Syndication;
using System.Xml;

namespace vsgallery.FileHelpers
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
