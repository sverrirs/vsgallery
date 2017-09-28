using System.Runtime.Serialization;

namespace VsGallery.WebService.Models
{
    [DataContract]
    public class PackagePopularity
    {
        [DataMember(Name = "rating")]
        public float Rating { get; set; }
        [DataMember(Name = "count")]
        public int Count { get; set; }
    }
}
