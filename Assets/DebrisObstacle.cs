using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisObstacle : Obstacle
{
    public DebrisObstaclePart[] debrisParts;
    public override void Awake()
    {
        base.Awake();

        debrisParts = new DebrisObstaclePart[this.transform.childCount];
        for (int i = 0; i<this.transform.childCount; i++)
        {
            debrisParts[i] = this.transform.GetChild(i).GetComponent<DebrisObstaclePart>();
        }
    }
}
