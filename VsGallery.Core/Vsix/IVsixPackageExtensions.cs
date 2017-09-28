using System.Collections.Generic;
using System.Linq;

using VsGallery.Core.VsixFeed;


namespace VsGallery.Core.Vsix
{
  public static class IVsixPackageExtensions
  {
    public static IEnumerable<IVsixPackage> OnlyMostRecentVersions(this IEnumerable<IVsixPackage> packages)
    {
      var groups = packages.GroupBy(pkg => pkg.Id);
      var versionComparer = new VersionComparer();
      foreach (var grp in groups)
      {
        if (grp.Count() > 1)
          yield return grp.OrderByDescending(pkg => pkg.Version, versionComparer).First();
        else
          yield return grp.First();
      }
    }
  }
}