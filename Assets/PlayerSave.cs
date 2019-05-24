using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerSave : MonoBehaviour, ISaveable
{
    BoogiesSpawner _boogiesSpawner;
    PlayerHealth _playerHealth;
    Animator _anim;
    NavMeshAgent _agent;
    private void Awake()
    {
        _boogiesSpawner = GetComponent<BoogiesSpawner>();
        _agent = GetComponent<NavMeshAgent>();
        _playerHealth = GetComponent<PlayerHealth>();
        _anim = GetComponent<Animator>();
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

        Debug.Log("player state loaded = " + SaverManager.I.saveData["playerAnimState"]);
        _anim.SetInteger("state", SaverManager.I.saveData["playerAnimState"]);

        _boogiesSpawner.cleanersSpawned = SaverManager.I.saveData["cleanersSpawned"];
        _boogiesSpawner.collectorsSpawned = SaverManager.I.saveData["collectorsSpawned"];
        _boogiesSpawner.explorersSpawned = SaverManager.I.saveData["explorersSpawned"];
        _boogiesSpawner.cleanersBackButtonPressed = SaverManager.I.saveData["cleanersBackButtonPressed"];
        _boogiesSpawner.collectorsBackButtonPressed = SaverManager.I.saveData["collectorsBackButtonPressed"];
        _boogiesSpawner.explorersBackButtonPressed = SaverManager.I.saveData["explorersBackButtonPressed"];
        _boogiesSpawner.cleanersAmount = SaverManager.I.saveData["cleanersAmount"];
        _boogiesSpawner.explorersAmount = SaverManager.I.saveData["explorersAmount"];
        _boogiesSpawner.collectorsAmount = SaverManager.I.saveData["collectorsAmount"];
        _boogiesSpawner.leftBoogiesAmount = SaverManager.I.saveData["leftAmount"];
        _boogiesSpawner.currentAmount = SaverManager.I.saveData["totalAmount"];
        _boogiesSpawner.canSpawnBoogies = SaverManager.I.saveData["canSpawnBoogies"];
        _boogiesSpawner.canSpawnSquad = SaverManager.I.saveData["canSpawnSquad"];
        _boogiesSpawner.currentFormationSelected = SaverManager.I.saveData["currentFormationSelected"];
        _boogiesSpawner.boogiesCleanersKnowCollectorMachinePosition = SaverManager.I.saveData["boogiesCleanersKnow"];
        _boogiesSpawner.selectorEnabled = SaverManager.I.saveData["selectorEnabled"];
        _playerHealth.health = SaverManager.I.saveData["playerHealth"];
        _playerHealth.alive = SaverManager.I.saveData["playerAlive"];
        _playerHealth.team = SaverManager.I.saveData["playerTeam"];
        _boogiesSpawner.collectorsConfig = SaverManager.I.saveData["collectorsConfig"];
        _boogiesSpawner.explorersConfig = SaverManager.I.saveData["explorersConfig"];
        _boogiesSpawner.cleanersConfig = SaverManager.I.saveData["cleanersConfig"];
    }

    public void Save()
    {
        SaverManager.I.saveData.Add("playerPosition", this.transform.position);
        SaverManager.I.saveData.Add("playerRotation", this.transform.rotation);
        //SaverManager.I.saveData.Add("playerBoogiesSpawner", _boogiesSpawner);

        SaverManager.I.saveData.Add("playerAnimState", _anim.GetInteger("state"));

        SaverManager.I.saveData.Add("cleanersSpawned", _boogiesSpawner.cleanersSpawned);
        SaverManager.I.saveData.Add("collectorsSpawned", _boogiesSpawner.collectorsSpawned);
        SaverManager.I.saveData.Add("explorersSpawned", _boogiesSpawner.explorersSpawned);
        SaverManager.I.saveData.Add("cleanersBackButtonPressed", _boogiesSpawner.cleanersBackButtonPressed);
        SaverManager.I.saveData.Add("collectorsBackButtonPressed", _boogiesSpawner.collectorsBackButtonPressed);
        SaverManager.I.saveData.Add("explorersBackButtonPressed", _boogiesSpawner.explorersBackButtonPressed);
        SaverManager.I.saveData.Add("cleanersAmount", _boogiesSpawner.cleanersAmount);
        SaverManager.I.saveData.Add("explorersAmount", _boogiesSpawner.explorersAmount);
        SaverManager.I.saveData.Add("collectorsAmount", _boogiesSpawner.collectorsAmount);
        SaverManager.I.saveData.Add("leftAmount", _boogiesSpawner.leftBoogiesAmount);
        SaverManager.I.saveData.Add("totalAmount", _boogiesSpawner.currentAmount);
        SaverManager.I.saveData.Add("canSpawnBoogies", _boogiesSpawner.canSpawnBoogies);
        SaverManager.I.saveData.Add("canSpawnSquad", _boogiesSpawner.canSpawnSquad);
        SaverManager.I.saveData.Add("currentFormationSelected", _boogiesSpawner.currentFormationSelected);
        SaverManager.I.saveData.Add("boogiesCleanersKnow", _boogiesSpawner.boogiesCleanersKnowCollectorMachinePosition);
        SaverManager.I.saveData.Add("selectorEnabled", _boogiesSpawner.selectorEnabled);
        SaverManager.I.saveData.Add("playerHealth", _playerHealth.health);
        SaverManager.I.saveData.Add("playerAlive", _playerHealth.alive);
        SaverManager.I.saveData.Add("playerTeam", _playerHealth.team);
        SaverManager.I.saveData.Add("collectorsConfig", _boogiesSpawner.collectorsConfig);
        SaverManager.I.saveData.Add("explorersConfig", _boogiesSpawner.explorersConfig);
        SaverManager.I.saveData.Add("cleanersConfig", _boogiesSpawner.cleanersConfig);
    }
}
