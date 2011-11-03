using System;
using Torshify.Core.Managers;

namespace Torshify.Core.Native
{
    internal class NativeTrackAndOffsetLink : NativeLink, ILink<ITrackAndOffset>
    {
        #region Fields

        private Lazy<LinkAndOffset> _track;

        #endregion Fields

        #region Constructors

        public NativeTrackAndOffsetLink(ISession session, IntPtr handle)
            : base(session, handle)
        {
        }

        #endregion Constructors

        #region Properties

        public override object Object
        {
            get { return _track.Value; }
        }

        ITrackAndOffset ILink<ITrackAndOffset>.Object
        {
            get { return (ITrackAndOffset)Object; }
        }

        #endregion Properties

        #region Public Methods

        public override void Initialize()
        {
            _track = new Lazy<LinkAndOffset>(() =>
                                                 {
                                                     AssertHandle();
                                                     int offset;
                                                     IntPtr trackPtr;

                                                     // TODO : If TrackPtr is Zero. return dummy link?
                                                     lock (Spotify.Mutex)
                                                     {
                                                         trackPtr = Spotify.sp_link_as_track_and_offset(Handle, out offset);
                                                     }

                                                     return new LinkAndOffset(Session, trackPtr, offset);
                                                 });
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }

            base.Dispose(disposing);
        }

        #endregion Protected Methods

        #region Nested Types

        private class LinkAndOffset : ITrackAndOffset
        {
            #region Fields

            private readonly ITrack _track;

            private TimeSpan _offset;

            #endregion Fields

            #region Constructors

            public LinkAndOffset(ISession session, IntPtr linkPtr, int offset)
            {
                _offset = TimeSpan.FromSeconds(offset);
                _track = TrackManager.Get(session, linkPtr);
            }

            #endregion Constructors

            #region Properties

            public ITrack Track
            {
                get { return _track; }
            }

            public TimeSpan Offset
            {
                get { return _offset; }
            }

            #endregion Properties
        }

        #endregion Nested Types
    }
}