using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIIndividualWrestlerOptionsController : GenericPanelController
{
    public Button JoinSquadButton;
    public Button FollowPlayerButton;
    public Button MovePositionButton;
    public Button AttackPosotionButton;
    public Button CancelButton;

    public static UIIndividualWrestlerOptionsController Create(Action callbackJoinSquad, Action callbackFollowPlayer, Action callbackMovePosition, Action callbackAttack)
    {
        if (FindObjectOfType<UIIndividualWrestlerOptionsController>() != null)
        {
            Destroy(FindObjectOfType<UIIndividualWrestlerOptionsController>().gameObject);
            return null;
        }
        GameObject UIIndividualOptions = Instantiate(Resources.Load("Prefabs/Popups/UIIndividualWrestlerOptions")) as GameObject;
        UIIndividualWrestlerOptionsController UIIndividualWrestlerController = UIIndividualOptions.GetComponent<UIIndividualWrestlerOptionsController>();

        UIIndividualWrestlerController.JoinSquadButton.onClick.AddListener(() => { callbackJoinSquad(); UIIndividualWrestlerController.ClosePanel(); });
        UIIndividualWrestlerController.FollowPlayerButton.onClick.AddListener(() => { callbackFollowPlayer(); UIIndividualWrestlerController.ClosePanel(); });
        UIIndividualWrestlerController.MovePositionButton.onClick.AddListener(() => { callbackMovePosition(); UIIndividualWrestlerController.ClosePanel(); });
        UIIndividualWrestlerController.AttackPosotionButton.onClick.AddListener(() => { callbackAttack(); UIIndividualWrestlerController.ClosePanel(); });
        UIIndividualWrestlerController.CancelButton.onClick.AddListener(() => { UIIndividualWrestlerController.ClosePanel(); });

        UIIndividualWrestlerController.transform.SetParent(GameObject.Find("MainCanvas").transform, false);

        UIIndividualWrestlerController.OpenPanel();

        return UIIndividualWrestlerController;
    }
}
