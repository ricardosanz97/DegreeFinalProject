using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Diagnostics.Contracts;

[System.Serializable]
public class SE_Slot
{
    public SquadConfiguration.SQUAD_ROL rol;
    public SquadConfiguration.Index index;
    public SE_Button editorButton;

    public SE_Slot(SquadConfiguration.SQUAD_ROL _rol, SquadConfiguration.Index _index)
    {
        this.rol = _rol;
        this.index = _index;
    }
}

public class SE_SquadEditorController : GenericPanelController
{
    public Transform matrix;
    public List<SE_Slot> slots;

    public SE_Button currentButtonClicked;

    public SE_Button commanderButton;
    public SE_Slot commanderSlot;

    public SE_Button objectButton;
    public SE_Slot objectSlot;

    public bool selectingWrestler;

    public List<SquadConfiguration.SQUAD_ROL> squadList = new List<SquadConfiguration.SQUAD_ROL>();
    public int numRows = 0;
    public int numCols = 0;

    public Text newSquadName;
    public Dropdown dropdownOptions;
    public GameObject squadNamePopup;
    public GameObject commanderNotSelectedPopup;
    public GameObject squadAlreadyExistsPopup;

    public bool allie;

    public static SE_SquadEditorController Create(int enemy)
    {
        GameObject SE_SquadEditor = Instantiate(Resources.Load("Prefabs/Popups/CustomSquadEditor")) as GameObject;
        SE_SquadEditorController SE_SquadEditorController = SE_SquadEditor.GetComponent<SE_SquadEditorController>();
        SE_SquadEditor.transform.SetParent(GameObject.Find("MainCanvas").transform, false);
        SE_SquadEditorController.allie = enemy == 1;
        SE_SquadEditorController.OpenPanel();

        return SE_SquadEditorController;
    }

    private void Awake()
    {
        ScriptableObject[] customConfigurations = Resources.LoadAll<ScriptableObject>("Configurations/WrestlersConfigurations/");
        foreach (ScriptableObject so in customConfigurations)
        {
            Dropdown.OptionData newOptionData = new Dropdown.OptionData(so.name);
            dropdownOptions.options.Add(newOptionData);
            
        }
        SetSlots();   
    }

    public void SlotClicked(SE_Button button)
    {
        SE_Slot clicked = slots.Find((x) => x.index.i == button.indexsMatrix.i && x.index.j == button.indexsMatrix.j);

        this.currentButtonClicked = button;
        this.selectingWrestler = true;

        button.gameObject.GetComponent<Image>().color = Color.yellow;
    }

    private void SetSlots()
    {
        slots.Clear();
        for (int i = 0; i < matrix.childCount; i++)
        {
            for (int j = 0; j < matrix.GetChild(i).childCount; j++)
            {
                SE_Slot newSlot = new SE_Slot(SquadConfiguration.SQUAD_ROL.None, new SquadConfiguration.Index(j, i));
                newSlot.editorButton = matrix.GetChild(i).GetChild(j).GetComponent<SE_Button>();
                newSlot.editorButton.GetComponent<Image>().color = Color.green;
                newSlot.editorButton.indexsMatrix = new SquadConfiguration.Index(j, i);
                slots.Add(newSlot);
            }
        }

        commanderSlot = slots.Find((x) => x.index.i == 4 && x.index.j == 4);
        commanderSlot.rol = SquadConfiguration.SQUAD_ROL.Commander;
        commanderSlot.editorButton.empty = false;
        commanderButton = commanderSlot.editorButton;
        commanderButton.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Portraits/allie_commander_portrait");
    }

    public void SetImage(SquadConfiguration.SQUAD_ROL rol)
    {
        Image slotImage = currentButtonClicked.transform.GetChild(0).gameObject.GetComponent<Image>();
        switch (rol)
        {
            case SquadConfiguration.SQUAD_ROL.Commander:
                if (commanderButton != null)
                {
                    commanderSlot.rol = SquadConfiguration.SQUAD_ROL.None;
                    commanderButton.transform.GetChild(0).GetComponent<Image>().sprite = null;
                    commanderButton.empty = true;
                }
                commanderButton = currentButtonClicked;
                commanderSlot = slots.Find((x)=>x.index.i == commanderButton.indexsMatrix.i && x.index.j == commanderButton.indexsMatrix.j);

                slotImage.sprite = Resources.Load<Sprite>("Sprites/Portraits/allie_commander_portrait");
                break;
            case SquadConfiguration.SQUAD_ROL.Close:
                slotImage.sprite = Resources.Load<Sprite>("Sprites/Portraits/allie_close_portrait");
                break;
            case SquadConfiguration.SQUAD_ROL.Distance:
                slotImage.sprite = Resources.Load<Sprite>("Sprites/Portraits/allie_distance_portrait");
                break;
            case SquadConfiguration.SQUAD_ROL.Giant:
                slotImage.sprite = Resources.Load<Sprite>("Sprites/Portraits/allie_giant_portrait");
                break;
            case SquadConfiguration.SQUAD_ROL.Body:
                if (objectButton != null)
                {
                    objectSlot.rol = SquadConfiguration.SQUAD_ROL.None;
                    objectButton.transform.GetChild(0).GetComponent<Image>().sprite = null;
                    objectButton.empty = true;
                }
                objectButton = currentButtonClicked;
                objectSlot = slots.Find((x) => x.index.i == objectButton.indexsMatrix.i && x.index.j == objectButton.indexsMatrix.j);

                slotImage.sprite = Resources.Load<Sprite>("Sprites/Portraits/player_portrait");
                break;
        }

        slots.Find((x) => x.index.i == currentButtonClicked.indexsMatrix.i && x.index.j == currentButtonClicked.indexsMatrix.j).rol = rol;
        currentButtonClicked.GetComponent<Image>().color = Color.green;
        currentButtonClicked.empty = false;
        currentButtonClicked = null;
    }

    public void ResetButtonPressed()
    {
        selectingWrestler = false;
        currentButtonClicked = null;
        for (int i = 0; i<slots.Count; i++)
        {
            slots[i].editorButton.GetComponent<Image>().color = Color.green;
            slots[i].editorButton.transform.GetChild(0).GetComponent<Image>().sprite = null;
            slots[i].rol = SquadConfiguration.SQUAD_ROL.None;
            slots[i].editorButton.empty = true;
        }
    }

    public void SaveButtonPressed()
    {
        squadList = new List<SquadConfiguration.SQUAD_ROL>();
        numRows = matrix.GetChild(0).childCount;
        numCols = matrix.childCount;
        Debug.Log("Num rows: " + numRows);
        Debug.Log("Num cols: " + numCols);
        for (int i = matrix.GetChild(0).childCount - 1; i >= 0; i--)
        {
            for (int j = 0; j < matrix.childCount; j++)
            {
                SquadConfiguration.SQUAD_ROL rol = slots.Find((x) => x.index.i == i && x.index.j == j).rol;
                squadList.Add(rol);
            }
        }

        bool hasCommander = false;
        for (int i = 0; i < squadList.Count; i++)
        {
            if (squadList[i] == SquadConfiguration.SQUAD_ROL.Commander)
            {
                hasCommander = true;
            }
        }
        if (!hasCommander)
        {
            commanderNotSelectedPopup.SetActive(true);
        }
        else
        {
            UIController.I.writing = true;
            squadNamePopup.SetActive(true);
        }
    }

    public void DefinitiveSaveButtonPressed()
    {
        Squad asset = ScriptableObject.CreateInstance<Squad>();
        string configName = dropdownOptions.transform.Find("Label").GetComponent<Text>().text;
        asset.customConfiguration = Resources.Load<WrestlersConfiguration>("Configurations/WrestlersConfigurations/" + configName);

        if (newSquadName.text == "")
        {
            asset.squadName = newSquadName.GetComponentInParent<InputField>().transform.Find("Placeholder").GetComponent<Text>().text;
        }
        else
        {
            asset.squadName = newSquadName.text;
        }

        asset.squadRol = squadList;
        asset.numRows = numRows;
        asset.numCols = numCols;

        Object a = Resources.Load("Squads/" + asset.squadName);
        if (a != null)
        {
            squadAlreadyExistsPopup.SetActive(true);
            return;
        }

        string path = null;
        if (allie)
        {
            path = "Assets/Resources/Squads/Allies/" + asset.squadName + ".asset";
        }
        else
        {
            path = "Assets/Resources/Squads/Enemies/" + asset.squadName + ".asset";
        }

#if UNITY_EDITOR
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
#else
        if (allie){
            LevelManager.I.provisionalAlliesCreatedSquads.Add(asset);
        }
        else{
            LevelManager.I.provisionalEnemiesCreatedSquads.Add(asset);
        }
#endif
        this.ClosePanel();
        squadNamePopup.SetActive(false);
        UIController.I.writing = false;
    }

    public void CancelButtonPressed()
    {
        squadNamePopup.SetActive(false);
        UIController.I.writing = false;
    }

    public void CloseButtonPressed()
    {
        this.ClosePanel();
    }

    public void OkCommanderNeededButtonPressed()
    {
        commanderNotSelectedPopup.SetActive(false);
    }

    public void OkSquadAlreadyExistsButtonPressed()
    {
        squadAlreadyExistsPopup.SetActive(false);
    }
}
