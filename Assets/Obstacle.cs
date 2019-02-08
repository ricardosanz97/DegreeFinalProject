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
    public BoxCollider _boxCol;
    #endregion

    #region private
    #endregion

    private void Awake()
    {
        boogiesSpawner = FindObjectOfType<BoogiesSpawner>();
        _boxCol = GetComponent<BoxCollider>();
    }

    public void OnMouseDown()
    {
        if (boogiesSpawner.selectingObjective)
        {
            boogiesSpawner.obstacleSelected = this;
            boogiesSpawner.selectingObjective = false;
        }
    }
}
