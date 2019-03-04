using System;
using UnityEngine;
using System.Collections;

public class BoogieWrestler : Boogie
{
    public WRESTLER_TYPE wrestlerType;
    public SquadConfiguration.Index indexs;
    public BoogieWrestlerCommander commander;
    public GameObject leader;

    public virtual void WrestlerClicked(int clickButton)
    {
        UIController.OnTroopClicked -= WrestlerClicked;
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
        TakeInitialPosition();
    }

    private void ChangeIndexsRelativeToLeader()
    {
        indexs.i -= commander.leaderIndex.i;
        indexs.j -= commander.leaderIndex.j;
    }

    private void TakeInitialPosition()
    {
        Debug.Log(leader.transform.position);
        Vector3 newPosZ = leader.transform.localPosition + this.transform.TransformDirection(leader.transform.forward.normalized * (commander.distanceBetweenUs * indexs.i));
        Vector3 newPosX = leader.transform.localPosition + this.transform.TransformDirection(leader.transform.right.normalized * (commander.distanceBetweenUs * indexs.j));

        Vector3 newPos = newPosZ - newPosX;
        _agent.SetDestination(leader.transform.TransformPoint(newPos));
    }
}
