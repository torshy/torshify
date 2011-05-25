using System.Runtime.Serialization;

namespace Torshify.Server.Contracts
{
    [DataContract]
    public class Album : TorshifyItem
    {
        [DataMember]
        public Artist Artist { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public bool IsAvailable { get; set; }
        [DataMember]
        public int Year { get; set; }
        [DataMember]
        public string CoverId { get; set; }
    }
}