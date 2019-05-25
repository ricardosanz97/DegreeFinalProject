using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClueBehavior : InteractableBody, ISaveable
{
    public BoogieExplorer carriedBy;
    public BoogieExplorer placedBy;
    public bool placed = false;
    public int corridorIndex = -1;
    public int pathIndex = -1;

    public void Load()
    {

    }

    public void Save()
    {

    }
}
