using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderBehavior : MonoBehaviour
{
    public void Start()
    {
        GameController.I.LoadScene((int)SCENES.Menu);
    }
}
