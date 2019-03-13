using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

public class SE_SquadEditorController : MonoBehaviour
{
    public Transform matrix;
    public List<SE_Slot> slots;

    public SE_Button currentButtonClicked;

    public SE_Button commanderButton;
    public SE_Slot commanderSlot;

    public SE_Button objectButton;
    public SE_Slot objectSlot;

    public bool selectingWrestler;

    private void Awake()
    {
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
        commanderButton.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites/Placeholder/Commander");
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

                slotImage.sprite = Resources.Load<Sprite>("Sprites/Placeholder/Commander");
                break;
            case SquadConfiguration.SQUAD_ROL.Close:
                slotImage.sprite = Resources.Load<Sprite>("Sprites/Placeholder/Close");
                break;
            case SquadConfiguration.SQUAD_ROL.Distance:
                slotImage.sprite = Resources.Load<Sprite>("Sprites/Placeholder/Distance");
                break;
            case SquadConfiguration.SQUAD_ROL.Giant:
                slotImage.sprite = Resources.Load<Sprite>("Sprites/Placeholder/Giant");
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

                slotImage.sprite = Resources.Load<Sprite>("Sprites/Placeholder/Body");
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
}
