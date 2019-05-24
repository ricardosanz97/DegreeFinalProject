using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElixirObstacle : Obstacle
{
    public ElixirObstacleStone[] elixirStones;
    public int totalElixirAvailable;
    public long uniqueId;
    public override void Awake()
    {
        base.Awake();
        elixirStones = GetComponentsInChildren<ElixirObstacleStone>();
        for (int i = 0; i<this.elixirStones.Length; i++)
        {
            elixirStones[i] = this.transform.GetChild(i).GetComponent<ElixirObstacleStone>();
            totalElixirAvailable += elixirStones[i].elixirAvailable;
            elixirStones[i].idStone = i;
        }

        uniqueId = GetHashCode() * Random.Range(1, 9);
        while (SaverManager.I.uniqueObstaclesIds.Contains(uniqueId))
        {
            uniqueId = GetHashCode() * Random.Range(1, 9);
        }
    }

    public override void Save()
    {
        base.Save();
        SaverManager.I.saveData.Add("elixirObstacleCompleted" + uniqueId, completed);
        SaverManager.I.saveData.Add("totalElixirAvailable" + uniqueId, totalElixirAvailable);
        for (int i = 0; i<elixirStones.Length; i++)
        {
            SaverManager.I.saveData.Add(uniqueId + "elixirStone" + elixirStones[i].idStone.ToString() + "ElixirAvailable", elixirStones[i].elixirAvailable);
            SaverManager.I.saveData.Add(uniqueId + "elixirStone" + elixirStones[i].idStone.ToString() + "Empty", elixirStones[i].empty);
            SaverManager.I.saveData.Add(uniqueId + "elixirStone" + elixirStones[i].idStone.ToString() + "BCollectorsIn", elixirStones[i].bCollectorsIn);
            SaverManager.I.saveData.Add(uniqueId + "elixirStone" + elixirStones[i].idStone.ToString() + "MaxCollectorsIn", elixirStones[i].maxCollectorsIn);
            SaverManager.I.saveData.Add(uniqueId + "elixirStone" + elixirStones[i].idStone.ToString() + "Type", elixirStones[i].type);
        }
    }

    public override void Load()
    {
        base.Load();
        completed = SaverManager.I.saveData["elixirObstacleCompleted" + uniqueId];
        totalElixirAvailable = SaverManager.I.saveData["totalElixirAvailable" + uniqueId];
        for (int i = 0; i<elixirStones.Length; i++)
        {
            elixirStones[i].elixirAvailable = SaverManager.I.saveData[uniqueId + "elixirStone" + elixirStones[i].idStone.ToString() + "ElixirAvailable"];
            elixirStones[i].empty = SaverManager.I.saveData[uniqueId + "elixirStone" + elixirStones[i].idStone.ToString() + "Empty"];
            elixirStones[i].bCollectorsIn = SaverManager.I.saveData[uniqueId + "elixirStone" + elixirStones[i].idStone.ToString() + "BCollectorsIn"];
            elixirStones[i].maxCollectorsIn = SaverManager.I.saveData[uniqueId + "elixirStone" + elixirStones[i].idStone.ToString() + "MaxCollectorsIn"];
            elixirStones[i].type = SaverManager.I.saveData[uniqueId + "elixirStone" + elixirStones[i].idStone.ToString() + "Type"];

        }
    }
}
