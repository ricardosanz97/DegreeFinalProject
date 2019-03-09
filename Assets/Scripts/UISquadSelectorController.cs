﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISquadSelectorController : GenericPanelController
{
    public List<SquadConfiguration.SQUAD_ROL> formation;
    public Button contentionButton;
    public Button penetrationButton;
    public Button aroundPlayerButton;

    public static UISquadSelectorController Create(List<SquadConfiguration.SQUAD_ROL> squadFormation)
    {
        if (FindObjectOfType<UISquadSelectorController>() != null)
        {
            Destroy(FindObjectOfType<UISquadSelectorController>().gameObject);
            return null;
        }
        GameObject UISquadSelector = Instantiate(Resources.Load("Prefabs/Popups/UISelectorSquad")) as GameObject;
        UISquadSelectorController UISquadSelectorController = UISquadSelector.GetComponent<UISquadSelectorController>();
        UISquadSelectorController.formation = squadFormation;
        UISquadSelectorController.HandleButtons();
        UISquadSelectorController.transform.SetParent(GameObject.Find("MainCanvas").transform, false);
        UISquadSelectorController.OpenPanel();
        return UISquadSelectorController;
    }

    public void ButtonContentionSquadPressed()
    {
        if (formation == null)
        {
            UIController.I.HandleSpawnSquadLogic();
        }
        else
        {
            BoogiesSpawner.CommanderSelected.ChangeSquadFormation(SquadConfiguration.SQUAD_FORMATION.Contention);
        }
        ClosePanel();
        Destroy(this.gameObject);
        FindObjectOfType<BoogiesSpawner>().currentFormationSelected = 1;

    }

    public void ButtonPenetrationSquadPressed()
    {
        if (formation == null)
        {
            UIController.I.HandleSpawnSquadLogic();
        }
        else
        {
            BoogiesSpawner.CommanderSelected.ChangeSquadFormation(SquadConfiguration.SQUAD_FORMATION.Penetration);
        }
        ClosePanel();
        Destroy(this.gameObject);
        FindObjectOfType<BoogiesSpawner>().currentFormationSelected = 2;
    }

    public void ButtonAroundPlayerSquadPressed()
    {
        if (formation == null)
        {
            UIController.OnGroundClicked -= UIController.I.OnSpawnSquadPositionSelected;
            FindObjectOfType<BoogiesSpawner>().CreateSquad(3, FindObjectOfType<BoogiesSpawner>().transform.position);
        }
        else
        {
            BoogiesSpawner.CommanderSelected.ChangeSquadFormation(SquadConfiguration.SQUAD_FORMATION.AroundPlayer);
        }
        FindObjectOfType<BoogiesSpawner>().currentFormationSelected = 3;
        ClosePanel();
        Destroy(this.gameObject);
    }

    public void HandleButtons()
    {

        if (formation != null)
        {
            if (SquadConfiguration.ListsAreEquals(formation, FindObjectOfType<SquadConfiguration>().contentionSquad.listFormation))
            {
                contentionButton.gameObject.SetActive(false);
            }
            else if (SquadConfiguration.ListsAreEquals(formation, FindObjectOfType<SquadConfiguration>().penetrationSquad.listFormation))
            {
                penetrationButton.gameObject.SetActive(false);
            }
            else if (SquadConfiguration.ListsAreEquals(formation, FindObjectOfType<SquadConfiguration>().aroundPlayerSquad.listFormation))
            {
                aroundPlayerButton.gameObject.SetActive(false);
            }

            else
            {
                contentionButton.gameObject.SetActive(true);
                penetrationButton.gameObject.SetActive(true);
                aroundPlayerButton.gameObject.SetActive(true);
            }
        }

    }
}
