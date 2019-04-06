﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoogieWrestlerGiant : BoogieWrestler
{
    public override void WrestlerClicked(int clickButton)
    {
        base.WrestlerClicked(clickButton);
        if (clickButton == 1 && currentWState == W_STATE.OnSquad)
        {
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
            () =>
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
            this.commander.giantWrestlers.Add(this);
            return true;
        }
    }

    public override void Start()
    {
        base.Start();
    }
}
