using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class UISquadTroopOptionsController : MonoBehaviour
{
    public Action callbackButtonMove;
    public Action callbackButtonFormation;

    public static UISquadTroopOptionsController Create(Action callbackButtonMove, Action callbackButtonFormation)
    {
        GameObject UISquadTroopOptions = Instantiate(Resources.Load("Prefabs/Popups/UISquadTroopOptions")) as GameObject;
        UISquadTroopOptionsController UISquadTroopOptionsController = UISquadTroopOptions.GetComponent<UISquadTroopOptionsController>();

        UISquadTroopOptionsController.callbackButtonMove = callbackButtonMove;
        UISquadTroopOptionsController.callbackButtonFormation = callbackButtonFormation;

        UISquadTroopOptions.SetActive(true);
        return UISquadTroopOptionsController;
    }

    public void ButtonMovePressed()
    {
        callbackButtonMove();
    }

    public void ButtonFormationPressed()
    {
        callbackButtonFormation();
    }

    public void ButtonCancelPressed()
    {
        this.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }
}
