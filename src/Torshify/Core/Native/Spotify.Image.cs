using System;
using System.Runtime.InteropServices;

namespace Torshify.Core.Native
{
    internal partial class Spotify
    {
        /// <summary>
        /// Create an image object.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <param name="idPtr">Spotify image ID.</param>
        /// <returns>Pointer to an image object. To free the object, use <c>sp_image_release()</c>.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_image_create(IntPtr sessionPtr, byte[] idPtr);

        /// <summary>
        /// Create an image object from a link
        /// 
        /// </summary>
        /// <param name="sessionptr">Session</param>
        /// <param name="linkPtr">Spotify link object. This must be of SP_LINKTYPE_IMAGE type</param>
        /// <returns>Pointer to an image object. To free the object, use sp_image_release()</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_image_create_from_link(IntPtr sessionptr, IntPtr linkPtr);

        /// <summary>
        /// Add a callback that will be invoked when the image is loaded.
        /// If an image is loaded, and loading fails, the image will behave like an empty image.
        /// </summary>
        /// <param name="imagePtr">The image.</param>
        /// <param name="callbackPtr">Callback that will be called when image has been fetched.</param>
        /// <param name="userdataPtr">Opaque pointer passed to <c>callback</c>.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_image_add_load_callback(IntPtr imagePtr, NativeImage.ImageLoadedCallback loadedCallback, IntPtr userdataPtr);

        /// <summary>
        /// Remove an image load callback previously added with libspotify.sp_image_add_load_callback.
        /// </summary>
        /// <param name="imagePtr">The image.</param>
        /// <param name="callbackPtr">Callback that will not be called when image has been fetched.</param>
        /// <param name="userdataPtr">Opaque pointer passed to <c>callback</c></param>
        [DllImport("libspotify")]
        internal static extern Error sp_image_remove_load_callback(IntPtr imagePtr, NativeImage.ImageLoadedCallback loadedCallback, IntPtr userdataPtr);

        /// <summary>
        /// Check if an image is loaded. Before the image is loaded,
        /// the rest of the methods will behave as if the image is empty.
        /// </summary>
        /// <param name="imagePtr">Image object.</param>
        /// <returns>True if image is loaded, false otherwise.</returns>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool sp_image_is_loaded(IntPtr imagePtr);

        /// <summary>
        /// Check if image retrieval returned an error code.
        /// </summary>
        /// <param name="imagePtr">Image object.</param>
        /// <returns>Error code.</returns>
        [DllImport("libspotify")]
        internal static extern Error sp_image_error(IntPtr imagePtr);

        /// <summary>
        /// Get image format.
        /// </summary>
        /// <param name="imagePtr">Image object.</param>
        /// <returns>Image format as described by <see cref="ImageFormat"/>.</returns>
        [DllImport("libspotify")]
        internal static extern ImageFormat sp_image_format(IntPtr imagePtr);

        /// <summary>
        /// Get image data.
        /// </summary>
        /// <param name="imagePtr">Image object.</param>
        /// <param name="sizePtr">Size of raw image data.</param>
        /// <returns>Pointer to raw image data.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_image_data(IntPtr imagePtr, out IntPtr sizePtr);

        /// <summary>
        /// Get image ID.
        /// </summary>
        /// <param name="imagePtr">Image object.</param>
        /// <returns>Image ID.</returns>
        [DllImport("libspotify")]
        internal static extern IntPtr sp_image_image_id(IntPtr imagePtr);

        /// <summary>
        /// Increase the reference count of an image.
        /// </summary>
        /// <param name="imagePtr">The image.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_image_add_ref(IntPtr imagePtr);

        /// <summary>
        /// Decrease the reference count of an image.
        /// </summary>
        /// <param name="imagePtr">The image.</param>
        [DllImport("libspotify")]
        internal static extern Error sp_image_release(IntPtr imagePtr);
    }
}