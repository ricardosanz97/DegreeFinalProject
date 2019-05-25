using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElixirStonePart : MonoBehaviour, ISaveable
{
    public long uniqueId;
    public string key;

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

    }

    public void Save()
    {

    }
}
