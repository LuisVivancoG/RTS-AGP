using UnityEngine;

[CreateAssetMenu(fileName = "UnitsData", menuName = "Create Scriptable Objects/Units Data")]

public class UnitsData : ScriptableObject
{
    [SerializeField] private float _maxHp;
    [SerializeField] private int _armor;
    [SerializeField] private float _attack;
    [SerializeField] private int _attackRange;
    [SerializeField] private int _movementSpeed;
    [SerializeField] private UnitsBase _unitPrefab;
    [SerializeField] private Sprite _unitSprite;
    [SerializeField] private UnitType _kindOfUnit;
    [SerializeField] private string _unitName;

    public float MaxHp => _maxHp;
    public int Armor => _armor;
    public float Attack => _attack;
    public int AttackRange => _attackRange;
    public int MovementSpeed => _movementSpeed;
    public UnitsBase UnitPrefab => _unitPrefab;
    public Sprite UnitSprite => _unitSprite;
    public UnitType KindOfUnit => _kindOfUnit;
}

public enum UnitType
{
    None = 0,
    Farmer = 1,
    BasicSoldier = 2,
    WaterResistance = 3,
    RangeAttack = 4,
    Tank = 5,
}
