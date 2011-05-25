using System.Runtime.Serialization;

namespace Torshify.Server.Contracts
{
    [DataContract]
    public class SearchResult
    {
        [DataMember]
        public string Query { get; set; }

        [DataMember]
        public string DidYouMean { get; set; }

        [DataMember]
        public Artist[] Artists { get; set; }

        [DataMember]
        public Album[] Albums { get; set; }

        [DataMember]
        public Track[] Tracks { get; set; }

        [DataMember]
        public int TotalArtists { get; set; }

        [DataMember]
        public int TotalAlbums { get; set; }

        [DataMember]
        public int TotalTracks { get; set; }
    }
}