using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UISquadSelectorController : GenericPanelController
{
    public List<SquadConfiguration.SQUAD_ROL> formation;
    public Button contentionButton;
    public Button penetrationButton;
    public Button aroundPlayerButton;
    public Button customButton;
    public Transform parent;

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

        ScriptableObject[] customSquads = Resources.LoadAll<ScriptableObject>("Squads/");
        foreach (ScriptableObject customSquad in customSquads)
        {
            Squad newSquad = AssetDatabase.LoadAssetAtPath("Assets/Resources/Squads/" + customSquad.name + ".asset" , typeof(ScriptableObject)) as Squad;

            GameObject squadGO = Instantiate(UISquadSelectorController.customButton.gameObject);
            squadGO.transform.SetParent(UISquadSelectorController.parent, false);
            squadGO.GetComponentInChildren<Text>().text = newSquad.squadName;
            squadGO.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
            squadGO.GetComponent<Button>().onClick.AddListener(() => 
            {
                UIController.I.HandleSpawnSquadLogic();
                Debug.Log("this is " + newSquad.squadName + ". It has " + newSquad.numRows + "rows and " + newSquad.numCols + " cols.");
                SquadConfiguration.Squad squadConfig = new SquadConfiguration.Squad(newSquad.squadRol, newSquad.numRows, newSquad.numCols);
                FindObjectOfType<SquadConfiguration>().currentSquadSelected = squadConfig;
            });
        }

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

    public void ButtonCustomPressed()
    {
        SE_SquadEditorController.Create();
        ClosePanel();
        Destroy(this.gameObject);
    }

    public void HandleButtons()
    {
        if (formation != null)
        {
            if (SquadConfiguration.ListsAreEquals(formation, FindObjectOfType<SquadConfiguration>().basicContentionSquad.listFormation))
            {
                contentionButton.gameObject.SetActive(false);
            }
            else if (SquadConfiguration.ListsAreEquals(formation, FindObjectOfType<SquadConfiguration>().basicPenetrationSquad.listFormation))
            {
                penetrationButton.gameObject.SetActive(false);
            }
            else if (SquadConfiguration.ListsAreEquals(formation, FindObjectOfType<SquadConfiguration>().basicAroundPlayerSquad.listFormation))
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

        else
        {
            customButton.gameObject.SetActive(true);
        }

    }
}
