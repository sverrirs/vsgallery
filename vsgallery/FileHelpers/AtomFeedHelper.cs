using System;
using System.IO;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;

namespace vsgallery.FileHelpers
{
    public static class AtomFeedHelper
    {
        private static readonly ReaderWriterLockSlim atomFeedLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

        public static SyndicationFeed LoadAtomFeed(string atomFeedFilePath)
        {
            // Wait until the feed file has been released by other processes before attempting to read
            atomFeedLock.EnterReadLock();
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
            finally
            {
                atomFeedLock.ExitReadLock();
            }
        }

        public static void WriteAtomFeed(string outFilePath, SyndicationFeed feed)
        {
            // Gain exclusive write lock for the atom file
            atomFeedLock.EnterWriteLock();
            try
            {
                var sb = new StringBuilder();
                using (var stringStream = new StringWriter(sb))
                {
                    using (var writer = XmlWriter.Create(stringStream))
                    {
                        var formatter = new Atom10FeedFormatter(feed);
                        formatter.WriteTo(writer);
                    }
                }

                File.WriteAllText(outFilePath, XElement.Parse(sb.ToString()).ToString());
            } finally {
                atomFeedLock.ExitWriteLock();
            }
        }
    }
}
