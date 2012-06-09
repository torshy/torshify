using System;
using System.Collections.Generic;

namespace Torshify.Core
{
    public class DelegateArray<T> : IArray<T>
    {
        #region Fields

        private Func<int> _getLength;
        private Func<int, T> _getIndex;

        #endregion Fields

        #region Constructors

        public DelegateArray(Func<int> getLength, Func<int, T> getIndex)
        {
            GetLength = getLength;
            GetIndex = getIndex;
        }

        #endregion Constructors

        #region Properties

        public int Count
        {
            get
            {
                return GetLength();
            }
        }

        protected Func<int> GetLength
        {
            get
            {
                return _getLength;
            }
            set
            {
                _getLength = value;
            }
        }

        protected Func<int, T> GetIndex
        {
            get
            {
                return _getIndex;
            }
            set
            {
                _getIndex = value;
            }
        }

        #endregion Properties

        #region Indexers

        public T this[int index]
        {
            get
            {
                if (index >= GetLength() || index < 0)
                {
                    throw new IndexOutOfRangeException();
                }

                return GetIndex(index);
            }
        }

        #endregion Indexers

        #region Public Methods

        public IEnumerator<T> GetEnumerator()
        {
            return new DelegateEnumerator(GetLength, GetIndex);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual IArray<TResult> Cast<TResult>()
        {
            return new DelegateArray<TResult>(GetLength, index => (TResult)Convert.ChangeType(GetIndex(index), typeof(TResult)));
        }

        #endregion Public Methods

        #region Nested Types

        public class DelegateEnumerator : IEnumerator<T>
        {
            #region Fields

            private Func<int> _getLength;
            private Func<int, T> _getIndex;
            private T _current;
            private int _currentIndex;
            private int _length;

            #endregion Fields

            #region Constructors

            public DelegateEnumerator(Func<int> getLength, Func<int, T> getIndex)
            {
                _getLength = getLength;
                _getIndex = getIndex;

                Reset();
            }

            #endregion Constructors

            #region Properties

            public T Current
            {
                get
                {
                    if (_currentIndex == -1 || _currentIndex == int.MaxValue)
                    {
                        throw new Exception("Before first element");
                    }

                    if (_current == null)
                    {
                        _current = _getIndex(_currentIndex);
                    }

                    return _current;
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }

            #endregion Properties

            #region Public Methods

            public void Dispose()
            {
                _getLength = null;
                _getIndex = null;
                _current = default(T);
            }

            public bool MoveNext()
            {
                _currentIndex++;
                _current = default(T);
                if (_currentIndex >= _length)
                {
                    _currentIndex = int.MaxValue;
                    return false;
                }

                return true;
            }

            public void Reset()
            {
                _currentIndex = -1;
                _current = default(T);
                _length = _getLength();
            }

            #endregion Public Methods
        }

        #endregion Nested Types
    }
}