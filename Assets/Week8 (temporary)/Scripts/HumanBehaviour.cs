using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanBehaviour : MonoBehaviour
{
    [SerializeField] private Transform _weaponSocket;
    public void AttackLand()
    {
        Debug.Log("Attak landed"); 
    }

    public void AttackCheck(int range)
    {
        Debug.Log("Pew " + range);
    }

    // string, int, float, bool, 
    public void AttackWithObject(GameObject weapon)
    {
        if (weapon != null)
        {
            var particle = Instantiate(weapon, _weaponSocket);
        }
        else
        {
            Debug.Log("no weapon assigned");
        }
    }

    public void OnAnimationEvent(AnimationEvent theTriggerinEvent)
    {
        //theTriggerinEvent.
    }
}
