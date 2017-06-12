using System;
using System.IO;
using System.Threading;

namespace vsgallery.FileHelpers
{
    public static class FileCounter
    {
        private static readonly ReaderWriterLockSlim vsixInfoFileLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private static readonly ReaderWriterLockSlim downloadCountFileLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private static readonly ReaderWriterLockSlim ratingsFileLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);


        public static bool SetIdToVsixFile(string vsixId, string vsixFile, IConfiguration configuration)
        {
            vsixInfoFileLock.EnterWriteLock();
            // Attempt to find the file, if exists then read the only file in it into memory
            try
            {
                string vsixMarkPath = Path.Combine(Environment.CurrentDirectory, configuration.Storage.VsixStorageDirectory, "VSIXData", vsixId, "info.txt");

                // Increment download count and write to the file again
                File.WriteAllText(vsixMarkPath, vsixFile);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // Swallow exception as counting the downloads is not critical, 
                // main priority is to serve the file to the user!!!
                return false;
            }
            finally
            {
                vsixInfoFileLock.ExitWriteLock();
            }
        }

        public static string GetVsixFileFromId(string vsixId, IConfiguration configuration)
        {
            vsixInfoFileLock.EnterReadLock();
            try
            {
                string vsixMarkPath = Path.Combine(Environment.CurrentDirectory, configuration.Storage.VsixStorageDirectory, "VSIXData", vsixId, "info.txt");

                if (!File.Exists(vsixMarkPath))
                    return null;

                var fileName = File.ReadAllText(vsixMarkPath);
                return Path.Combine(Environment.CurrentDirectory, configuration.Storage.VsixStorageDirectory, fileName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
            finally
            {
                vsixInfoFileLock.ExitReadLock();
            }
        }

        public static bool SetDownloadCount(string vsixId, string vsixFile, IConfiguration configuration)
        {
            downloadCountFileLock.EnterWriteLock();
            // Attempt to find the file, if exists then read the only file in it into memory
            try
            {
                string vsixDownloadCountPath = Path.Combine(Environment.CurrentDirectory, configuration.Storage.VsixStorageDirectory, "VSIXData", vsixId, "downloads.txt");

                int downloadCount = CoreGetDownloadCount(vsixId, vsixFile, configuration);

                // Increment download count and write to the file again
                File.WriteAllText(vsixDownloadCountPath, (++downloadCount).ToString());

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // Swallow exception as counting the downloads is not critical, 
                // main priority is to serve the file to the user!!!
                return false;
            }
            finally
            {
                downloadCountFileLock.ExitWriteLock();
            }
        }

        public static int GetDownloadCount(string vsixId, string vsixFile, IConfiguration configuration)
        {
            downloadCountFileLock.EnterReadLock();
            try
            {
                return CoreGetDownloadCount(vsixId, vsixFile, configuration);
            }
            finally
            {
                downloadCountFileLock.ExitReadLock();
            }
        }

        private static int CoreGetDownloadCount(string vsixId, string vsixFile, IConfiguration configuration)
        {
            try { 
                string vsixDownloadCountPath = Path.Combine(Environment.CurrentDirectory, configuration.Storage.VsixStorageDirectory, "VSIXData", vsixId, "downloads.txt");

                int downloadCount = 0;
                if (File.Exists(vsixDownloadCountPath))
                {
                    // Attempt to read the download count, don't care if it fails really...
                    int.TryParse(File.ReadAllText(vsixDownloadCountPath), out downloadCount);
                }
                return downloadCount;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return 0;
            }
            
        }

        public static bool SetRating(string vsixId, float rating, IConfiguration configuration)
        {
            ratingsFileLock.EnterWriteLock();
            try
            {
                string vsixDownloadRatingPath = Path.Combine(Environment.CurrentDirectory, configuration.Storage.VsixStorageDirectory, "VSIXData", vsixId, "ratings.txt");

                var ratingAndCount = CoreGetRating(vsixId, configuration);

                // Calculate a running average
                var adjRating = new Tuple<float, int>(
                    Math.Min(5, Math.Max(0, (ratingAndCount.Item1 * ratingAndCount.Item2 + rating) / (ratingAndCount.Item2 + 1))),
                    ratingAndCount.Item2 + 1);

                // Increment download count and write to the file again
                File.WriteAllText(vsixDownloadRatingPath, $"{adjRating.Item1}|{adjRating.Item2}");

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // Swallow exception as counting the downloads is not critical, 
                // main priority is to serve the file to the user!!!
                return false;
            }
            finally
            {
                ratingsFileLock.ExitWriteLock();
            }
        }

        public static Tuple<float, int> GetRating(string vsixId, IConfiguration configuration)
        {
            ratingsFileLock.EnterReadLock();
            try
            {
                return CoreGetRating(vsixId, configuration);
            }
            finally
            {
                ratingsFileLock.ExitReadLock();
            }
        }

        private static Tuple<float, int> CoreGetRating(string vsixId, IConfiguration configuration)
        {
            try
            {
                string vsixDownloadRatingPath = Path.Combine(Environment.CurrentDirectory, configuration.Storage.VsixStorageDirectory, "VSIXData", vsixId, "ratings.txt");
                if (File.Exists(vsixDownloadRatingPath))
                {
                    // Attempt to read the download count, don't care if it fails really...
                    var rawRatingsText = File.ReadAllText(vsixDownloadRatingPath).Split('|');
                    return new Tuple<float, int>(float.Parse(rawRatingsText[0]), int.Parse(rawRatingsText[1]));
                }
                return new Tuple<float, int>(0, 0);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new Tuple<float, int>(0, 0);
            }
        }
    }
}
