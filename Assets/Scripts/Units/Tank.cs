using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : UnitsBase
{
    //[SerializeField] private Rigidbody _projectileRb;
    //[SerializeField] private Transform _projectileSpawner;
    //[SerializeField] private float _launchAngle;
    [SerializeField] private GameObject _deathParticles;

    internal override void CalculateDamage(int damageReceived)
    {
        base.CalculateDamage(damageReceived);

        if (_currentHP <= 0 && !_isDeath)
        {
            _isDeath = true;
            StartCoroutine(Death());
        }
    }

    public override void AnimAttack()
    {
        AudioManager.Instance.AttackSound(UnitType.Tank);
        base.AnimAttack();
    }

    IEnumerator Death()
    {
        AudioManager.Instance.DeathSound(UnitType.Tank);
        _deathParticles.SetActive (true);
        AnimDeath();
        yield return new WaitForSeconds(5);
        this.gameObject.SetActive(false);
        _uManager.UnitDeath(this);
        _isDeath = false;
        _deathParticles.SetActive(false);
    }
}
