using System;
using UnityEngine;
using System.Collections;

public class BoogieWrestler : Boogie
{
    public WRESTLER_TYPE wrestlerType;
    public SquadConfiguration.Index indexs;
    public BoogieWrestlerCommander commander;

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
        ChangeIndexsRelativeToCommander();
        TakeInitialPosition();
    }

    private void ChangeIndexsRelativeToCommander()
    {
        indexs.i -= commander.commanderIndex.i;
        indexs.j -= commander.commanderIndex.j;
    }

    private void TakeInitialPosition()
    {
        Vector3 newPosZ = transform.localPosition + this.transform.TransformDirection(commander.transform.forward.normalized * (commander.distanceBetweenUs * indexs.i));
        Vector3 newPosX = transform.localPosition + this.transform.TransformDirection(commander.transform.right.normalized * (commander.distanceBetweenUs * indexs.j));

        Vector3 newPos = newPosZ - newPosX;
        _agent.SetDestination(transform.TransformPoint(newPos));
    }

    public void OnMouseUp()
    {
        Debug.Log("clicking a wrestler!");
        commander.selectingPosition = true;
        //SelectorMouseBehavior.Create(SELECTION_TYPE.SquadMovingPosition);
    }
}
