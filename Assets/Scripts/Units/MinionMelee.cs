using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionMelee : UnitsBase
{

    public override void AnimAttack()
    {
        AudioManager.Instance.AttackSound(UnitType.BasicSoldier);
        base.AnimAttack();
    }

    internal override void CalculateDamage(int damageReceived)
    {
        base.CalculateDamage(damageReceived);

        if (_currentHP <= 0 && !_isDeath)
        {
            _isDeath = true;
            StartCoroutine(Death());
        }
    }

    IEnumerator Death()
    {
        AnimDeath();
        AudioManager.Instance.DeathSound(UnitType.BasicSoldier);
        yield return new WaitForSeconds(1.2f);
        this.gameObject.SetActive(false);
        _uManager.UnitDeath(this);
        _isDeath = false;
    }
}
