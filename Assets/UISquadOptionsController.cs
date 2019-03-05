using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.UI;

public class UISquadOptionsController : GenericPanelController
{
    public Button MoveOrStopButton;
    public Button ChangeFormationButton;
    public Button RotateFormationButton;
    public Button CoverBoogieButton;
    public Button CancelButton;

    public static UISquadOptionsController Create(Action callbackButtonMoveOrStop, Action callbackButtonFormation, Action callbackButtonRotate, Action callbackButtonCoverBoogie)
    {
        if (FindObjectOfType<UISquadOptionsController>() != null)
        {
            Destroy(FindObjectOfType<UISquadOptionsController>().gameObject);
        }
        GameObject UISquadTroopOptions = Instantiate(Resources.Load("Prefabs/Popups/UISquadOptions")) as GameObject;
        UISquadOptionsController UISquadOptionsController = UISquadTroopOptions.GetComponent<UISquadOptionsController>();

        UISquadOptionsController.MoveOrStopButton.onClick.AddListener(() => { callbackButtonMoveOrStop(); UISquadOptionsController.ClosePanel(); });
        UISquadOptionsController.ChangeFormationButton.onClick.AddListener(() => { callbackButtonFormation(); UISquadOptionsController.ClosePanel(); });
        UISquadOptionsController.RotateFormationButton.onClick.AddListener(() => { callbackButtonRotate(); });
        UISquadOptionsController.CoverBoogieButton.onClick.AddListener(() => { callbackButtonCoverBoogie(); UISquadOptionsController.ClosePanel(); });
        UISquadOptionsController.CancelButton.onClick.AddListener(() => { UISquadOptionsController.ClosePanel(); UISquadOptionsController.ClosePanel(); });

        UISquadOptionsController.transform.SetParent(GameObject.Find("MainCanvas").transform, false);

        UISquadOptionsController.OpenPanel();

        return UISquadOptionsController;
    }
}
