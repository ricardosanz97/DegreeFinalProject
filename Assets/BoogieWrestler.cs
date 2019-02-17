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
        TakePositions();
    }

    private void ChangeIndexsRelativeToCommander()
    {
        SquadConfiguration.Index commanderIndex = commander.indexs;
        int i = commanderIndex.i;
        int j = commanderIndex.j;

        indexs.i -= i;
        indexs.j -= j;
    }

    private void TakePositions()
    {
        Vector3 commanderPos = commander.transform.position;
        //Debug.Log("commanderPos = " + commanderPos);
        //Vector3 ourPosition = new Vector3(commanderPos.x + (commander.distanceBetweenUs * indexs.i), transform.position.y, commanderPos.z + (commander.distanceBetweenUs * indexs.j));
        //Debug.Log("ourpos = " + ourPosition);
        //_agent.SetDestination(ourPosition);
        //Debug.Log(_agent.gameObject.name);
        this.transform.Translate(commander.transform.forward * (commander.distanceBetweenUs * indexs.i));
        this.transform.Translate(commander.transform.right * (commander.distanceBetweenUs * indexs.j));
    }
}
