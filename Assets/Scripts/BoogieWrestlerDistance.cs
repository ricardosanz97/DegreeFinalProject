using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoogieWrestlerDistance : BoogieWrestler
{
    public override void WrestlerClicked(int clickButton)
    {
        base.WrestlerClicked(clickButton);
        if (clickButton == 1)
        {
            Debug.Log("hola soy " + gameObject.name);
        }
    }

    public override void Start()
    {
        base.Start();
    }
}
