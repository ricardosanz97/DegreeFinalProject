using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UISquadSelectorController : GenericPanelController
{
    public BoogieWrestlerCommander commander;
    public Button customButton;
    public Transform parent;

    public static UISquadSelectorController Create(BoogieWrestlerCommander commander = null)
    {
        if (FindObjectOfType<UISquadSelectorController>() != null)
        {
            Destroy(FindObjectOfType<UISquadSelectorController>().gameObject);
            return null;
        }
        GameObject UISquadSelector = Instantiate(Resources.Load("Prefabs/Popups/UISelectorSquad")) as GameObject;
        UISquadSelectorController UISquadSelectorController = UISquadSelector.GetComponent<UISquadSelectorController>();
        UISquadSelectorController.transform.SetParent(GameObject.Find("MainCanvas").transform, false);
        UISquadSelectorController.commander = commander;
        UISquadSelectorController.HandleButtons();
        
        ScriptableObject[] customSquads = Resources.LoadAll<ScriptableObject>("Squads/");

        if (commander == null) //spawning squad
        {
            foreach (ScriptableObject customSquad in customSquads)
            {
                Squad newSquad = AssetDatabase.LoadAssetAtPath("Assets/Resources/Squads/" + customSquad.name + ".asset", typeof(ScriptableObject)) as Squad;

                GameObject squadGO = Instantiate(UISquadSelectorController.customButton.gameObject);
                squadGO.transform.SetParent(UISquadSelectorController.parent, false);
                squadGO.GetComponentInChildren<Text>().text = newSquad.squadName;
                squadGO.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
                squadGO.GetComponent<Button>().onClick.AddListener(() =>
                {
                    UIController.I.HandleSpawnSquadLogic();
                    SquadConfiguration.Squad squadConfig = new SquadConfiguration.Squad(newSquad.squadRol, newSquad.numRows, newSquad.numCols);
                    FindObjectOfType<SquadConfiguration>().currentSquadSelected = new SquadConfiguration.Squad(newSquad.squadRol, newSquad.numRows, newSquad.numCols);
                });
            }
        }
        else //changing squad
        {
            foreach (ScriptableObject customSquad in customSquads)
            {
                Squad newSquad = AssetDatabase.LoadAssetAtPath("Assets/Resources/Squads/" + customSquad.name + ".asset", typeof(ScriptableObject)) as Squad;
                SquadConfiguration.Squad squadConfig = new SquadConfiguration.Squad(newSquad.squadRol, newSquad.numRows, newSquad.numCols);

                if (SquadConfiguration.ListsAreNumberByWrestlersEqual(squadConfig, commander.squadInfo) && squadConfig.hasBody && commander.squadInfo.hasBody && squadConfig != commander.squadInfo)
                {
                    GameObject squadGO = Instantiate(UISquadSelectorController.customButton.gameObject);
                    squadGO.transform.SetParent(UISquadSelectorController.parent, false);
                    squadGO.GetComponentInChildren<Text>().text = newSquad.squadName;
                    squadGO.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
                    squadGO.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        commander.ChangeSquadFormation(squadConfig);
                        FindObjectOfType<SquadConfiguration>().currentSquadSelected = new SquadConfiguration.Squad(newSquad.squadRol, newSquad.numRows, newSquad.numCols);
                    });
                }
            }
        }

        UISquadSelectorController.OpenPanel();
        return UISquadSelectorController;
    }

    public void ButtonCustomPressed()
    {
        SE_SquadEditorController.Create();
        ClosePanel();
        Destroy(this.gameObject);
    }

    public void HandleButtons()
    {
        if (commander != null)
        {
            customButton.gameObject.SetActive(false);
        }
        else
        {
            customButton.gameObject.SetActive(true);
        }
    }
}
