using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectorMouseBehavior : MonoBehaviour
{
    public Vector3 clickPosition;
    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            this.transform.position = hit.point;
            if (Input.GetMouseButtonDown(0) && FindObjectOfType<BoogiesSpawner>().selectorEnabled)
            {
                clickPosition = this.transform.position;
                FindObjectOfType<BoogiesSpawner>().HandleWrestlersSelect();
                //FindObjectOfType<BoogiesSpawner>().SpawnWrestlersSquad(hit.point);
                FindObjectOfType<UISquadSelectorController>().transform.Find("Background").gameObject.SetActive(true);
            }
        }
    }
}
