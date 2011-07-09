using System;
using System.Collections;
using System.Collections.Generic;

namespace Torshify.Core
{
    internal class WeakHandleDictionary<T> : Dictionary<IntPtr, WeakReference<T>>
        where T : class
    {
        #region Fields

        private float _lastGlobalMem;
        private int _lastHashCount;

        #endregion Fields

        #region Methods

        public void SetValue(IntPtr key, T value)
        {
            base[key] = new WeakReference<T>(value);
            ScavengeKeys();
        }

        public bool TryGetValue(IntPtr key, out T value)
        {
            WeakReference<T> weakValue;
            if (base.TryGetValue(key, out weakValue))
            {
                value = weakValue.Target;
                return weakValue.IsAlive;
            }

            value = null;
            return false;
        }

        /// <devdoc>
        ///     This method checks to see if it is necessary to
        ///     scavenge keys, and if it is it performs a scan
        ///     of all keys to see which ones are no longer valid.
        ///     To determine if we need to scavenge keys we need to
        ///     try to track the current GC memory.  Our rule of
        ///     thumb is that if GC memory is decreasing and our
        ///     key count is constant we need to scavenge.  We
        ///     will need to see if this is too often for extreme
        ///     use cases like the CompactFramework (they add
        ///     custom type data for every object at design time).
        /// </devdoc>
        private void ScavengeKeys()
        {
            int hashCount = Count;

            if (hashCount == 0)
            {
                return;
            }

            if (_lastHashCount == 0)
            {
                _lastHashCount = hashCount;
                return;
            }

            long globalMem = GC.GetTotalMemory(false);

            if (_lastGlobalMem == 0)
            {
                _lastGlobalMem = globalMem;
                return;
            }

            float memDelta = (globalMem - _lastGlobalMem) / (float)_lastGlobalMem;
            float hashDelta = (hashCount - _lastHashCount) / (float)_lastHashCount;

            if (memDelta < 0 && hashDelta >= 0)
            {
                // Perform a scavenge through our keys, looking
                // for dead references.
                List<IntPtr> cleanupList = null;
                foreach (IntPtr o in Keys)
                {
                    WeakReference<T> wr = base[o];
                    if (wr != null && !wr.IsAlive)
                    {
                        if (cleanupList == null)
                        {
                            cleanupList = new List<IntPtr>();
                        }

                        cleanupList.Add(o);
                    }
                }

                if (cleanupList != null)
                {
                    foreach (IntPtr o in cleanupList)
                    {
                        Remove(o);
                    }
                }
            }

            _lastGlobalMem = globalMem;
            _lastHashCount = hashCount;
        }

        #endregion Methods
    }
}