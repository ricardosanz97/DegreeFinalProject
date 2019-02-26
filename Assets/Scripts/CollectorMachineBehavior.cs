using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectorMachineBehavior : MonoBehaviour
{
    public int totalElixir;
    public static int ElixirGot
    {
        get { return FindObjectOfType<CollectorMachineBehavior>().totalElixir; }
        set { FindObjectOfType<CollectorMachineBehavior>().totalElixir = value; }
    }
}
