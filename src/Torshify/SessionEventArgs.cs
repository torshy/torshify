using System;

namespace Torshify
{
    public class SessionEventArgs : EventArgs
    {
        public SessionEventArgs(Error status)
        {
            Status = status;
        }

        public SessionEventArgs(string message)
        {
            Message = message;
        }

        public SessionEventArgs(Error status, string message)
        {
            Status = status;
            Message = message;
        }

        public Error Status
        {
            get;
            private set;
        }

        public string Message
        {
            get; 
            private set;
        }
    }
}