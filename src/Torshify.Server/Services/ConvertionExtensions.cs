using System.Linq;

using Torshify.Server.Contracts;
using Torshify.Server.Services.Caching;

namespace Torshify.Server.Services
{
    public static class ConvertionExtensions
    {
        #region Public Static Methods

        public static Album ToDto(this IAlbum album)
        {
            Album dto = new Album();
            dto.ID = album.GetHashCode();
            dto.Name = album.Name;
            dto.Artist = album.Artist.ToDto();
            dto.CoverId = album.CoverId;
            dto.IsAvailable = album.IsAvailable;
            dto.Year = album.Year;
            return dto;
        }

        public static Artist ToDto(this IArtist artist)
        {
            Artist dto = new Artist();
            dto.ID = artist.GetHashCode();
            dto.Name = artist.Name;
            return dto;
        }

        public static Track ToDto(this ITrack track)
        {
            TrackCache.Instance.Put(track, track.GetHashCode());

            Track dto;

            //if (track is IPlaylistTrack)
            //{
            //    IPlaylistTrack playlistTrack = (IPlaylistTrack) track;
            //    dto = new PlaylistTrack
            //              {
            //                  PlaylistID = playlistTrack.Playlist.GetHashCode(),
            //                  CreateTime = playlistTrack.CreateTime,
            //                  Seen = playlistTrack.Seen
            //              };
            //}
            //else
            {
                dto = new Track();
            }

            dto.ID = track.GetHashCode();
            dto.Name = track.Name;
            dto.Album = track.Album.ToDto();
            dto.Disc = track.Disc;
            dto.Index = track.Index;
            dto.IsAvailable = track.IsAvailable;
            dto.IsStarred = track.IsStarred;
            dto.Popularity = track.Popularity;
            dto.Duration = track.Duration;
            dto.Artists = track.Artists.Select(a => a.ToDto()).ToArray();

            return dto;
        }

        public static SearchResult ToDto(this ISearch search)
        {
            SearchResult dto = new SearchResult();
            dto.DidYouMean = search.DidYouMean;
            dto.Query = search.Query;
            dto.TotalAlbums = search.TotalAlbums;
            dto.TotalArtists = search.TotalArtists;
            dto.TotalTracks = search.TotalTracks;
            dto.Albums = search.Albums.Select(a => a.ToDto()).ToArray();
            dto.Artists = search.Artists.Select(a => a.ToDto()).ToArray();
            dto.Tracks = search.Tracks.Select(t => t.ToDto()).ToArray();
            return dto;
        }

        public static Playlist ToDto(this IPlaylist playlist)
        {
            Playlist dto = PlaylistCache.Instance.Get(playlist);

            if (dto != null)
            {
                return dto;
            }

            dto = new Playlist();
            dto.ID = playlist.GetHashCode();
            dto.Name = playlist.Name;
            dto.Description = playlist.Description;
            dto.IsCollaborative = playlist.IsCollaborative;
            dto.ImageId = playlist.ImageId;
            dto.PendingChanges = playlist.PendingChanges;
            dto.Tracks = playlist.Tracks.Select(t => t.ToDto()).ToArray();

            PlaylistCache.Instance.Put(playlist, dto);

            return dto;
        }

        #endregion Public Static Methods
    }
}