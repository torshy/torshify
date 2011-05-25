using System.Runtime.Serialization;

namespace Torshify.Server.Contracts
{
    [DataContract]
    public abstract class TorshifyItem
    {
        [DataMember]
        public int ID { get; set; }
    }
}