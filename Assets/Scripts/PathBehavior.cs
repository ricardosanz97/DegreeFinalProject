using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathBehavior : MonoBehaviour, ISaveable
{
    public bool FirstPath = false;
    public int CorridorCorrectIndex;
    public int PathIndex;
    public int LastCorridorCorrectIndex = -1;
    public GameObject nextInitCorridor;
    public CorridorBegin begin;
    public CorridorEnd end;
    public PathCorridorTrigger[] corridors;
    public int bExplorersIn = 0;

    private void OnEnable()
    {
        SaverManager.OnSaveData += Save;
        SaverManager.OnLoadData += Load;
    }

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

    public void Load()
    {
        long uniqueId = GetComponentInParent<MultipathObstacle>().uniqueId;
        string key = "MultipathObstacle" + uniqueId + this.PathIndex.ToString() + "InitialDoorsOpened";
        begin.doorsOpened = SaverManager.I.saveData[key];
        foreach (PathCorridorTrigger pct in corridors)
        {
            pct.doorOpened = SaverManager.I.saveData["MultipathObstacle" + GetComponentInParent<MultipathObstacle>().uniqueId.ToString() + this.PathIndex.ToString() + pct.CorriderIndex.ToString() + "DoorOpened"];
        }
    }

    public void Save()
    {
        long uniqueId = GetComponentInParent<MultipathObstacle>().uniqueId;
        string key = "MultipathObstacle" + uniqueId + this.PathIndex.ToString() + "InitialDoorsOpened";
        //Debug.Log(key);
        SaverManager.I.saveData.Add(key, begin.doorsOpened);
        foreach (PathCorridorTrigger pct in corridors)
        {
            SaverManager.I.saveData.Add("MultipathObstacle" + GetComponentInParent<MultipathObstacle>().uniqueId.ToString() + this.PathIndex.ToString() +
            pct.CorriderIndex.ToString() + "DoorOpened", pct.doorOpened);
        }
    }

    private void AssignRandomCorrectCorridor()
    {
        int random = Random.Range(0, corridors.Length);
        CorridorCorrectIndex = corridors[random].CorriderIndex;
        corridors[random].Correct = true;
    }
}
