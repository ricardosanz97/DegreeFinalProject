using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathBehavior : MonoBehaviour
{
    public bool FirstCorridor = false;
    public int CorridorCorrectIndex;
    public int PathIndex;
    public int LastCorridorCorrectIndex = -1;
    public GameObject nextInitCorridor;
    public CorridorBegin begin;
    public CorridorEnd end;
    public PathCorridorTrigger[] corridors;
    public int bExplorersIn = 0;

    public void AssignCorridorIndexs()
    {
        corridors = new PathCorridorTrigger[this.transform.childCount];
        for (int i = 0; i<this.transform.childCount; i++)
        {
            this.transform.GetChild(i).GetComponent<PathCorridorTrigger>().CorriderIndex = i;
            corridors[i] = this.transform.GetChild(i).GetComponent<PathCorridorTrigger>();
        }

        begin = this.transform.parent.GetComponentInChildren<CorridorBegin>();
        end = this.transform.parent.GetComponentInChildren<CorridorEnd>();

        AssignRandomCorrectCorridor();
    }

    private void AssignRandomCorrectCorridor()
    {
        int random = Random.Range(0, corridors.Length);
        CorridorCorrectIndex = corridors[random].CorriderIndex;
        corridors[random].Correct = true;
    }
}
