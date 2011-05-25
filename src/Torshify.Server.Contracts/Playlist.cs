using System.Runtime.Serialization;

namespace Torshify.Server.Contracts
{
    [DataContract]
    public class Playlist : TorshifyItem
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public Track[] Tracks { get; set; }
        [DataMember]
        public bool PendingChanges { get; set; }
        [DataMember]
        public bool IsCollaborative { get; set; }
        [DataMember]
        public string ImageId { get; set; }
    }
}