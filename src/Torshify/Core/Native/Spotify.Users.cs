using System;
using System.Runtime.InteropServices;

namespace Torshify.Core.Native
{
    internal partial class Spotify
    {
        /// <summary>
        /// Get a pointer to a string representing the user's canonical username.
        /// </summary>
        /// <param name="user">The Spotify user whose canonical username you would like a string representation of</param>
        /// <returns> A string representing the canonical username.</returns>
        //[DllImport("libspotify")]
        //internal static extern IntPtr sp_user_canonical_name(IntPtr user);

        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(MarshalPtrToUtf8))]
        internal static extern string sp_user_canonical_name(IntPtr user);

        /// <summary>
        /// Get a pointer to a string representing the user's displayable username.
        /// If there is no difference between the canonical username and the display name,
        /// or if the library does not know about the display name yet, the canonical username will be returned.
        /// </summary>
        /// <param name="user">The Spotify user whose displayable username you would like a string representation of</param>
        /// <returns>A string</returns>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(MarshalPtrToUtf8))]
        internal static extern string sp_user_display_name(IntPtr user);

        /// <summary>
        /// Get load status for a user object. Before it is loaded, only the user's canonical username is known.
        /// </summary>
        /// <param name="user">Spotify user object</param>
        /// <returns>True if user object is loaded, otherwise false</returns>
        [DllImport("libspotify")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool sp_user_is_loaded(IntPtr user);

        /// <summary>
        /// Increase the reference count of an user
        /// </summary>
        /// <param name="user">The user object</param>
        [DllImport("libspotify")]
        internal static extern Error sp_user_add_ref(IntPtr user);

        /// <summary>
        /// Decrease the reference count of an user
        /// </summary>
        /// <param name="user">The user object</param>
        [DllImport("libspotify")]
        internal static extern Error sp_user_release(IntPtr user);
    }
}