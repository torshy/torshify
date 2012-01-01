using System;
using System.Linq;

namespace Torshify.Core.Native
{
    internal class NativePlaylistList : DelegateList<IContainerPlaylist>, IPlaylistList
    {
        #region Fields

        private readonly Func<ILink<IPlaylist>, IContainerPlaylist> _addExisting;
        private readonly Func<int, string, Error> _addFolder;
        private readonly Func<string, IContainerPlaylist> _addNew;
        private readonly Func<int, Error> _removeAt;

        #endregion Fields

        #region Constructors

        public NativePlaylistList(
            Func<int> getLength,
            Func<int, IContainerPlaylist> getIndex,
            Func<string, IContainerPlaylist> addNew,
            Func<ILink<IPlaylist>, IContainerPlaylist> addExisting,
            Func<int, string, Error> addFolder,
            Action<IContainerPlaylist, int> addFunc,
            Action<int> removeFunc,
            Func<int, Error> removeAt,
            Func<bool> readonlyFunc,
            Action<int, int> moveFunc)
            : base(getLength, getIndex, addFunc, removeFunc, readonlyFunc, moveFunc)
        {
            _addNew = addNew;
            _addExisting = addExisting;
            _addFolder = addFolder;
            _removeAt = removeAt;
        }

        #endregion Constructors

        #region Indexers

        public IContainerPlaylist this[string name]
        {
            get { return this.FirstOrDefault(p => name.Equals(p.Name, StringComparison.InvariantCultureIgnoreCase)); }
        }

        #endregion Indexers

        #region Methods

        public IContainerPlaylist Add(string name)
        {
            return _addNew(name);
        }

        public IContainerPlaylist Add(ILink<IPlaylist> playlist)
        {
            return _addExisting(playlist);
        }

        public Error AddFolder(int index, string name)
        {
            return _addFolder(index, name);
        }

        public Error RemoveAt(int index)
        {
            return _removeAt(index);
        }

        #endregion Methods
    }
}