using UnityEngine;

[CreateAssetMenu(fileName = "AllUnitsData", menuName = "Create Scriptable Objects/All Units Data")]
public class AllUnitsData : ScriptableObject
{
    [SerializeField] private UnitsData[] _data;
    public UnitsData[] Data => _data;
}

