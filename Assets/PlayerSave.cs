using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerSave : MonoBehaviour, ISaveable
{
    BoogiesSpawner _boogiesSpawner;
    NavMeshAgent _agent;
    private void Awake()
    {
        _boogiesSpawner = GetComponent<BoogiesSpawner>();
        _agent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        SaverManager.OnSaveData += Save;
        SaverManager.OnLoadData += Load;
    }

    public void Load()
    {
        _agent.enabled = false;
        this.transform.position = SaverManager.I.saveData["playerPosition"];
        this.transform.rotation = SaverManager.I.saveData["playerRotation"];
        _agent.enabled = true;
    }

    public void Save()
    {
        SaverManager.I.saveData.Add("playerPosition", this.transform.position);
        SaverManager.I.saveData.Add("playerRotation", this.transform.rotation);
        SaverManager.I.saveData.Add("playerBoogiesSpawner", _boogiesSpawner);
    }
}
