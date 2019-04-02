using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericPanelController : MonoBehaviour
{
    public void ClosePanel()
    {
        UIController.I.popupsOpened.Remove(this);
        this.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }

    public void OpenPanel()
    {
        UIController.I.popupsOpened.Add(this);
        this.gameObject.SetActive(true);
    }
}
