using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.AI;

public class TriggerOpenDoorCleaners : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>() && !LevelManager.I.cleanersDoorOpened)
        {
            LevelManager.I.OpenCleanersDoors();
        }
    }
}
