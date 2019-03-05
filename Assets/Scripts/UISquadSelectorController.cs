using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISquadSelectorController : GenericPanelController
{
    public int currentFormation = 0;
    public Button contentionButton;
    public Button penetrationButton;
    public Button aroundPlayerButton;

    public static UISquadSelectorController Create(int currentFormation = 0)
    {
        if (FindObjectOfType<UISquadSelectorController>() != null)
        {
            Destroy(FindObjectOfType<UISquadSelectorController>().gameObject);
            return null;
        }
        GameObject UISquadSelector = Instantiate(Resources.Load("Prefabs/Popups/UISelectorSquad")) as GameObject;
        UISquadSelectorController UISquadSelectorController = UISquadSelector.GetComponent<UISquadSelectorController>();
        UISquadSelectorController.currentFormation = currentFormation;
        UISquadSelectorController.HandleButtonsState();
        UISquadSelectorController.transform.SetParent(GameObject.Find("MainCanvas").transform, false);
        UISquadSelectorController.OpenPanel();
        return UISquadSelectorController;
    }

    public void ButtonContentionSquadPressed()
    {
        if (currentFormation == 0)
        {
            UIController.I.HandleSpawnSquadLogic();
        }
        else
        {
            BoogiesSpawner.CommanderSelected.ChangeSquadFormation(SquadConfiguration.SQUAD_FORMATION.Contention);
        }
        ClosePanel();
        FindObjectOfType<BoogiesSpawner>().currentFormationSelected = 1;

    }

    public void ButtonPenetrationSquadPressed()
    {
        if (currentFormation == 0)
        {
            UIController.I.HandleSpawnSquadLogic();
        }
        else
        {
            BoogiesSpawner.CommanderSelected.ChangeSquadFormation(SquadConfiguration.SQUAD_FORMATION.Penetration);
        }
        ClosePanel();
        FindObjectOfType<BoogiesSpawner>().currentFormationSelected = 2;
    }

    public void ButtonAroundPlayerSquadPressed()
    {
        if (currentFormation == 0)
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
    }

    private void HandleButtonsState()
    {
        switch (currentFormation)
        {
            case 1:
                contentionButton.enabled = false;
                break;
            case 2:
                penetrationButton.enabled = false;
                break;
            case 3:
                aroundPlayerButton.enabled = false;
                break;
        }
    }
}
