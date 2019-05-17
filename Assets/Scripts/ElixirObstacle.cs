using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElixirObstacle : Obstacle
{
    public ElixirObstacleStone[] elixirStones;
    public int totalElixirAvailable;
    public override void Awake()
    {
        base.Awake();

        elixirStones = new ElixirObstacleStone[this.transform.childCount];
        for (int i = 0; i<this.transform.childCount; i++)
        {
            if (this.transform.GetChild(i).GetComponent<ElixirObstacleStone>())
            {
                elixirStones[i] = this.transform.GetChild(i).GetComponent<ElixirObstacleStone>();
                totalElixirAvailable += elixirStones[i].elixirAvailable;
            }
        }
    }
}
