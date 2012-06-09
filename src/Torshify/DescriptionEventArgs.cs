using System;

namespace Torshify
{
    public class DescriptionEventArgs : EventArgs
    {
        #region Constructors

        public DescriptionEventArgs(string description)
        {
            Description = description;
        }

        #endregion Constructors

        #region Properties

        public string Description
        {
            get; private set;
        }

        #endregion Properties
    }
}