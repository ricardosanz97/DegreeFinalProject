using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SE_Button : MonoBehaviour, IPointerClickHandler
{
    private SE_SquadEditorController controller;
    public SquadConfiguration.Index indexsMatrix;
    public bool empty = true;

    private void Awake()
    {
        controller = this.gameObject.GetComponentInParent<SE_SquadEditorController>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (!this.empty)
            {
                return;
            }
            if (!controller.selectingWrestler)
            {
                controller.SlotClicked(this);
            }
            else
            {
                if (controller.currentButtonClicked == this)
                {
                    controller.currentButtonClicked.GetComponent<Image>().color = Color.green;
                    controller.currentButtonClicked = null;
                    controller.selectingWrestler = false;
                }
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (!this.empty)
            {
                Debug.Log("deleted: " + indexsMatrix.i + ", " + indexsMatrix.j);
                if (controller.slots.Find((x) => x.index.i == this.indexsMatrix.i && x.index.j == this.indexsMatrix.j).rol == SquadConfiguration.SQUAD_ROL.Commander)
                {
                    Debug.Log("commander deleted. ");
                    controller.commanderButton = null;
                    controller.commanderSlot = null;
                }

                else if (controller.slots.Find((x)=>x.index.i == this.indexsMatrix.i && x.index.j == this.indexsMatrix.j).rol == SquadConfiguration.SQUAD_ROL.Body)
                {
                    Debug.Log("object deleted. ");
                    controller.objectButton = null;
                    controller.objectSlot = null;
                }

                controller.slots.Find((x) => x.index.i == this.indexsMatrix.i && x.index.j == this.indexsMatrix.j).rol = SquadConfiguration.SQUAD_ROL.None;
                this.empty = true;
                this.transform.GetChild(0).GetComponent<Image>().sprite = null;
                
            }
        }
    }
}
