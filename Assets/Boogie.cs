using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoogieType
{
    Cleaner,
    Wrestler,
    Explorer,
    MudThrower,
    Collector
}
public class Boogie : MonoBehaviour
{
    public BoogieType type;
    public Obstacle currentObjective;
    public void SetObjective(Obstacle obs)
    {
        currentObjective = obs;
    }
}
