using System.Collections.Generic;

namespace Torshify
{
    public interface IArray<T> : IEnumerable<T>
    {
        #region Properties

        int Count
        {
            get;
        }

        T this[int index]
        {
            get;
        }

        #endregion Properties

        #region Methods

        IArray<TResult> Cast<TResult>();

        #endregion Methods
    }
}