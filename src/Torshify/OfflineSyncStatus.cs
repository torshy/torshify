namespace Torshify
{
    public class OfflineSyncStatus
    {
        /// <summary>
        /// Queued tracks is things left to sync in current sync operation
        /// </summary>
        public int QueuedTracks
        {
            get; 
            internal set;
        }

        /// <summary>
        /// Queued bytes is things left to sync in current sync operation
        /// </summary>
        public ulong QueuedBytes
        {
            get;
            internal set;
        }

        /// <summary>
        /// Done tracks is things marked for sync that existed on device before current sync operation
        /// </summary>
        public int DoneTracks
        {
            get; 
            internal set;
        }

        /// <summary>
        /// Done bytes is things marked for sync that existed on device before current sync operation
        /// </summary>
        public ulong DoneBytes
        {
            get; 
            internal set;
        }

        /// <summary>
        ///  Copied tracks is things that has been copied in current sync operation
        /// </summary>
        public int CopiedTracks
        {
            get; 
            internal set;
        }

        /// <summary>
        ///  Copied bytes is things that has been copied in current sync operation
        /// </summary>
        public ulong CopiedBytes
        {
            get; 
            internal set;
        }

        /// <summary>
        /// Tracks that are marked as synced but will not be copied (for various reasons)
        /// </summary>
        public int WillNotCopyTracks
        {
            get; 
            internal set;
        }

        /// <summary>
        ///  A track is counted as error when something goes wrong while syncing the track
        /// </summary>
        public int ErrorTracks
        {
            get; 
            internal set;
        }

        /// <summary>
        /// Set if sync operation is in progress
        /// </summary>
        public bool IsSyncing
        {
            get; 
            internal set;
        }
    }
}