using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCorridorTrigger : MonoBehaviour
{
    public int CorriderIndex;
    public GameObject endDoor;
    public bool doorOpened = false;
    public bool Correct = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>() != null)
        {
            this.GetComponentInParent<MultipathController>().currentPlayerIndex = this.GetComponent<PathCorridorTrigger>().CorriderIndex;
        }

        if (other.GetComponent<PlayerMovement>() || other.GetComponent<BoogieExplorer>())
        {
            if (!doorOpened)
            {
                OpenDoor();
            }
        }
    }

    private void Start()
    {
        endDoor = this.transform.Find("DoorModelEnd").gameObject;
    }

    

    void OpenDoor()
    {
        endDoor.transform.Find("pCube4").transform.localPosition = GetComponentInParent<MultipathController>().rightDoorOpened.position;
        endDoor.transform.Find("pCube4").transform.rotation = GetComponentInParent<MultipathController>().rightDoorOpened.rotation;

        endDoor.transform.Find("polySurface4").transform.localPosition = GetComponentInParent<MultipathController>().leftDoorOpened.position;
        endDoor.transform.Find("polySurface4").transform.rotation = GetComponentInParent<MultipathController>().leftDoorOpened.rotation;

        this.doorOpened = true;
    }
}
