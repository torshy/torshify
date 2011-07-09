using System;

namespace Torshify.Core.Native
{
    internal class NativePlaylistTrackList : DelegateList<IPlaylistTrack>, IPlaylistTrackList
    {
        #region Fields

        private readonly Action<ITrack, int> _addTrackFunc;
        private readonly Action<int[], int> _moveMultipleFunc;

        #endregion Fields

        #region Constructors

        public NativePlaylistTrackList(
            Func<int> getLength,
            Func<int, IPlaylistTrack> getIndex,
            Action<IPlaylistTrack, int> addFunc,
            Action<ITrack, int> addTrackFunc,
            Action<int> removeFunc,
            Func<bool> readonlyFunc,
            Action<int, int> moveFunc,
            Action<int[], int> moveMultipleFunc)
            : base(getLength, getIndex, addFunc, removeFunc, readonlyFunc, moveFunc)
        {
            _addTrackFunc = addTrackFunc;
            _moveMultipleFunc = moveMultipleFunc;
        }

        #endregion Constructors

        #region Public Methods

        public void Add(ITrack item)
        {
            _addTrackFunc(item, GetLength());
        }

        public void Add(int index, ITrack item)
        {
            _addTrackFunc(item, index);
        }

        public void Move(int[] indices, int newIndex)
        {
            _moveMultipleFunc(indices, newIndex);
        }

        #endregion Public Methods
    }
}