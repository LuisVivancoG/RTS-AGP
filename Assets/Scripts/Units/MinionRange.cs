using System.Collections;
using UnityEngine;

public class MinionRange : UnitsBase
{
    [SerializeField] private Rigidbody _projectileRb;
    [SerializeField] private Transform _projectileSpawner;
    [SerializeField] private float _launchAngle;

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
        AudioManager.Instance.DeathSound(UnitType.RangeAttack);
        yield return new WaitForSeconds(1.1f);
        this.gameObject.SetActive(false);
        _uManager.UnitDeath(this);
        _isDeath = false;
    }

    public void RangeAttack(AnimationEvent triggerEvent)
    {
        AudioManager.Instance.AttackSound(UnitType.RangeAttack);

        _projectileRb.position = _projectileSpawner.position;
        _projectileRb.velocity = Vector3.zero;
        _projectileRb.gameObject.SetActive(true);

        Vector3 direction = CellUnit.EnemyUnit.position - _projectileSpawner.position;
        float horizontalDistance = new Vector2(direction.x, direction.z).magnitude;
        float verticalDistance = direction.y;

        float g = Physics.gravity.y * -1;
        float angleRad = _launchAngle * Mathf.Deg2Rad;

        float v = Mathf.Sqrt((g * Mathf.Pow(horizontalDistance, 2)) / (2 * (horizontalDistance * Mathf.Tan(angleRad) - verticalDistance)));

        float vX = v * Mathf.Cos(angleRad);
        float vY = v * Mathf.Sin(angleRad);
        Vector3 velocity = new Vector3(direction.x, 0, direction.z).normalized * vX + Vector3.up * vY;

        _projectileRb.velocity = velocity;
    }
}
