using System;
using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class BoogieWrestler : Boogie
{
    public WRESTLER_TYPE wrestlerType;
    public SquadConfiguration.Index indexs;
    public BoogieWrestlerCommander commander;
    public GameObject leader;
    private bool followPlayer = false;

    public virtual void WrestlerClicked(int clickButton)
    {
        BoogiesSpawner.CommanderSelected = commander;
        UIController.OnTroopClicked -= WrestlerClicked;
        if (clickButton == 0)
        {
            UISquadOptionsController.Create(() =>
            {
                Debug.Log("moving squad. ");
                UIController.I.UIShowMouseSelector(SELECTION_TYPE.SquadMovingPosition);
                UIController.OnMoveSquadPositionSelected += commander.MoveToPosition;
            },
            () =>
            {
                Debug.Log("changing formation. ");
                UISquadSelectorController.Create(commander.formation);
            },
            () =>
            {
                Debug.Log("rotation formation. ");
                this.commander.transform.Rotate(new Vector3(0, 1, 0) * 90f);
            },
            () =>
            {
                Debug.Log("cover boogie. ");
            }
            );
        }
    }

    public override void BackToPlayer()
    {
    }

    public override void OnObjectiveSelected()
    {
    }

    public enum WRESTLER_TYPE
    {
        Commander,
        Distance,
        Close,
        Giant,
    }

    public virtual void Start()
    {
        commander = this.transform.parent.gameObject.GetComponentInChildren<BoogieWrestlerCommander>();
        if (commander.hasPlayer)
        {
            leader = FindObjectOfType<BoogiesSpawner>().gameObject;
        }
        else
        {
            leader = commander.gameObject;
        }
        ChangeIndexsRelativeToLeader();
        followPlayer = true;
        TakeInitialPosition();
    }

    public void ChangeIndexsRelativeToLeader()
    {
        indexs.i -= commander.leaderIndex.i;
        indexs.j -= commander.leaderIndex.j;
    }

    public void TakeInitialPosition()
    {
        this.transform.rotation = leader.transform.rotation;
        Vector3 offset = new Vector3(indexs.j * commander.distanceBetweenUs, this.transform.position.y, indexs.i * commander.distanceBetweenUs);
        _agent.SetDestination(leader.transform.position + this.transform.TransformDirection(offset));
    }

    public virtual void Update()
    {
        if (this.leader != this.gameObject) TakeInitialPosition();
    }
}
