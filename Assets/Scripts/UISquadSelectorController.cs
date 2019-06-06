using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UISquadSelectorController : GenericPanelController
{
    public BoogieWrestlerCommander commander;
    public Button customButton;
    public Transform parent;
    public TEAM team;

    public static UISquadSelectorController Create(TEAM team, BoogieWrestlerCommander commander = null)
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
        UISquadSelectorController.team = team;
        UISquadSelectorController.HandleButtons();

        ScriptableObject[] customSquads = null;
        if (team == TEAM.A)
        {
            customSquads = Resources.LoadAll<ScriptableObject>("Squads/Allies/");
        }
        else
        {
            customSquads = Resources.LoadAll<ScriptableObject>("Squads/Enemies/");
        }

        if (commander == null) //spawning squad
        {
            foreach (ScriptableObject customSquad in customSquads)
            {
                Squad newSquad = null;
                if (team == TEAM.A)
                {
                    //newSquad = AssetDatabase.LoadAssetAtPath("Assets/Resources/Squads/Allies/" + customSquad.name + ".asset",
                    //typeof(ScriptableObject)) as Squad;
                    newSquad = Resources.Load<ScriptableObject>("Squads/Allies/" + customSquad.name) as Squad;
                }
                else
                {
                    //newSquad = AssetDatabase.LoadAssetAtPath("Assets/Resources/Squads/Enemies/" + customSquad.name + ".asset",
                    //typeof(ScriptableObject)) as Squad;
                    newSquad = Resources.Load<ScriptableObject>("Squads/Enemies/" + customSquad.name) as Squad;
                }

                GameObject squadGO = Instantiate(UISquadSelectorController.customButton.gameObject);
                squadGO.transform.SetParent(UISquadSelectorController.parent, false);
                squadGO.GetComponentInChildren<Text>().text = newSquad.squadName;
                squadGO.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
                squadGO.GetComponent<Button>().onClick.AddListener(() =>
                {
                    if (team == TEAM.A)
                    {
                        UIController.I.HandleSpawnAllieSquadLogic();
                    }
                    else
                    {
                        UIController.I.HandleSpawnEnemySquadLogic();
                    }
                    
                    SquadConfiguration.Squad squadConfig = new SquadConfiguration.Squad(newSquad.squadName, newSquad.squadRol, newSquad.numRows, newSquad.numCols, newSquad.customConfiguration);
                    FindObjectOfType<SquadConfiguration>().currentSquadSelected = new SquadConfiguration.Squad(newSquad.squadName, newSquad.squadRol, newSquad.numRows, newSquad.numCols, newSquad.customConfiguration);
                    UISquadSelectorController.ClosePanel();
                });

            }
            if (team == TEAM.A)
            {
                if (FindObjectOfType<LevelManager>() == null)
                {
                    return null;
                }
                foreach (Squad s in LevelManager.I.provisionalAlliesCreatedSquads)
                {
                    GameObject squadGO = Instantiate(UISquadSelectorController.customButton.gameObject);
                    squadGO.transform.SetParent(UISquadSelectorController.parent, false);
                    squadGO.GetComponentInChildren<Text>().text = s.squadName;
                    squadGO.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
                    squadGO.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        UIController.I.HandleSpawnAllieSquadLogic();
                        SquadConfiguration.Squad squadConfig = new SquadConfiguration.Squad(s.squadName, s.squadRol, s.numRows, s.numCols, s.customConfiguration);
                        FindObjectOfType<SquadConfiguration>().currentSquadSelected = new SquadConfiguration.Squad(s.squadName, s.squadRol, s.numRows, s.numCols, s.customConfiguration);
                        UISquadSelectorController.ClosePanel();
                    });
                }
            }
            else
            {
                if (FindObjectOfType<LevelManager>() == null)
                {
                    return null;
                }
                foreach (Squad s in LevelManager.I.provisionalEnemiesCreatedSquads)
                {
                    GameObject squadGO = Instantiate(UISquadSelectorController.customButton.gameObject);
                    squadGO.transform.SetParent(UISquadSelectorController.parent, false);
                    squadGO.GetComponentInChildren<Text>().text = s.squadName;
                    squadGO.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
                    squadGO.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        UIController.I.HandleSpawnEnemySquadLogic();
                        SquadConfiguration.Squad squadConfig = new SquadConfiguration.Squad(s.squadName, s.squadRol, s.numRows, s.numCols, s.customConfiguration);
                        FindObjectOfType<SquadConfiguration>().currentSquadSelected = new SquadConfiguration.Squad(s.squadName, s.squadRol, s.numRows, s.numCols, s.customConfiguration);
                        UISquadSelectorController.ClosePanel();
                    });
                }
            }
            
        }
        else //changing squad
        {
            foreach (ScriptableObject customSquad in customSquads)
            {
                Squad newSquad = null;
                if (team == TEAM.A)
                {
                    //newSquad = AssetDatabase.LoadAssetAtPath("Assets/Resources/Squads/Allies/" + customSquad.name + ".asset", typeof(ScriptableObject)) as Squad;
                    newSquad = Resources.Load<ScriptableObject>("Squads/Allies/" + customSquad.name) as Squad;
                    
                }
                else
                {
                    //newSquad = AssetDatabase.LoadAssetAtPath("Assets/Resources/Squads/Enemies/" + customSquad.name + ".asset", typeof(ScriptableObject)) as Squad;
                    newSquad = Resources.Load<ScriptableObject>("Squads/Enemies/" + customSquad.name) as Squad;
                }
                SquadConfiguration.Squad squadConfig = new SquadConfiguration.Squad(newSquad.squadName, newSquad.squadRol, newSquad.numRows, newSquad.numCols, newSquad.customConfiguration);

                if (SquadConfiguration.ListsAreNumberByWrestlersEqual(squadConfig, commander.squadInfo) 
                    && squadConfig.hasBody == commander.squadInfo.hasBody && squadConfig.name != commander.squadInfo.name)
                {
                    GameObject squadGO = Instantiate(UISquadSelectorController.customButton.gameObject);
                    squadGO.transform.SetParent(UISquadSelectorController.parent, false);
                    squadGO.SetActive(true);
                    squadGO.GetComponentInChildren<Text>().text = newSquad.squadName;
                    squadGO.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
                    squadGO.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        commander.ChangeSquadFormation(squadConfig);
                        FindObjectOfType<SquadConfiguration>().currentSquadSelected = 
                        new SquadConfiguration.Squad(newSquad.squadName, newSquad.squadRol, newSquad.numRows, newSquad.numCols, newSquad.customConfiguration);
                        UISquadSelectorController.ClosePanel();
                    });
                }
            }

            if (team == TEAM.A)
            {
                if (FindObjectOfType<LevelManager>() == null)
                {
                    return null;
                }
                foreach (Squad s in LevelManager.I.provisionalAlliesCreatedSquads)
                {
                    SquadConfiguration.Squad squadConfig = new SquadConfiguration.Squad(s.squadName, s.squadRol, s.numRows, s.numCols, s.customConfiguration);
                    if (SquadConfiguration.ListsAreNumberByWrestlersEqual(squadConfig, commander.squadInfo)
                    && squadConfig.hasBody == commander.squadInfo.hasBody && s.name != commander.squadInfo.name)
                    {
                        GameObject squadGO = Instantiate(UISquadSelectorController.customButton.gameObject);
                        squadGO.transform.SetParent(UISquadSelectorController.parent, false);
                        squadGO.SetActive(true);
                        squadGO.GetComponentInChildren<Text>().text = s.squadName;
                        squadGO.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
                        squadGO.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            commander.ChangeSquadFormation(squadConfig);
                            FindObjectOfType<SquadConfiguration>().currentSquadSelected =
                            new SquadConfiguration.Squad(s.squadName, s.squadRol, s.numRows, s.numCols, s.customConfiguration);
                            UISquadSelectorController.ClosePanel();
                        });
                    }
                }
            }
            else
            {

                foreach (Squad s in LevelManager.I.provisionalEnemiesCreatedSquads)
                {
                    SquadConfiguration.Squad squadConfig = new SquadConfiguration.Squad(s.squadName, s.squadRol, s.numRows, s.numCols, s.customConfiguration);
                    if (SquadConfiguration.ListsAreNumberByWrestlersEqual(squadConfig, commander.squadInfo)
                    && squadConfig.hasBody == commander.squadInfo.hasBody && s.name != commander.squadInfo.name)
                    {
                        GameObject squadGO = Instantiate(UISquadSelectorController.customButton.gameObject);
                        squadGO.transform.SetParent(UISquadSelectorController.parent, false);
                        squadGO.SetActive(true);
                        squadGO.GetComponentInChildren<Text>().text = s.squadName;
                        squadGO.GetComponent<Button>().onClick = new Button.ButtonClickedEvent();
                        squadGO.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            commander.ChangeSquadFormation(squadConfig);
                            FindObjectOfType<SquadConfiguration>().currentSquadSelected =
                            new SquadConfiguration.Squad(s.squadName, s.squadRol, s.numRows, s.numCols, s.customConfiguration);
                            UISquadSelectorController.ClosePanel();
                        });
                    }
                }
            }
        }

        UISquadSelectorController.OpenPanel();
        return UISquadSelectorController;
    }

    public void ButtonCustomPressed()
    {
        SE_SquadEditorController.Create(FindObjectOfType<UISquadSelectorController>().team == TEAM.A ? 1 : 0);
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
