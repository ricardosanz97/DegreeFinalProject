using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoogieWrestlerClose : BoogieWrestler
{
    public override void WrestlerClicked(int clickButton)
    {
        base.WrestlerClicked(clickButton);
        if (clickButton == 1 && currentWState == W_STATE.OnSquad)
        {
            Debug.Log("hola soy " + gameObject.name);
            UISquadIndividualOptionsController.Create(
            () =>
            {
                FollowPlayer();
            },
            () =>
            {
                BreakFormation();
            }
            ,
            () =>
            {
                ChangePosition();
            },
            ()=>
            {
                AssignAsLeader();
            }
            );
        }
    }

    public override bool JoinSquad(BoogieWrestlerCommander bwc)
    {
        bool canJoin = base.JoinSquad(bwc);
        if (!canJoin)
        {
            return false;
        }
        else
        {
            this.commander.closeWrestlers.Add(this);
            return true;
        }
    }

    public override void Start()
    {
        base.Start();
    }
}
