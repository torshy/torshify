using System;

namespace Torshify.Core.Native
{
    internal class NativeImageLink : NativeLink, ILink<IImage>
    {
        #region Fields

        private Lazy<IImage> _image;

        #endregion Fields

        #region Constructors

        public NativeImageLink(ISession session, IntPtr handle)
            : base(session, handle)
        {
        }

        #endregion Constructors

        #region Properties

        public override object Object
        {
            get { return _image.Value; }
        }

        IImage ILink<IImage>.Object
        {
            get { return (IImage)Object; }
        }

        #endregion Properties

        #region Public Methods

        public override void Initialize()
        {
            _image = new Lazy<IImage>(() =>
            {
                AssertHandle();

                var image = new NativeImageFromLink(Session, Handle);
                image.Initialize();
                return image;
            });
        }

        #endregion Public Methods

        #region Nested Types

        private class NativeImageFromLink : NativeImage
        {
            #region Fields

            private readonly IntPtr _linkHandle;

            #endregion Fields

            #region Constructors

            public NativeImageFromLink(ISession session, IntPtr linkHandle)
                : base(session, string.Empty)
            {
                _linkHandle = linkHandle;
            }

            #endregion Constructors

            #region Public Methods

            public override void Initialize()
            {
                try
                {
                    lock (Spotify.Mutex)
                    {
                        Handle = Spotify.sp_image_create_from_link(Session.GetHandle(), _linkHandle);
                    }

                    DataLoadLazy = new Lazy<byte[]>(GetImageData);
                    ImageLoaded = OnImageLoadedCallback;

                    lock (Spotify.Mutex)
                    {
                        Spotify.sp_image_add_load_callback(Handle, ImageLoaded, IntPtr.Zero);
                    }
                }
                catch
                {
                }
            }

            #endregion Public Methods
        }

        #endregion Nested Types
    }
}