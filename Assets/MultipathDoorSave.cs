using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipathDoorSave : MonoBehaviour, ISaveable
{
    public string key;
    private void Awake()
    {
        
    }

    private void Start()
    {
        key = "MultipathObstacle" + GetComponentInParent<MultipathObstacle>().uniqueId.ToString() + this.gameObject.name + this.transform.parent.parent.GetComponent<PathBehavior>().PathIndex.ToString() + this.transform.parent.GetComponent<PathCorridorTrigger>().CorriderIndex;
    }

    private void OnEnable()
    {
        SaverManager.OnSaveData += Save;
        SaverManager.OnLoadData += Load;
    }
    public void Load()
    {
        this.transform.Find("left").transform.localRotation = SaverManager.I.saveData[key + "LeftRotation"];
        this.transform.Find("left").transform.localPosition = SaverManager.I.saveData[key + "LeftPosition"];

        this.transform.Find("right").transform.localRotation = SaverManager.I.saveData[key + "RightRotation"];
        this.transform.Find("right").transform.localPosition = SaverManager.I.saveData[key + "RightPosition"];
    }

    public void Save()
    {
        SaverManager.I.saveData.Add(key + "LeftRotation", transform.Find("left").transform.localRotation);
        SaverManager.I.saveData.Add(key + "LeftPosition", transform.Find("left").transform.localPosition);
        SaverManager.I.saveData.Add(key + "RightRotation", transform.Find("right").transform.localRotation);
        SaverManager.I.saveData.Add(key + "RightPosition", transform.Find("right").transform.localPosition);
    }
}
