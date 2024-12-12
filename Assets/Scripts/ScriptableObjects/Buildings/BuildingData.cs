using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "Create Scriptable Objects/Building Data")]

public class BuildingData : ScriptableObject
{
    [SerializeField] private float[] _maxHp;
    [SerializeField] private float _armor;
    [SerializeField] private GameObject _buildingGhostPrefab;
    [SerializeField] private PlacedBuildingBase _buildingPlacedPrefab;
    [SerializeField] private Sprite _buildingSprite;
    [SerializeField] private BuildingType _kindOfStructure;
    [SerializeField] private string _buildingName;
    [SerializeField] private int _buildingSize;
    [SerializeField] private int _obstacleLevel;


    public float[] MaxHp => _maxHp;
    public float Armor => _armor;
    public int BuildingSize => _buildingSize;
    public int ObstacleLevel => _obstacleLevel;
    public GameObject BuildingGhostPrefab => _buildingGhostPrefab;
    public PlacedBuildingBase BuildingPlacedPrefab => _buildingPlacedPrefab;
    public Sprite BuildingSprite => _buildingSprite;
    public BuildingType KindOfStructure => _kindOfStructure;

    public bool CanLevelUp(int currentLevel)
    {
        if (currentLevel <= 0)
            return false;

        bool isMaxLevel = currentLevel < _maxHp.Length;
        return !isMaxLevel;
    }
}

public enum BuildingType
{
    None = 0,
    TownHall = 1,
    PassiveResource = 2,
    ArmyCamp = 3,
    Gate = 4,
    ResearchLab = 5,
    Wall= 6,
}
