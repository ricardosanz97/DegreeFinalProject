using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCorridorTrigger : MonoBehaviour
{
    public int CorriderIndex;
    public bool Correct = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>() != null)
        {
            this.GetComponentInParent<MultipathController>().currentPlayerIndex = this.GetComponent<PathCorridorTrigger>().CorriderIndex;
        }
    }
}
