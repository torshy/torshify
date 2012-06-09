namespace Torshify
{
    public interface IEditableArray<T> : IArray<T>
    {
        #region Properties

        bool IsReadOnly
        {
            get;
        }

        #endregion Properties

        #region Methods

        void Add(T item);

        void Clear();

        bool Contains(T item);

        void CopyTo(T[] array, int arrayIndex);

        int IndexOf(T item);

        bool Remove(T item);

        void Move(int oldIndex, int newIndex);

        #endregion Methods
    }
}