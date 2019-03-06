using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoogieWrestlerDistance : BoogieWrestler
{
    public override void WrestlerClicked(int clickButton)
    {
        Debug.Log("hola soy " + gameObject.name);
        base.WrestlerClicked(clickButton);
        if (clickButton == 1)
        {
            //SINGULAR MODE
        }
    }

    public override void Start()
    {
        base.Start();
    }
}
