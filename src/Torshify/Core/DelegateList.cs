using System;
using System.Linq;

namespace Torshify.Core
{
    public class DelegateList<T> : DelegateArray<T>, IEditableArray<T>
    {
        #region Fields

        private readonly Action<T, int> _addFunc;
        private readonly Action<int> _removeFunc;
        private readonly Func<bool> _readonlyFunc;
        private readonly Action<int, int> _moveFunc;

        #endregion Fields

        #region Constructors

        public DelegateList(
            Func<int> getLength, 
            Func<int, T> getIndex, 
            Action<T, int> addFunc, 
            Action<int> removeFunc, 
            Func<bool> readonlyFunc,
            Action<int, int> moveFunc)
            : base(getLength, getIndex)
        {
            _addFunc = addFunc;
            _removeFunc = removeFunc;
            _readonlyFunc = readonlyFunc;
            _moveFunc = moveFunc;
        }

        #endregion Constructors

        #region Properties

        public bool IsReadOnly
        {
            get
            {
                return _readonlyFunc();
            }
        }

        #endregion Properties

        #region Public Methods

        public void Add(T item)
        {
            _addFunc(item, GetLength());
        }

        public void Clear()
        {
            while (Count > 0)
            {
                _removeFunc(0);
            }
        }

        public bool Contains(T item)
        {
            return Enumerable.Contains(this, item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException("arrayIndex");
            }

            if (Count > array.Length - arrayIndex)
            {
                throw new ArgumentException("Array to small");
            }

            int i = arrayIndex;
            foreach (T item in this)
            {
                array[i++] = item;
            }
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);
            if (index == -1)
            {
                return false;
            }

            _removeFunc(index);
            return true;
        }

        public void Move(int oldIndex, int newIndex)
        {
            _moveFunc(oldIndex, newIndex);
        }

        public int IndexOf(T item)
        {
            bool found = false;
            int i = 0, size = GetLength();

            while (!found && i < size)
            {
                if (!this[i].Equals(item))
                {
                    i++;
                }
                else
                {
                    found = true;
                }
            }

            if (!found)
            {
                return -1;
            }

            return i;
        }

        public override IArray<TResult> Cast<TResult>()
        {
            return new DelegateList<TResult>(
                GetLength,
                index =>
                {
                    object obj = GetIndex(index);
                    return (TResult)obj;
                },
                (value, index) =>
                {
                    throw new InvalidOperationException();
                },
                _removeFunc,
                _readonlyFunc,
                _moveFunc);
        }

        #endregion Public Methods
    }
}