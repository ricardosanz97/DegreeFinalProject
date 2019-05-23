using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorSave : MonoBehaviour, ISaveable
{
    public void Load()
    {
        this.transform.localPosition = SaverManager.I.saveData[this.gameObject.name + "position"];
    }

    public void Save()
    {
        SaverManager.I.saveData.Add(this.gameObject.name + "position", this.transform.localPosition);
    }

    private void OnEnable()
    {
        SaverManager.OnLoadData += Load;
        SaverManager.OnSaveData += Save;
    }
}
