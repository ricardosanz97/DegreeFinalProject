using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SE_SelectWrestlerButton : MonoBehaviour
{
    public SquadConfiguration.SQUAD_ROL rol;
    private SE_SquadEditorController controller;
    private void Awake()
    {
        controller = FindObjectOfType<SE_SquadEditorController>();
        this.GetComponent<Button>().onClick.AddListener(() =>
        {
            if (controller.selectingWrestler)
            {
                controller.SetImage(rol);
                controller.selectingWrestler = false;
            }
        });
    }
}
