using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UISquadIndividualOptionsController : GenericPanelController
{
    public Button FollowPlayerButton;
    public Button BreakFormationButton;
    public Button ChangePositionButton;
    public Button AssignLeaderButton;
    public Button CancelButton;

    public static UISquadIndividualOptionsController Create(Action callbackButtonFollowPlayer, Action callbackButtonBreakFormation, Action callbackButtonChangePosition, Action callbackButtonAssignLeader)
    {
        if (FindObjectOfType<UISquadIndividualOptionsController>() != null)
        {
            Destroy(FindObjectOfType<UISquadIndividualOptionsController>().gameObject);
            return null;
        }
        GameObject UIIndividualOptions = Instantiate(Resources.Load("Prefabs/Popups/UIIndividualSquadOptions")) as GameObject;
        UISquadIndividualOptionsController UIIndividualOptionsController = UIIndividualOptions.GetComponent<UISquadIndividualOptionsController>();

        UIIndividualOptionsController.FollowPlayerButton.onClick.AddListener(() => { callbackButtonFollowPlayer(); UIIndividualOptionsController.ClosePanel(); });
        UIIndividualOptionsController.BreakFormationButton.onClick.AddListener(() => { callbackButtonBreakFormation(); UIIndividualOptionsController.ClosePanel(); });
        UIIndividualOptionsController.ChangePositionButton.onClick.AddListener(() => { callbackButtonChangePosition(); UIIndividualOptionsController.ClosePanel(); });
        UIIndividualOptionsController.AssignLeaderButton.onClick.AddListener(() => { callbackButtonAssignLeader(); UIIndividualOptionsController.ClosePanel(); });
        UIIndividualOptionsController.CancelButton.onClick.AddListener(() => { UIIndividualOptionsController.ClosePanel(); Destroy(UIIndividualOptionsController.gameObject); });

        UIIndividualOptionsController.transform.SetParent(GameObject.Find("MainCanvas").transform, false);

        UIIndividualOptionsController.OpenPanel();

        return UIIndividualOptionsController;
    }
}
