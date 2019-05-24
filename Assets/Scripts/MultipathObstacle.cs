using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipathObstacle : Obstacle
{
    public long uniqueId;

    public override void Awake()
    {
        base.Awake();
        uniqueId = GetHashCode() * Random.Range(1, 9);
        while (SaverManager.I.uniqueObstaclesIds.Contains(uniqueId))
        {
            uniqueId = GetHashCode() * Random.Range(1, 9);
        }
    }
}
