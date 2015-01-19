using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Torshify.Core.Native
{
    internal static partial class Spotify
    {
        #region Fields

        public const int SPOTIFY_API_VERSION = 12;
        public const int STRINGBUFFER_SIZE = 256;

        internal static readonly object Mutex = new object();

        #endregion Fields

        #region Enumerations

        internal enum SpotifySampletype
        {
            Int16NativeEndian = 0
        }

        #endregion Enumerations

        #region Internal Static Methods

        internal static string GetString(IntPtr ptr, string defaultValue)
        {
            if (ptr == IntPtr.Zero)
            {
                return defaultValue;
            }

            var l = new List<byte>();
            byte read = 0;
            do
            {
                read = Marshal.ReadByte(ptr, l.Count);
                l.Add(read);
            }
            while (read != 0);

            if (l.Count > 0)
            {
                return Encoding.UTF8.GetString(l.ToArray(), 0, l.Count - 1);
            }

            return string.Empty;
        }

        public class MarshalPtrToUtf8 : ICustomMarshaler
        {
            static MarshalPtrToUtf8 marshaler = new MarshalPtrToUtf8();

            private bool _allocated;

            public void CleanUpManagedData(object ManagedObj)
            {

            }

            public void CleanUpNativeData(IntPtr pNativeData)
            {
                if (_allocated)
                {
                    Marshal.FreeHGlobal(pNativeData);
                }
            }

            public int GetNativeDataSize()
            {
                return -1;
            }

            public int GetNativeDataSize(IntPtr ptr)
            {
                int size = 0;
                for (size = 0; Marshal.ReadByte(ptr, size) != 0; size++)
                    ;
                return size;
            }

            public IntPtr MarshalManagedToNative(object managedObj)
            {
                if (managedObj == null)
                    return IntPtr.Zero;
                if (managedObj.GetType() != typeof(string))
                    throw new ArgumentException("ManagedObj", "Can only marshal type of System.string");

                byte[] array = Encoding.UTF8.GetBytes((string)managedObj);
                int size = Marshal.SizeOf(typeof(byte)) * (array.Length + 1);

                IntPtr ptr = Marshal.AllocHGlobal(size);

                Marshal.Copy(array, 0, ptr, array.Length);
                Marshal.WriteByte(ptr, array.Length, 0);
                _allocated = true;
                return ptr;
            }

            public object MarshalNativeToManaged(IntPtr pNativeData)
            {
                if (pNativeData == IntPtr.Zero)
                    return null;

                int size = 0;
                while (Marshal.ReadByte(pNativeData, size) > 0)
                    size++;

                byte[] array = new byte[size];
                Marshal.Copy(pNativeData, array, 0, size);

                return Encoding.UTF8.GetString(array);
            }

            public static ICustomMarshaler GetInstance(string cookie)
            {
                return marshaler;
            }
        }

        public class Utf8StringBuilderMarshaler : ICustomMarshaler
        {
            public static ICustomMarshaler GetInstance(String cookie)
            {
                return new Utf8StringBuilderMarshaler();
            }

            public virtual Object MarshalNativeToManaged(IntPtr pNativeData)
            {
                String stringData = GetString(pNativeData);
                StringBuilder stringBuilder = GetStringBuilder(pNativeData);
                if (stringBuilder != null)
                {
                    stringBuilder.Clear();
                    if (stringData != null)
                    {
                        stringBuilder.Append(stringData);
                    }
                }

                return stringData;
            }

            public virtual IntPtr MarshalManagedToNative(Object ManagedObj)
            {
                return AllocStringBuffer(ManagedObj as StringBuilder);
            }

            public virtual void CleanUpNativeData(IntPtr pNativeData)
            {
                FreeStringBuffer(pNativeData);
            }

            public virtual void CleanUpManagedData(Object ManagedObj)
            { }

            public Int32 GetNativeDataSize()
            {
                return -1;
            }

            protected IntPtr AllocStringBuffer(StringBuilder stringBuilder)
            {
                int bufferSize = GetNativeDataSize() + IntPtr.Size;
                IntPtr bufferPtr = Marshal.AllocHGlobal(bufferSize);

                if (stringBuilder != null)
                {
                    GCHandle stringBuilderHandle = GCHandle.Alloc(stringBuilder, GCHandleType.Normal);
                    IntPtr stringBuilderPtr = GCHandle.ToIntPtr(stringBuilderHandle);
                    Marshal.WriteIntPtr(bufferPtr, stringBuilderPtr);
                }
                else
                {
                    Marshal.WriteIntPtr(bufferPtr, IntPtr.Zero);
                }

                bufferPtr = IntPtr.Add(bufferPtr, IntPtr.Size);
                return bufferPtr;
            }

            protected void FreeStringBuffer(IntPtr stringBuffer)
            {
                stringBuffer = IntPtr.Add(stringBuffer, -IntPtr.Size);
                IntPtr stringBuilderPtr = Marshal.ReadIntPtr(stringBuffer);
                if (stringBuilderPtr != IntPtr.Zero)
                {
                    GCHandle stringBuilderHandle = GCHandle.FromIntPtr(stringBuilderPtr);
                    stringBuilderHandle.Free();
                }

                //Marshal.FreeHGlobal(stringBuffer);
            }

            protected String GetString(IntPtr stringBuffer)
            {
                if (stringBuffer == IntPtr.Zero)
                    return null;

                int size = 0;
                while (Marshal.ReadByte(stringBuffer, size) > 0)
                    size++;

                byte[] array = new byte[size];
                Marshal.Copy(stringBuffer, array, 0, size);

                return Encoding.UTF8.GetString(array);
            }

            protected StringBuilder GetStringBuilder(IntPtr stringBuffer)
            {
                IntPtr stringBuilderPtr = Marshal.ReadIntPtr(stringBuffer, -IntPtr.Size);
                if (stringBuilderPtr != IntPtr.Zero)
                {
                    GCHandle stringBuilderHandle = GCHandle.FromIntPtr(stringBuilderPtr);
                    return stringBuilderHandle.IsAllocated ? (StringBuilder)stringBuilderHandle.Target : null;
                }
                return null;
            }
        }

        internal static string ImageIdToString(IntPtr idPtr)
        {
            if (idPtr == IntPtr.Zero)
            {
                return string.Empty;
            }

            byte[] id = new byte[20];
            Marshal.Copy(idPtr, id, 0, 20);

            return ImageIdToString(id);
        }

        internal static string ImageIdToString(byte[] id)
        {
            if (id == null)
            {
                return string.Empty;
            }

            StringBuilder sb = new StringBuilder();
            foreach (byte b in id)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        internal static byte[] StringToImageId(string id)
        {
            if (string.IsNullOrEmpty(id) || id.Length != 40)
            {
                return null;
            }

            byte[] ret = new byte[20];
            try
            {
                for (int i = 0; i < 20; i++)
                {
                    ret[i] = byte.Parse(id.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
                }

                return ret;
            }
            catch
            {
                return null;
            }
        }

        #endregion Internal Static Methods

        #region Nested Types

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct SpotifyAudioBufferStats
        {
            internal int Samples;
            internal int Stutter;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct SpotifyAudioFormat
        {
            internal IntPtr SampleType;
            internal int SampleRate;
            internal int Channels;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SpotifySessionCallbacks
        {
            internal IntPtr LoggedIn;
            internal IntPtr LoggedOut;
            internal IntPtr MetadataUpdated;
            internal IntPtr ConnectionError;
            internal IntPtr MessageToUser;
            internal IntPtr NotifyMainThread;
            internal IntPtr MusicDelivery;
            internal IntPtr PlayTokenLost;
            internal IntPtr LogMessage;
            internal IntPtr EndOfTrack;
            internal IntPtr StreamingError;
            internal IntPtr UserinfoUpdated;
            internal IntPtr StartPlayback;
            internal IntPtr StopPlayback;
            internal IntPtr GetAudioBufferStats;
            internal IntPtr OfflineStatusUpdated;
            internal IntPtr OfflineError;
            internal IntPtr CredentialsBlobUpdated;
            internal IntPtr ConnectionStateUpdated;
            internal IntPtr ScrobbleError;
            internal IntPtr PrivateSessionModeChanged;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct SpotifySessionConfig
        {
            internal int ApiVersion;
            internal string CacheLocation;
            internal string SettingsLocation;
            internal IntPtr ApplicationKey;
            internal int ApplicationKeySize;
            internal string UserAgent;
            internal IntPtr Callbacks;
            internal IntPtr UserData;
            internal bool CompressPlaylists;
            internal bool DontSaveMetadataForPlaylists;
            internal bool InitiallyUnloadPlaylists;
            internal string DeviceID;
            internal string TraceFile;
            internal string Proxy;
            internal string ProxyUsername;
            internal string ProxyPassword;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct SpotifySubscribers
        {
            [MarshalAs(UnmanagedType.U4)]
            internal uint Count;
            internal IntPtr Subscribers;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct SpotifyOfflineSyncStatus
        {
            /**
            * Queued tracks is things left to sync in current sync
            * operation
            */
            internal int QueuedTracks;
            /**
            * Queued bytes is things left to sync in current sync
            * operation
            */
            internal ulong QueuedBytes;
            /**
            * Done tracks is things marked for sync that existed on
            * device before current sync operation
            */
            internal int DoneTracks;
            /**
            * Done bytes is things marked for sync that existed on
            * device before current sync operation
            */
            internal ulong DoneBytes;
            /**
             * Copied tracks/bytes is things that has been copied in
             * current sync operation
             */
            internal int CopiedTracks;
            /**
             * Copied bytes is things that has been copied in
             * current sync operation
             */
            internal ulong CopiedBytes;

            /**
             * Tracks that are marked as synced but will not be copied
             * (for various reasons)
             */
            internal int WillNotCopyTracks;

            /**
             * A track is counted as error when something goes wrong while
             * syncing the track
             */
            internal int ErrorTracks;

            /**
             * Set if sync operation is in progress
             */
            [MarshalAsAttribute(UnmanagedType.I1)]
            internal bool Syncing;
        }

        #endregion Nested Types
    }
}