using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MenuDoorBehavior : MonoBehaviour
{
    public GameObject leftDoor;
    public GameObject rightDoor;

    public Vector3 RightFinalRotation;
    public Vector3 LeftFinalRotation;

    private Vector3 LeftInitialRotation;
    private Vector3 RightInitialRotation;

    private void Start()
    {
        LeftInitialRotation = leftDoor.transform.localRotation.eulerAngles;
        RightInitialRotation = rightDoor.transform.localRotation.eulerAngles;
    }

    public void Trigger()
    {
        leftDoor.transform.DOLocalRotate(LeftFinalRotation, 2f);
        rightDoor.transform.DOLocalRotate(RightFinalRotation, 2f);
    }
    
    public void RestartDoor()
    {
        leftDoor.transform.DOLocalRotate(LeftInitialRotation, 0f);
        rightDoor.transform.DOLocalRotate(RightInitialRotation, 0f);
    }
}
