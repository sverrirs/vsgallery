using System;
using System.Collections.Generic;


namespace VsGallery.Core.VsixFeed
{
    public sealed class VersionComparer : IComparer<string>
    {
        public int Compare(string left, string right)
        {
            var lVer = _TryParse(left);
            var rVer = _TryParse(right);

            if (lVer == null && rVer == null)
            {
                return string.CompareOrdinal(left, right);
            }

            if (lVer == null)
            {
                return -1;
            }

            if (rVer == null)
            {
                return 1;
            }

            return lVer.CompareTo(rVer);
        }

        private static Version _TryParse(string value)
        {
            Version version;
            return Version.TryParse(value, out version) ? version : null;
        }
    }
}
