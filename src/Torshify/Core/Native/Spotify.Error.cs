using System;
using System.Runtime.InteropServices;

namespace Torshify.Core.Native
{
  internal partial class Spotify
  {
      /// <summary>
      /// Convert a numeric libspotify error code to a text string.
      /// </summary>
      /// <param name="error">The error code.</param>
      /// <returns>The text-representation of the error.</returns>
      [DllImport("libspotify")]
      [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(MarshalPtrToUtf8))]
      public static extern string sp_error_message(Error error);
  }
}