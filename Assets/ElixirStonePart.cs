using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElixirStonePart : MonoBehaviour, ISaveable
{
    public long uniqueId;
    public string key;
    public BoogieCollector carriedBy;

    private void Awake()
    {
        uniqueId = GetHashCode();
    }

    public void AssignKey(string part)
    {
        key = part + "ElixirObstaclePart" + uniqueId.ToString();
    }

    public void Load()
    {
        try
        {
            carriedBy = SaverManager.I.saveData[key + "ElixirObstaclePart" + uniqueId.ToString() + "CarriedBy"];
        }
        catch
        {
            Destroy(this.gameObject);
        }
    }

    public void Save()
    {
        SaverManager.I.saveData.Add(key + "ElixirObstaclePart" + uniqueId.ToString() + "CarriedBy", carriedBy);
    }
}
