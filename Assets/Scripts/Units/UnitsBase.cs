using System;
using UnityEngine;

public class UnitsBase : MonoBehaviour //Base class for units. It tracks current stats of the unit on the game
                                       //Stats are overwritten by scripted data of the unit depending on their type
{
    [SerializeField] private UnitsData _scriptedObjectData;
    public UnitsData UnitData => _scriptedObjectData;

    private float _currentHP;

    protected UnitsManager Manager;
    protected int Faction = 1; //change this to Player class
    protected GameGrid GameGrid;

    private CellUnit _cellUnit;
    public CellUnit CellUnit => _cellUnit;

    private void Start()
    {
        _currentHP = UnitData.MaxHp;
    }
    /*/*internal void SetManager(UnitsManager manager, GameGrid grid, int faction) //pass player class instead of int
    {
        Manager = manager;
        Faction = faction;
        GameGrid = grid;
    }
    public int GetFaction()
    {
        return Faction; //.PlayerFaction;
    }
    public void CalculateDamage(int damageReceived)
    {
        damageReceived -= UnitData.Armor;
        TakeDamage(damageReceived);
    }
    private void TakeDamage(int damageTaken)
    {
        _currentHP -= damageTaken;
    }

    public void CanLvlUp()
    {
    }
    protected virtual void Tick()
    {

    }
    public virtual void OnPlaced() { }
    public virtual void OnRemoved() { }*/
}

