using System;
using UnityEngine;
using UnityEngine.Pool;

namespace Pool
{
    public class BuildingPool : ObjectPool<PlacedBuildingsBase>
    {
        public BuildingPool(
            Func<PlacedBuildingsBase> createFunc,
            Action<PlacedBuildingsBase> actionOnGet = null,
            Action<PlacedBuildingsBase> actionOnRelease = null,
            Action<PlacedBuildingsBase> actionOnDestroy = null,
            bool collectionCheck = true,
            int defaultCapacity = 10,
            int maxSize = 10000) : base(createFunc, actionOnGet, actionOnRelease, actionOnDestroy, collectionCheck, defaultCapacity, maxSize)
        {
            actionOnGet = getbuildingfrompool;
        }
    }
}
