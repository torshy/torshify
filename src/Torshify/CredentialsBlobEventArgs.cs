using System;

namespace Torshify
{
    public class CredentialsBlobEventArgs : EventArgs
    {
        public CredentialsBlobEventArgs(string blob)
        {
            Blob = blob;
        }

        public string Blob
        {
            get;
            private set;
        }
    }
}