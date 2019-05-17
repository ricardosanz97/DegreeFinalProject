using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisObstacle : Obstacle
{
    public DebrisObstaclePart[] debrisParts;
    public DebrisObstaclePart[] debrisPotentialParts;
    public override void Awake()
    {
        base.Awake();

        debrisParts = new DebrisObstaclePart[this.transform.childCount];
        for (int i = 0; i<this.transform.childCount; i++)
        {
            debrisParts[i] = this.transform.GetChild(i).GetComponent<DebrisObstaclePart>();
        }
    }

    private void Start()
    {
        ResetSettledDownDebris();
    }

    private void ResetSettledDownDebris()
    {
        foreach (DebrisObstaclePart dop in debrisParts)
        {
            dop.settledDown = false;
        }

        debrisPotentialParts[Random.Range(0, debrisPotentialParts.Length)].settledDown = true;
    }
}
