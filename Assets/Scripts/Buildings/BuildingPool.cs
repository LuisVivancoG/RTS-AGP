using System;
using UnityEngine;
using UnityEngine.Pool;

namespace Pool
{
    public class BuildingPool : ObjectPool<PlacedBuildingBase>
    {
        public BuildingPool(
            Func<PlacedBuildingBase> createFunc,
            Action<PlacedBuildingBase> actionOnGet = null,
            Action<PlacedBuildingBase> actionOnRelease = null,
            Action<PlacedBuildingBase> actionOnDestroy = null,
            bool collectionCheck = true,
            int defaultCapacity = 10,
            int maxSize = 10000) : base(createFunc, actionOnGet, actionOnRelease, actionOnDestroy, collectionCheck, defaultCapacity, maxSize)
        {}
    }
}
