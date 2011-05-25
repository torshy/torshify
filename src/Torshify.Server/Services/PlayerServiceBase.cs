using System;
using System.Collections.Generic;
using Microsoft.Practices.ServiceLocation;
using Torshify.Server.Extensions;
using Torshify.Server.Interfaces;
using Torshify.Server.Services.Caching;

namespace Torshify.Server.Services
{
    public abstract class PlayerServiceBase
    {
        public void Enqueue(int trackId)
        {
            IPlayer player = ServiceLocator.Current.Resolve<IPlayer>();

            ITrack track = TrackCache.Instance.Get(trackId);

            if (track != null)
            {
                player.Playlist.Enqueue(track);
            }
        }

        public void Play(int trackId)
        {
            IPlayer player = ServiceLocator.Current.Resolve<IPlayer>();

            ITrack track = TrackCache.Instance.Get(trackId);

            if (track != null)
            {
                IPlaylistTrack playlistTrack = track as IPlaylistTrack;

                if (playlistTrack != null)
                {
                    int index = playlistTrack.Playlist.Tracks.IndexOf(playlistTrack);

                    List<ITrack> tracks = new List<ITrack>();

                    for (int i = index; i < playlistTrack.Playlist.Tracks.Count; i++)
                    {
                        tracks.Add(playlistTrack.Playlist.Tracks[i]);
                    }

                    player.Playlist.Set(tracks);
                    player.Play();
                }
                else
                {
                    player.Playlist.Set(new[] { track });
                    player.Play();

                    // 1. Get all tracks by artist
                    // 2. Jump to the track
                    // 3. Create a playlist with the remaining tracks
                }
            }
        }

        public void TogglePlayPause()
        {
            IPlayer player = ServiceLocator.Current.Resolve<IPlayer>();
            
            if (player.IsPlaying)
            {
                player.Pause();
            }
            else
            {
                player.Play();
            }
        }

        public void Pause()
        {
            IPlayer player = ServiceLocator.Current.Resolve<IPlayer>();
            player.Pause();
        }

        public void Seek(int milliseconds)
        {
            IPlayer player = ServiceLocator.Current.Resolve<IPlayer>();
            player.Seek(TimeSpan.FromMilliseconds(milliseconds));
        }

        public void Prefetch(int trackId)
        {
            ITrack track = TrackCache.Instance.Get(trackId);

            if (track != null)
            {
                track.Prefetch();
            }
        }

        public bool Next()
        {
            IPlayer player = ServiceLocator.Current.Resolve<IPlayer>();
            return player.Playlist.Next();
        }

        public bool Previous()
        {
            IPlayer player = ServiceLocator.Current.Resolve<IPlayer>();
            return player.Playlist.Previous();
        }

        public bool CanGoNext()
        {
            IPlayer player = ServiceLocator.Current.Resolve<IPlayer>();
            return player.Playlist.CanGoNext;
        }

        public bool CanGoPrevious()
        {
            IPlayer player = ServiceLocator.Current.Resolve<IPlayer>();
            return player.Playlist.CanGoPrevious;
        }
    }
}