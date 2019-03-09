using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorBegin : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>() != null)
        {
            MultipathController mc = this.GetComponentInParent<MultipathController>();
            PathBehavior pb = this.transform.parent.GetComponentInChildren<PathBehavior>();
            mc.distanceTraveled = (mc.initialPosition - other.transform.localPosition);
            if (pb.FirstPath)
            {
                mc.distanceTraveled = Vector3.zero;
                mc.initialPosition = other.transform.position;
            }
            else if (!pb.FirstPath && mc.currentPlayerIndex == pb.LastCorridorCorrectIndex)
            {
            }
            else
            {
                mc.initialPosition = Vector3.zero;
                //other.transform.Translate(mc.distanceTraveled);
                other.transform.position += mc.distanceTraveled;

                mc.distanceTraveled = Vector3.zero;
            }
        }
    }
}
