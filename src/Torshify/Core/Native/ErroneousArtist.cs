using System;

namespace Torshify.Core.Native
{
    internal class ErroneousArtist : NativeObject, IArtist
    {
        #region Constructors

        public ErroneousArtist(ISession session, IntPtr handle)
            : base(session, handle)
        {
        }

        #endregion Constructors

        #region Properties

        public string Name
        {
            get { return string.Empty; }
        }

        public string PortraitId
        {
            get { return string.Empty; }
        }

        public bool IsLoaded
        {
            get { return false; }
        }

        #endregion Properties

        #region Public Methods

        public override void Initialize()
        {
        }

        #endregion Public Methods
    }
}