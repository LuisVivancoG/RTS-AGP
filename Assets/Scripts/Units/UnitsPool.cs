using System;
using UnityEngine;
using UnityEngine.Pool;

namespace Pool
{
    public class UnitsPool : ObjectPool<UnitsBase>
    {
        public UnitsPool(
            Func<UnitsBase> createFunc,
            Action<UnitsBase> actionOnGet = null,
            Action<UnitsBase> actionOnRelease = null,
            Action<UnitsBase> actionOnDestroy = null,
            bool collectionCheck = true,
            int defaultCapacity = 10,
            int maxSize = 10000) : base(createFunc, actionOnGet, actionOnRelease, actionOnDestroy, collectionCheck, defaultCapacity, maxSize)
        {}
    }
}
