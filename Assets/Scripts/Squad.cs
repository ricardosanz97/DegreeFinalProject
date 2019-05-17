using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Squad", menuName = "Squad")]
public class Squad : ScriptableObject
{
    public string squadName;
    public List<SquadConfiguration.SQUAD_ROL> squadRol;
    public int numRows;
    public int numCols;
    public WrestlersConfiguration customConfiguration;
    //public int squadCost;
}
