using System;

namespace Torshify.Server.Interfaces
{
    public interface IPlayer
    {
        event EventHandler IsPlayingChanged;


        IPlayerPlaylist Playlist { get; }
        bool IsPlaying { get; }


        void Play();
        void Stop();
        void Pause();
        void Seek(TimeSpan timeSpan);
    }
}