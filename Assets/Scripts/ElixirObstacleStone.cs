using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElixirObstacleStone : InteractableBody
{
    public int elixirAvailable = 5;
    public int maxCollectorsIn = 3;
    public bool empty = false;
    public int bCollectorsIn = 0;
    public TYPE type;
    public enum TYPE
    {
        Elixir,
        Energy,
        None
    }

    private void Update()
    {
        if (elixirAvailable == 0)
        {
            this.transform.GetChild(0).GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
        }
    }
}
