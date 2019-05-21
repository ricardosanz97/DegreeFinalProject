using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTrigger : MonoBehaviour
{
    public enum TYPE
    {
        FirstEntity,
        SecondEntity,
        Door,
        DoorEnd
    }
    public TYPE triggerType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<MenuCameraMovement>())
        {
            switch (triggerType)
            {
                case TYPE.FirstEntity:
                    FindObjectOfType<MenuManager>().TriggerFirst();
                    break;
                case TYPE.SecondEntity:
                    FindObjectOfType<MenuManager>().TriggerSecond();
                    break;
                case TYPE.Door:
                    FindObjectOfType<MenuManager>().TriggerDoor();
                    break;
                case TYPE.DoorEnd:
                    FindObjectOfType<MenuManager>().RestartAnimation();
                    break;
            }
        }
    }
}
