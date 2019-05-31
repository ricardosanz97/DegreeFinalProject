using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandboxManager : MonoBehaviour
{
    void Start()
    {
        FindObjectOfType<MultipathController>().CloseAllDoors();
    }
}
