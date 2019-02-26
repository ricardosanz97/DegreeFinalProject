using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIController : Singleton<UIController>
{
    public static event Action<Vector3> OnGroundClicked;
    public static event Action OnSpawnBoogiesEnabled;
    public static event Action OnSpawnSquadEnabled;
    public static event Action OnTroopClicked;

    private void Start()
    {
        OnSpawnBoogiesEnabled += UISpawnBoogiesEnabled;
        OnSpawnSquadEnabled += UISpawnSquadEnabled;
    }

    void Update()
    {
        UIHandleClick();
        UIHandleKeyInput();
    }

    public void UIHandleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            if (FindObjectOfType<SelectorMouseBehavior>() != null)
            {
                FindObjectOfType<SelectorMouseBehavior>().transform.position = hit.point;
            }
            if (Input.GetMouseButtonDown(0))
            {
                OnGroundClicked?.Invoke(hit.point);
            }
        }
        if (Physics.Raycast(ray, out hit, Mathf.Infinity) && hit.transform.gameObject.GetComponent<BoogieWrestler>())
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnTroopClicked?.Invoke();
            }
        }
    }

    public void UIHandleKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (BoogiesSpawner.CanSpawnBoogies)
            {
                if (!BoogiesSpawner.CanSpawnSquad)
                {
                    BoogiesSpawner.SetCanSpawnSquad(true);
                    BoogiesSpawner.SpawnSquadDisabled();
                }
                BoogiesSpawner.SpawnBoogiesEnabled();
                BoogiesSpawner.SetCanSpawnBoogies(false);
                OnSpawnBoogiesEnabled?.Invoke();
            }
            else
            {
                BoogiesSpawner.SpawnBoogiesDisabled();
                BoogiesSpawner.SetCanSpawnBoogies(true);
                UISpawnBoogiesDisabled();
            }
            
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (BoogiesSpawner.CanSpawnSquad)
            {
                if (!BoogiesSpawner.CanSpawnBoogies)
                {
                    BoogiesSpawner.SetCanSpawnBoogies(true);
                    BoogiesSpawner.SpawnBoogiesDisabled();
                }
                BoogiesSpawner.SpawnSquadEnabled();
                BoogiesSpawner.SetCanSpawnSquad(false);
                OnSpawnSquadEnabled?.Invoke();
            }
            else
            {
                BoogiesSpawner.SpawnSquadDisabled();
                BoogiesSpawner.SetCanSpawnSquad(true);
                UISpawnSquadDisabled();
            }
        }
    }

    public void UISpawnBoogiesEnabled()
    {
        UIShowMouseSelector(SELECTION_TYPE.SpawnBoogiesArea);
    }

    public void UISpawnBoogiesDisabled()
    {
        UIHideMouseSelector();
    }

    public void UISpawnSquadEnabled()
    {
        UIShowMouseSelector(SELECTION_TYPE.SquadSpawnPosition);
    }

    public void UISpawnSquadDisabled()
    {
        UIHideMouseSelector();
    }

    public void UIHideMouseSelector()
    {
        if (FindObjectOfType<SelectorMouseBehavior>() != null)
        {
            SelectorMouseBehavior.Destroy();
        }
    }

    public void UIShowMouseSelector(SELECTION_TYPE type)
    {
        if (FindObjectOfType<SelectorMouseBehavior>() == null)
        {
            SelectorMouseBehavior.Create(type);
        }
        else
        {
            UIHideMouseSelector();
            SelectorMouseBehavior.Create(type);
        }
    }

    public void UIShowSquadConfigSelector(Vector3 position)
    {
        UIShowXMarker(position);
        FindObjectOfType<UISquadSelectorController>().transform.Find("Background").gameObject.SetActive(true);
        UIHideMouseSelector();
        OnGroundClicked -= UIShowSquadConfigSelector;
    }

    public void UIHideSquadConfigSelector()
    {
        if (FindObjectOfType<UISquadSelectorController>().transform.Find("Background").gameObject.activeInHierarchy)
        {
            FindObjectOfType<UISquadSelectorController>().transform.Find("Background").gameObject.SetActive(false);
        }
    }

    public void UIHideXMarker()
    {
        if (FindObjectOfType<xMarkerBehavior>() != null)
        {
            Destroy(FindObjectOfType<xMarkerBehavior>().gameObject);
        }
    }

    public void UIShowXMarker(Vector3 position)
    {
        Instantiate(Resources.Load("Prefabs/UI/xMarker"), position, Quaternion.identity);
    }

    public void UIHideSquadSelector()
    {
        Destroy(FindObjectOfType<xMarkerBehavior>().gameObject);
        FindObjectOfType<UISquadSelectorController>().transform.Find("Background").gameObject.SetActive(false);
    }
}
