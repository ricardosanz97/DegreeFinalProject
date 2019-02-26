using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObstacleType
{
    Debris,
    Enemy
}

public class Obstacle : MonoBehaviour
{
    #region public
    public BoogiesSpawner boogiesSpawner;
    public ObstacleType type;
    public Collider _col;
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
}
