using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisObstaclePart : MonoBehaviour
{
    public BoogieCleaner charger;
    public bool settledDown;
    private void Update()
    {
        if (charger != null)
        {
            transform.position = charger.gameObject.transform.Find("ChargingPoint").transform.position;
        }
    }
}
