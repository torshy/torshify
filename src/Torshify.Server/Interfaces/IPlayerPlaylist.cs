using System;
using System.Collections.Generic;

namespace Torshify.Server.Interfaces
{
    public interface IPlayerPlaylist
    {
        event EventHandler ShuffleChanged;
        event EventHandler RepeatChanged;
        event EventHandler CurrentChanged;

        ITrack Current { get; }
        bool CanGoNext { get; }
        bool CanGoPrevious { get; }

        void Set(IEnumerable<ITrack> tracks);
        void Enqueue(ITrack track);
        void Enqueue(IEnumerable<ITrack> tracks);
        bool Next();
        bool Previous();

        bool Shuffle { get; set; }
        bool Repeat { get; set; }
    }
}