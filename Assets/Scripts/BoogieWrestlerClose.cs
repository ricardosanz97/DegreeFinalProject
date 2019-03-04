using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoogieWrestlerClose : BoogieWrestler
{
    public override void WrestlerClicked(int clickButton)
    {
        Debug.Log("hola soy " + gameObject.name);
        base.WrestlerClicked(clickButton);
        if (clickButton == 0)
        {
            //TROOP MODE
            UISquadTroopOptionsController.Create(() => 
            {
                UIController.OnMoveSquadPositionSelected += commander.MoveToPosition;
            }, 
            () => 
            {

            });
        }
        else if (clickButton == 1)
        {
            //SINGULAR MODE
        }
    }

    public override void Start()
    {
        base.Start();
    }
}
