using System;
using System.Runtime.Serialization;

namespace Torshify.Server.Contracts
{
    [DataContract]
    public class Track : TorshifyItem
    {
        [DataMember]
        public Album Album { get; set; }
        [DataMember]
        public Artist[] Artists { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int Index { get; set; }
        [DataMember]
        public int Popularity { get; set; }
        [DataMember]
        public int Disc { get; set; }
        [DataMember]
        public TimeSpan Duration { get; set; }
        [DataMember]
        public bool IsAvailable { get; set; }
        [DataMember]
        public bool IsStarred { get; set; }
    }

    [DataContract]
    public class PlaylistTrack : Track
    {
        [DataMember]
        public int PlaylistID { get; set; }
        [DataMember]
        public DateTime CreateTime { get; set; }
        [DataMember]
        public bool Seen { get; set; }
    }
}