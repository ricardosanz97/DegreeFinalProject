using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisObstaclePart : InteractableBody
{
    public int idPart;

    public BoogieCleaner charger;
    public bool settledDown;
    [HideInInspector]public float initialY;
   
    private void Awake()
    {
        initialY = this.transform.position.y;
    }
}
