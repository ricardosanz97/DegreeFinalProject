using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoogieWrestlerDistance : BoogieWrestler
{
    public override void WrestlerClicked(int clickButton)
    {
        Debug.Log("hola soy " + gameObject.name);
        base.WrestlerClicked(clickButton);
    }

    public override void Start()
    {
        base.Start();
    }
}
