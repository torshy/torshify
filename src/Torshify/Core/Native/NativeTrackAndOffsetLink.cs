using System;

using Torshify.Core.Managers;

namespace Torshify.Core.Native
{
    internal class NativeTrackAndOffsetLink : NativeLink, ILink<ITrackAndOffset>
    {
        #region Fields

        private TimeSpan? _offset;
        private Lazy<LinkAndOffset> _track;

        #endregion Fields

        #region Constructors

        public NativeTrackAndOffsetLink(ISession session, IntPtr handle)
            : base(session, handle)
        {
        }

        public NativeTrackAndOffsetLink(ISession session, IntPtr handle, TimeSpan? offset)
            : base(session, handle)
        {
            _offset = offset;
        }

        public NativeTrackAndOffsetLink(ISession session, IntPtr handle, string linkAsString)
            : base(session, handle)
        {
            if (!string.IsNullOrEmpty(linkAsString))
            {
                int indexOfHash = linkAsString.LastIndexOf("#", StringComparison.Ordinal);

                if (indexOfHash != -1)
                {
                    try
                    {
                        string timeOffsetAsString = linkAsString.Substring(indexOfHash + 1, (linkAsString.Length - indexOfHash) - 1);

                        TimeSpan timeOffset;
                        if (TimeSpan.TryParse(timeOffsetAsString, out timeOffset))
                        {
                            _offset = new TimeSpan(0, 0, timeOffset.Hours, timeOffset.Minutes);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
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

        #region Methods

        public override void Initialize()
        {
            _track = new Lazy<LinkAndOffset>(() =>
                                                 {
                                                     AssertHandle();
                                                     IntPtr trackPtr = IntPtr.Zero;

                                                     // TODO : If TrackPtr is Zero. return dummy link?
                                                     lock (Spotify.Mutex)
                                                     {
                                                         if (!_offset.HasValue)
                                                         {
                                                             int offset;
                                                             trackPtr = Spotify.sp_link_as_track_and_offset(
                                                                 Handle,
                                                                 out offset);
                                                             _offset = TimeSpan.FromMilliseconds(offset);
                                                         }

                                                         if (trackPtr == IntPtr.Zero)
                                                         {
                                                             trackPtr = Spotify.sp_link_as_track(Handle);
                                                         }
                                                     }

                                                     return new LinkAndOffset(Session, trackPtr, _offset.GetValueOrDefault());
                                                 });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }

            base.Dispose(disposing);
        }

        #endregion Methods

        #region Nested Types

        private class LinkAndOffset : ITrackAndOffset
        {
            #region Fields

            private readonly ITrack _track;
            private readonly TimeSpan _offset;

            #endregion Fields

            #region Constructors

            public LinkAndOffset(ISession session, IntPtr linkPtr, TimeSpan offset)
            {
                _offset = offset;
                _track = TrackManager.Get(session, linkPtr);
            }

            #endregion Constructors

            #region Properties

            public TimeSpan Offset
            {
                get { return _offset; }
            }

            public ITrack Track
            {
                get { return _track; }
            }

            #endregion Properties
        }

        #endregion Nested Types
    }
}