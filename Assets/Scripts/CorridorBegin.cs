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

        if (other.GetComponent<BoogieExplorer>() || other.GetComponent<PlayerMovement>())
        {
            if (!doorsOpened)
            {
                OpenBeginDoors();
            }
        }
    }

    public bool doorsOpened = false;
    public GameObject[] doorsBegin;
    public GameObject[] doorsEnd;
    public PathCorridorTrigger[] paths;

    public void Start()
    {
        paths = this.transform.parent.GetComponentsInChildren<PathCorridorTrigger>();
        doorsBegin = new GameObject[paths.Length];
        doorsEnd = new GameObject[paths.Length];
        for (int i = 0; i<paths.Length; i++)
        {
            doorsBegin[i] = paths[i].transform.Find("DoorModelBegin").gameObject;
            doorsEnd[i] = paths[i].transform.Find("DoorModelEnd").gameObject;
        }
    }

   

    public void OpenBeginDoors()
    {
        for (int i = 0; i<doorsBegin.Length; i++)
        {
            doorsBegin[i].transform.Find("pCube4").transform.localPosition = GetComponentInParent<MultipathController>().rightDoorOpened.position;
            doorsBegin[i].transform.Find("pCube4").transform.rotation = GetComponentInParent<MultipathController>().rightDoorOpened.rotation;

            doorsBegin[i].transform.Find("polySurface4").transform.localPosition = GetComponentInParent<MultipathController>().leftDoorOpened.position;
            doorsBegin[i].transform.Find("polySurface4").transform.rotation = GetComponentInParent<MultipathController>().leftDoorOpened.rotation;
        }
        doorsOpened = true;
    }
}
