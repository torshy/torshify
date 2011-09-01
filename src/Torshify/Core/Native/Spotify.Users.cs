using System;
using System.Runtime.InteropServices;

namespace Torshify.Core.Native
{
    internal partial class Spotify
    {
        /// <summary>
        /// Fetches the currently logged in user.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <returns>The logged in user (or null if not logged in).</returns>
        [DllImport("spotify")]
        internal static extern IntPtr sp_session_user(IntPtr sessionPtr);

        /// <summary>
        /// The userdata associated with the session.
        /// </summary>
        /// <param name="sessionPtr">Session object returned from <c>sp_session_create</c>.</param>
        /// <returns>The userdata that was passed in on session creation.</returns>
        [DllImport("spotify")]
        internal static extern IntPtr sp_session_user_data(IntPtr sessionPtr);

        /// <summary>
        /// Get a pointer to a string representing the user's canonical username.
        /// </summary>
        /// <param name="user">The Spotify user whose canonical username you would like a string representation of</param>
        /// <returns> A string representing the canonical username.</returns>
        //[DllImport("spotify")]
        //internal static extern IntPtr sp_user_canonical_name(IntPtr user);

        [DllImport("spotify")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(MarshalPtrToUtf8))]
        internal static extern string sp_user_canonical_name(IntPtr user);

        /// <summary>
        /// Get a pointer to a string representing the user's displayable username.
        /// If there is no difference between the canonical username and the display name,
        /// or if the library does not know about the display name yet, the canonical username will be returned.
        /// </summary>
        /// <param name="user">The Spotify user whose displayable username you would like a string representation of</param>
        /// <returns>A string</returns>
        [DllImport("spotify")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(MarshalPtrToUtf8))]
        internal static extern string sp_user_display_name(IntPtr user);

        /// <summary>
        /// Get load status for a user object. Before it is loaded, only the user's canonical username is known.
        /// </summary>
        /// <param name="user">Spotify user object</param>
        /// <returns>True if user object is loaded, otherwise false</returns>
        [DllImport("spotify")]
        internal static extern bool sp_user_is_loaded(IntPtr user);

        /// <summary>
        /// Get a pointer to a string representing the user's full name as returned from social networks.
        /// </summary>
        /// <param name="user">The Spotify user whose displayable username you would like a string representation of</param>
        /// <returns>A string, NULL if the full name is not known.</returns>
        [DllImport("spotify")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(MarshalPtrToUtf8))]
        internal static extern string sp_user_full_name(IntPtr user);

        /// <summary>
        /// Get a pointer to an URL for an picture representing the user
        /// </summary>
        /// <param name="user">The Spotify user whose displayable username you would like a string representation of</param>
        /// <returns>A string, NULL if the URL is not known.</returns>
        [DllImport("spotify")]
        internal static extern string sp_user_picture(IntPtr user);

        /// <summary>
        /// Increase the reference count of an user
        /// </summary>
        /// <param name="user">The user object</param>
        [DllImport("spotify")]
        internal static extern void sp_user_add_ref(IntPtr user);

        /// <summary>
        /// Decrease the reference count of an user
        /// </summary>
        /// <param name="user">The user object</param>
        [DllImport("spotify")]
        internal static extern void sp_user_release(IntPtr user);

        /// <summary>
        /// Get relation type for a given user
        /// </summary>
        /// <param name="sessionPtr">Session</param>
        /// <param name="user">The Spotify user you want to query relation type for</param>
        /// <returns></returns>
        [DllImport("spotify")]
        internal static extern RelationType sp_user_relation_type(IntPtr sessionPtr, IntPtr user);
    }
}