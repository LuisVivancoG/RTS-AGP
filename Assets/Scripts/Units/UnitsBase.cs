using System;
using UnityEngine;
using UnityEngine.UI;

public class UnitsBase : MonoBehaviour //Base class for units. It tracks current stats of the unit on the game
                                       //Stats are overwritten by scripted data of the unit depending on their type
{
    [SerializeField] private UnitsData _scriptedObjectData;
    [SerializeField] private Animator _anim;
    [SerializeField] private CellUnit _cellUnit;
    [SerializeField] private Canvas _unitCanvas;
    [SerializeField] private Slider _hpSlider;
    [SerializeField] private GameObject _targetedImg;
    public CellUnit CellUnit => _cellUnit;
    public UnitsData UnitData => _scriptedObjectData;

    internal float _currentHP;

    internal UnitsManager _uManager;

    internal bool _isDeath;

    private void Start()
    {
        _cellUnit.SetData(this, AnimMovement, AnimIdle, CalculateDamage);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CalculateDamage(4);
        }
    }

    internal void SetUnitManager(UnitsManager uManager)
    {
        _uManager = uManager;
    }

    private void OnEnable()
    {
        _currentHP = UnitData.MaxHp;
        _hpSlider.value = 1;
    }

    public void OnSelected()
    {
        _anim.SetTrigger("Selected");
        _unitCanvas.enabled = true;
        _targetedImg.SetActive(true);
    }

    public void OnDeselect()
    {
        _unitCanvas.enabled = false;
        _targetedImg.SetActive(false);
    }

    void AnimMovement()
    {
        _anim.SetBool("IsMoving", true);
    }

    void AnimIdle()
    {
        _anim.SetBool("IsMoving", false);
    }

    public virtual void AnimAttack()
    {
        _anim.SetTrigger("Attacking");
    }

    internal void AnimDeath()
    {
        _anim.SetTrigger("Death");
    }

    
    internal virtual void CalculateDamage(int damageReceived)
    {
        //damageReceived -= UnitData.Armor;
        TakeDamage(damageReceived);
        //Debug.Log(_currentHP + "hp left");
    }
    private void TakeDamage(int damageTaken)
    {
        _currentHP -= damageTaken;
        _hpSlider.value = ConvertAndClampHP();
    }

    private float ConvertAndClampHP()
    {
        return Mathf.Clamp(_currentHP / UnitData.MaxHp, 0, 1);
    }

    /*public void CanLvlUp()
    {
    }*/
    /*protected virtual void Tick()
    {

    }*/
}

