using System;
using System.Collections.Generic;

using Torshify.Server.Interfaces;

namespace Torshify.Server.Services
{
    public class PlayerPlaylist : IPlayerPlaylist
    {
        #region Fields

        private Queue<ITrack> _queue;
        private List<ITrack> _playlist;
        private int[] _playlistIndicies;
        private int _playlistTrackIndex;
        private object _queueLock = new object();
        private ITrack _currentTrack;
        private bool _shuffle;
        private bool _repeat;

        #endregion Fields

        #region Constructors

        public PlayerPlaylist()
        {
            _queue = new Queue<ITrack>();
            _playlist = new List<ITrack>();
        }

        #endregion Constructors

        #region Events

        public event EventHandler CurrentChanged;

        public event EventHandler RepeatChanged;

        public event EventHandler ShuffleChanged;

        #endregion Events

        #region Properties

        public bool Shuffle
        {
            get { return _shuffle; }
            set
            {
                _shuffle = value;
                Update();
                OnShuffleChanged();
            }
        }

        public bool Repeat
        {
            get { return _repeat; }
            set
            {
                _repeat = value;
                OnRepeatChanged();
            }
        }

        public IEnumerable<ITrack> All
        {
            get
            {
                yield return Current;

                foreach (var track in _queue.ToArray())
                {
                    yield return track;
                }

                foreach (var track in _playlist.ToArray())
                {
                    yield return track;
                }
            }
        }

        public ITrack Current
        {
            get
            {
                return _currentTrack;
            }
            private set
            {
                _currentTrack = value;
                OnCurrentChanged();
            }
        }

        public bool CanGoNext
        {
            get
            {
                if (_playlistTrackIndex == -1)
                {
                    return false;
                }

                if (_queue.Count > 0 )
                {
                    return true;
                }

                if (_playlistTrackIndex < (_playlistIndicies.Length - 1))
                {
                    return true;
                }

                if (_repeat)
                {
                    return true;
                }

                return false;
            }
        }

        public bool CanGoPrevious
        {
            get
            {
                if (_playlistTrackIndex == -1)
                {
                    return false;
                }

                if (_playlistTrackIndex > 0)
                {
                    return true;
                }

                if (_repeat)
                {
                    return true;
                }

                return false;
            }
        }

        #endregion Properties

        #region Public Methods

        public void Set(IEnumerable<ITrack> tracks)
        {
            _playlist = new List<ITrack>(tracks);
            
            Update();

            Current = _playlist[_playlistIndicies[0]];
        }

        public void Enqueue(ITrack track)
        {
            lock (_queueLock)
            {
                _queue.Enqueue(track);
            }
        }

        public void Enqueue(IEnumerable<ITrack> tracks)
        {
            lock (_queueLock)
            {
                foreach (var track in tracks)
                {
                    _queue.Enqueue(track);
                }
            }
        }

        public bool Next()
        {
            if (_queue.Count > 0)
            {
                Current = _queue.Dequeue();
                return true;
            }

            if (_playlistTrackIndex < (_playlistIndicies.Length - 1))
            {
                _playlistTrackIndex++;

                int indexToPlay = _playlistIndicies[_playlistTrackIndex];
                Current = _playlist[indexToPlay];
                return true;
            }

            if (Repeat)
            {
                _playlistTrackIndex = 0;
                int indexToPlay = _playlistIndicies[_playlistTrackIndex];
                Current = _playlist[indexToPlay];
                return true;
            }

            return false;
        }

        public bool Previous()
        {
            if (_playlistTrackIndex > 0)
            {
                _playlistTrackIndex--;

                int indexToPlay = _playlistIndicies[_playlistTrackIndex];
                Current = _playlist[indexToPlay];
                return true;
            }

            if (_repeat)
            {
                _playlistTrackIndex = _playlistIndicies.Length - 1;

                int indexToPlay = _playlistIndicies[_playlistTrackIndex];
                Current = _playlist[indexToPlay];
                return true;
            }

            return false;
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void OnCurrentChanged()
        {
            var handler = CurrentChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnRepeatChanged()
        {
            var handler = RepeatChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected virtual void OnShuffleChanged()
        {
            var handler = ShuffleChanged;

            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }

        #endregion Protected Methods

        #region Private Static Methods

        private static int[] BuildShuffledIndexArray(int size)
        {
            int[] array = new int[size];
            Random rand = new Random();
            for (int currentIndex = array.Length - 1; currentIndex > 0; currentIndex--)
            {
                int nextIndex = rand.Next(currentIndex + 1);
                Swap(array, currentIndex, nextIndex);
            }

            return array;
        }

        private static void Swap(IList<int> array, int firstIndex, int secondIndex)
        {
            if (array[firstIndex] == 0)
            {
                array[firstIndex] = firstIndex;
            }

            if (array[secondIndex] == 0)
            {
                array[secondIndex] = secondIndex;
            }

            int temp = array[secondIndex];
            array[secondIndex] = array[firstIndex];
            array[firstIndex] = temp;
        }

        #endregion Private Static Methods

        #region Private Methods

        private void Update()
        {
            if (_playlist == null)
            {
                return;
            }

            if (Shuffle)
            {
                _playlistIndicies = BuildShuffledIndexArray(_playlist.Count);
            }
            else
            {
                _playlistIndicies = new int[_playlist.Count];

                for (int i = 0; i < _playlist.Count; i++)
                {
                    _playlistIndicies[i] = i;
                }
            }
        }

        #endregion Private Methods
    }
}