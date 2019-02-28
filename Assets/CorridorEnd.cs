using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorEnd : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>() != null)
        {
            if (other.GetComponent<PlayerMovement>() != null)
            {
                if (this.transform.parent.GetComponentInChildren<PathBehavior>().nextInitCorridor == null)
                {
                    this.GetComponentInParent<MultipathController>().playerCompleteMultipath = true;
                }
            }
        }
    }
}
