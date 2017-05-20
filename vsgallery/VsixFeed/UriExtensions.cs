using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VISXPrivateGallery.VsixFeed
{
    public static class UriExtensions
    {
        public static bool IsUrl(this string uriCandidate)
        {
            return Uri.TryCreate(uriCandidate, UriKind.Absolute, out Uri uriResult) &&
                    (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }
    }
}
