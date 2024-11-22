using System;
using UnityEngine;

public interface ISaveData
{
    public ISaveData OnSave(string dataToSerialized);
    public string OnLoad(ISaveData serializedData);
}

[Serializable()]
public class SavedBuildingData //: ISaveData
{
    public int CurrentHP;
    public int BuildingLevel;
    public int OwnerId;
    public Vector3 Position;
    public BuildingType KindOfType;

    //public string OnSave(SavedBuildingData dataToSerialized)
    //{
    //    string serialized = "";

    //    return serialized;
    //}

    //public ISaveData OnLoad(string serializedData)
    //{
    //    SavedBuildingData dataFromSave = new SavedBuildingData();

    //}
}
