using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSave : MonoBehaviour, ISaveable
{
    CameraBehaviour _camera;
    private void Awake()
    {
        _camera = GetComponent<CameraBehaviour>();
    }

    private void OnEnable()
    {
        SaverManager.OnSaveData += Save;
        SaverManager.OnLoadData += Load;
    }
    public void Load()
    {
        _camera.enabled = false;
        this.transform.position = SaverManager.I.saveData["cameraPosition"];
        this.transform.rotation = SaverManager.I.saveData["cameraRotation"];
        _camera.enabled = true;
    }

    public void Save()
    {
        SaverManager.I.saveData.Add("cameraPosition", this.transform.position);
        SaverManager.I.saveData.Add("cameraRotation", this.transform.rotation);
    }
}
