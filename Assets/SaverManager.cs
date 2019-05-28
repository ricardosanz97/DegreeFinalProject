﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SaverManager : Singleton<SaverManager>
{
    public GAME_STEP currentGameStep;

    public Dictionary<string, dynamic> saveData = new Dictionary<string, dynamic>();
    public List<long> uniqueBoogiesIds = new List<long>();
    public List<long> uniqueObstaclesIds = new List<long>();
    public static event Action OnSaveData;
    public static event Action OnLoadData;

    public void LoadLastSavedState()
    {
        Debug.Log("state loaded");
        OnLoadData.Invoke();
    }

    public void SaveState(bool force = false)
    {
        if (!force)
        {
            if (FindObjectOfType<BoogiesSpawner>().BoogiesActing())
            {
                Debug.Log("Can't save because boogies are acting.");
                return;
            }
        }
        saveData.Clear();
        Debug.Log("state saved");
        OnSaveData.Invoke();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            saveData.Clear();
            Debug.Log("data saved");
            OnSaveData.Invoke();
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("loading data");
            OnLoadData.Invoke();
        }
    }

    
}
