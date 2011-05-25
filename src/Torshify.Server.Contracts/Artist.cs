using System.Runtime.Serialization;

namespace Torshify.Server.Contracts
{
    [DataContract]
    public class Artist : TorshifyItem
    {
        [DataMember]
        public string Name { get; set; }
    }
}