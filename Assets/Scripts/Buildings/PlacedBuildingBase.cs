using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlacedBuildingBase : MonoBehaviour
{
    [SerializeField] private BuildingData _scriptedObjectData;
    [SerializeField] private Canvas _buildingCanvas;
    [SerializeField] private Slider _buildingSliderHp;
    public BuildingData _buildingData => _scriptedObjectData;

    internal float _currentHP;
    internal bool _isDestroyed;
    private int _buildingLvl;

    internal PlayerBuildingsManager Manager;
    protected Player Owner;

    public int Faction; //delete later

    private bool _isWalkable;

    private void OnEnable()
    {
        Initialize();
    }

    public void SetManager(PlayerBuildingsManager manager, ref Action onTick, Player owner)
    {
        Manager = manager;
        onTick += Tick;
        Owner = owner;
    }
    public int GetFaction()
    {
        return Owner.PlayerFaction;
    }

    public void ToggleCanvas()
    {
        if(_buildingCanvas.enabled)
        {
            _buildingCanvas.enabled = false;
        }
        else _buildingCanvas.enabled = true;
    }

    public virtual void CalculateDamage(float damageReceived)
    {
        //damageReceived -= _scriptedObjectData.Armor;
        TakeDamage(damageReceived);
    }

    public virtual void Initialize()
    {
        _isDestroyed = false;
        _buildingSliderHp.value = 1;
        _currentHP = _scriptedObjectData.MaxHp[0];
        _buildingLvl = 1;
    }

    private void TakeDamage(float damageTaken)
    {
        _currentHP -= damageTaken;
        _buildingSliderHp.value = ConvertAndClampHP();
        Debug.Log(ConvertAndClampHP());
    }

    private float ConvertAndClampHP()
    {
        float maxHP = _scriptedObjectData.MaxHp[0];
        return Mathf.Clamp(_currentHP/maxHP, 0f, 1f);
    }

    public virtual IEnumerator Destruction()
    {
        //AnimDeath();
        AudioManager.Instance.BuildingSound(AudioManager.BuildingAction.Dismantle);
        //explosionParticles and sound
        yield return new WaitForSeconds(2);
        //this.gameObject.SetActive(false);
        //_uManager.UnitDeath(this);
        _isDestroyed = false;
    }

    public void CanLvlUp()
    {
        _scriptedObjectData.CanLevelUp(_buildingLvl);
    }
    protected virtual void Tick()
    {

    }
    public virtual void OnPlaced() { }
    public virtual void OnRemoved() { }
}

