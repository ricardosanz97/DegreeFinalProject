using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebrisObstacle : Obstacle
{
    public DebrisObstaclePart[] debrisParts;
    public DebrisObstaclePart[] debrisPotentialParts;
    public long uniqueId;
    public override void Awake()
    {
        base.Awake();

        debrisParts = new DebrisObstaclePart[this.transform.childCount];
        for (int i = 0; i<this.transform.childCount; i++)
        {
            debrisParts[i] = this.transform.GetChild(i).GetComponent<DebrisObstaclePart>();
            debrisParts[i].idPart = i;
        }

        uniqueId = GetHashCode() * Random.Range(1, 9);
    }

    private void Start()
    {
        ResetSettledDownDebris();
    }

    private void ResetSettledDownDebris()
    {
        foreach (DebrisObstaclePart dop in debrisParts)
        {
            dop.settledDown = false;
        }

        debrisPotentialParts[Random.Range(0, debrisPotentialParts.Length)].settledDown = true;
    }

    public override void Save()
    {
        base.Save();
        SaverManager.I.saveData.Add("debrisObstacleCompleted" + uniqueId, completed);
        for (int i = 0; i<debrisParts.Length; i++)
        {
            SaverManager.I.saveData.Add(uniqueId + "debrisPart" + debrisParts[i].idPart.ToString() + "Position", debrisParts[i].transform.position);
            SaverManager.I.saveData.Add(uniqueId + "debrisPart" + debrisParts[i].idPart.ToString() + "Rotation", debrisParts[i].transform.rotation);
            SaverManager.I.saveData.Add(uniqueId + "debrisPart" + debrisParts[i].idPart.ToString() + "Charger", debrisParts[i].charger);
            SaverManager.I.saveData.Add(uniqueId + "debrisPart" + debrisParts[i].idPart.ToString() + "SettledDown", debrisParts[i].settledDown);
        }
    }

    public override void Load()
    {
        base.Load();
        completed = SaverManager.I.saveData["debrisObstacleCompleted" + uniqueId];
        for (int i = 0; i<debrisParts.Length; i++)
        {
            debrisParts[i].transform.position = SaverManager.I.saveData[uniqueId + "debrisPart" + debrisParts[i].idPart.ToString() + "Position"];
            debrisParts[i].transform.rotation = SaverManager.I.saveData[uniqueId + "debrisPart" + debrisParts[i].idPart.ToString() + "Rotation"];
            debrisParts[i].charger = SaverManager.I.saveData[uniqueId + "debrisPart" + debrisParts[i].idPart.ToString() + "Charger"];
            debrisParts[i].settledDown = SaverManager.I.saveData[uniqueId + "debrisPart" + debrisParts[i].idPart.ToString() + "SettledDown"];
        }
    }
}
