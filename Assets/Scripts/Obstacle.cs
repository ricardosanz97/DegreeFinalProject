using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObstacleType
{
    Debris,
    Elixir,
    Multipath
}

public class Obstacle : MonoBehaviour, ISaveable
{
    #region public
    public BoogiesSpawner boogiesSpawner;
    public ObstacleType type;
    public Collider _col;
    public bool completed;
    #endregion

    #region private
    #endregion

    public virtual void Awake()
    {
        boogiesSpawner = FindObjectOfType<BoogiesSpawner>();
        if (GetComponent<Collider>() != null)
        {
            _col = GetComponent<Collider>();
        }
    }

    private void OnEnable()
    {
        SaverManager.OnLoadData += Load;
        SaverManager.OnSaveData += Save;
    }

    public virtual void Load()
    {
    }

    public virtual void Save()
    {
    }
}
