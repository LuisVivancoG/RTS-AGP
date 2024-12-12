using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownHall : PlacedBuildingBase
{
    //[SerializeField] private int[] _villagersUnitsMax;
    [SerializeField] private float _baseFoodPerCycle = 500;


    public override IEnumerator Destruction()
    {
        yield return base.Destruction();

        GameManager.Instance.BaseDestroyed(GameManager.Contestants.player);
    }
}
