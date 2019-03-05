using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericPanelController : MonoBehaviour
{
    public void ClosePanel()
    {
        this.gameObject.SetActive(false);
    }

    public void OpenPanel()
    {
        this.gameObject.SetActive(true);
    }
}
