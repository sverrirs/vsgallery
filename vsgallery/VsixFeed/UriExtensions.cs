using System;

namespace vsgallery.VsixFeed
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
