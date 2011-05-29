using System;
using System.Runtime.InteropServices;

namespace Torshify.Core.Native
{
    internal class NativeImage : NativeObject, IImage
    {
        #region Fields

        protected ImageLoadedCallback _imageLoaded;
        protected Lazy<byte[]> _data;

        private readonly string _id;

        #endregion Fields

        #region Constructors

        public NativeImage(ISession session, string id)
            : base(session, IntPtr.Zero)
        {
            _id = id;
        }

        #endregion Constructors

        #region Delegates

        internal delegate void ImageLoadedCallback(IntPtr imagePtr, IntPtr userdataPtr);

        #endregion Delegates

        #region Events

        public event EventHandler Loaded;

        #endregion Events

        #region Properties

        public bool IsLoaded
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_image_is_loaded(Handle);
                }
            }
        }

        public byte[] Data
        {
            get
            {
                AssertHandle();

                return _data.Value;
            }
        }

        public Error Error
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_image_error(Handle);
                }
            }
        }

        public ImageFormat Format
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.sp_image_format(Handle);
                }
            }
        }

        public string ImageId
        {
            get
            {
                AssertHandle();

                lock (Spotify.Mutex)
                {
                    return Spotify.ImageIdToString(Spotify.sp_image_image_id(Handle));
                }
            }
        }

        #endregion Properties

        #region Public Methods

        public override void Initialize()
        {
            byte[] idArray = Spotify.StringToImageId(_id);

            if (idArray.Length != 20)
                throw new Exception("Internal error in FromId");

            try
            {
                lock (Spotify.Mutex)
                {
                    Handle = Spotify.sp_image_create(Session.GetHandle(), idArray);
                }

                _data = new Lazy<byte[]>(GetImageData);
                _imageLoaded = OnImageLoadedCallback;

                lock (Spotify.Mutex)
                {
                    Spotify.sp_image_add_ref(Handle);
                    Spotify.sp_image_add_load_callback(Handle, _imageLoaded, IntPtr.Zero);
                }
            }
            catch
            {

            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dipose managed
            }

            if (!IsInvalid)
            {
                try
                {
                    lock (Spotify.Mutex)
                    {
                        Spotify.sp_image_remove_load_callback(Handle, _imageLoaded, IntPtr.Zero);
                        Spotify.sp_image_release(Handle);
                    }

                }
                catch
                {

                }
                finally
                {
                    Handle = IntPtr.Zero;
                }
            }

            base.Dispose(disposing);
        }

        protected byte[] GetImageData()
        {
            try
            {
                IntPtr lengthPtr = IntPtr.Zero;
                IntPtr dataPtr = IntPtr.Zero;
                lock (Spotify.Mutex)
                {
                    dataPtr = Spotify.sp_image_data(Handle, out lengthPtr);
                }

                int length = lengthPtr.ToInt32();

                if (dataPtr == IntPtr.Zero)
                    return null;
                if (length == 0)
                    return new byte[0];

                byte[] imageData = new byte[length];
                Marshal.Copy(dataPtr, imageData, 0, length);
                return imageData;
            }
            catch
            {
                return null;
            }
        }

        protected void OnImageLoadedCallback(IntPtr imageptr, IntPtr userdataptr)
        {
            if (imageptr != Handle) return;

            this.QueueThis<NativeImage, EventArgs>(
                image => image.OnImageLoaded,
                this,
                EventArgs.Empty);
        }

        #endregion Protected Methods

        #region Private Methods

        private void OnImageLoaded(EventArgs e)
        {
            Loaded.RaiseEvent(this, e);
        }

        #endregion Private Methods
    }
}