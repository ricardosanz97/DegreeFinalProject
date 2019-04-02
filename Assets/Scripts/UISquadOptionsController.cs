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
    public Button CoverButton;
    public Button UncoverButton;
    public Button ResetDefaultFormationButton;
    public Button CancelButton;
    public BoogieWrestlerCommander commander;

    public static UISquadOptionsController Create(BoogieWrestlerCommander bwc, Action callbackButtonMoveOrStop, Action callbackButtonFormation, Action callbackButtonRotate, Action callbackButtonCover, Action callbackButtonUncover, Action callbackButtonResetDefaultFormation)
    {
        if (FindObjectOfType<UISquadOptionsController>() != null)
        {
            Destroy(FindObjectOfType<UISquadOptionsController>().gameObject);
            return null;
        }
        GameObject UISquadTroopOptions = Instantiate(Resources.Load("Prefabs/Popups/UISquadOptions")) as GameObject;
        UISquadOptionsController UISquadOptionsController = UISquadTroopOptions.GetComponent<UISquadOptionsController>();

        UISquadOptionsController.MoveOrStopButton.onClick.AddListener(() => { callbackButtonMoveOrStop(); UISquadOptionsController.ClosePanel(); });
        UISquadOptionsController.ChangeFormationButton.onClick.AddListener(() => { callbackButtonFormation(); UISquadOptionsController.ClosePanel(); });
        UISquadOptionsController.RotateFormationButton.onClick.AddListener(() => { callbackButtonRotate(); });
        UISquadOptionsController.CoverButton.onClick.AddListener(() => { callbackButtonCover(); UISquadOptionsController.ClosePanel(); });
        UISquadOptionsController.UncoverButton.onClick.AddListener(() => { callbackButtonUncover(); UISquadOptionsController.ClosePanel(); });
        UISquadOptionsController.ResetDefaultFormationButton.onClick.AddListener(() => { callbackButtonResetDefaultFormation(); UISquadOptionsController.ClosePanel(); });
        UISquadOptionsController.CancelButton.onClick.AddListener(() => { UISquadOptionsController.ClosePanel(); Destroy(UISquadOptionsController.gameObject); });
        UISquadOptionsController.commander = bwc;
        UISquadOptionsController.HandleButtonStatus();

        UISquadOptionsController.transform.SetParent(GameObject.Find("MainCanvas").transform, false);

        UISquadOptionsController.OpenPanel();

        return UISquadOptionsController;
    }

    private void HandleButtonStatus()
    {
        if (commander.squadInfo.hasBody && commander.coveringBody == null)
        {
            //then, we can cover
            CoverButton.gameObject.SetActive(true);
        }
        else
        {
            CoverButton.gameObject.SetActive(false);
        }

        //TODO: 2- solo podemos rotar en caso que no estemos cubriendo al player

        if (commander.coveringBody)
        {
            UncoverButton.gameObject.SetActive(true);
        }
        else
        {
            UncoverButton.gameObject.SetActive(false);
        }

        if ((commander.squadWrestlers.Count != commander.squadInfo.numClose + commander.squadInfo.numDistance + commander.squadInfo.numGiant + 1)//the commander 
            || SquadConfiguration.ListsAreEquals(commander.currentSquadList, commander.squadInfo.listFormation))
        {
            ResetDefaultFormationButton.gameObject.SetActive(false);
        }
        else
        {
            ResetDefaultFormationButton.gameObject.SetActive(true);
        }

    }
}
