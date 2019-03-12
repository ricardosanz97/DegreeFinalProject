using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SE_Button : MonoBehaviour
{
    private SE_SquadEditorController controller;
    public SquadConfiguration.Index indexsMatrix;
    public bool empty = true;

    private void Awake()
    {
        controller = this.gameObject.GetComponentInParent<SE_SquadEditorController>();
        this.GetComponent<Button>().onClick.AddListener(() => {
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
        });
    }

    
}
