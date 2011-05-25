using System.Collections.Generic;

namespace Torshify
{
    public interface IArray<T> : IEnumerable<T>
    {
        int Count { get; }
        T this[int index] { get; }
        IArray<TResult> Cast<TResult>();
    }
}
