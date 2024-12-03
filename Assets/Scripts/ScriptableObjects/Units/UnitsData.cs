using UnityEngine;

[CreateAssetMenu(fileName = "UnitsData", menuName = "Create Scriptable Objects/Units Data")]

public class UnitsData : ScriptableObject
{
    [SerializeField] private float _maxHp;
    //[SerializeField] private int _armor;
    [SerializeField] private float _attack;
    [Range(1, 10)]
    [SerializeField] private int _attackRange;
    [Range(1, 10)]
    [SerializeField] private int _visionRange;
    [SerializeField] private int _movementSpeed;
    [SerializeField] private UnitsBase _unitPrefab;
    [SerializeField] private Sprite _unitSprite;
    [SerializeField] private UnitType _kindOfUnit;

    public float MaxHp => _maxHp;
    //public int Armor => _armor;
    public float Attack => _attack;
    public int AttackRange => _attackRange;
    public int VisionRange => _visionRange;
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
