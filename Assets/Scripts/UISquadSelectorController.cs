using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class UISquadSelectorController : GenericPanelController
{
    public Button customButton;
    public Transform parent;

    public static UISquadSelectorController Create()
    {
        if (FindObjectOfType<UISquadSelectorController>() != null)
        {
            Destroy(FindObjectOfType<UISquadSelectorController>().gameObject);
            return null;
        }
        GameObject UISquadSelector = Instantiate(Resources.Load("Prefabs/Popups/UISelectorSquad")) as GameObject;
        UISquadSelectorController UISquadSelectorController = UISquadSelector.GetComponent<UISquadSelectorController>();
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

    public void ButtonCustomPressed()
    {
        SE_SquadEditorController.Create();
        ClosePanel();
        Destroy(this.gameObject);
    }

    public void HandleButtons()
    {
        customButton.gameObject.SetActive(true);

    }
}
