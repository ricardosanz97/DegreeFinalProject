using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIController : Singleton<UIController>
{
    public static event Action<Vector3> OnGroundClicked;
    public static event Action OnSpawnBoogiesEnabled;
    public static event Action OnSpawnSquadEnabled;
    public static event Action<int> OnWrestlerClicked;
    public static event Action<Vector3> OnMoveSquadPositionSelected;
    public static event Action<InteractableBody> OnInteractableBodyPressed;

    public bool selectingBodyToCover = false;

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

    public bool OnMoveSquadPositionSelectedNull()
    {
        return OnMoveSquadPositionSelected == null;
    }
    public bool OnInteractableBodyPressedNull()
    {
        return OnInteractableBodyPressed == null;
    }

    public void UIHandleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")) && !selectingBodyToCover)
        {
            if (FindObjectOfType<SelectorMouseBehavior>() != null)
            {
                FindObjectOfType<SelectorMouseBehavior>().transform.position = hit.point;
            }
            if (Input.GetMouseButtonDown(0))
            {
                OnGroundClicked?.Invoke(hit.point);
                OnMoveSquadPositionSelected?.Invoke(hit.point);
            }
        }
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Npc")) && hit.transform.gameObject.GetComponent<BoogieWrestler>() && !selectingBodyToCover)
        {
            if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("clicking on a wrestler. ");
                BoogieWrestler boogieClicked = hit.transform.GetComponent<BoogieWrestler>();
                OnWrestlerClicked += boogieClicked.WrestlerClicked;
                OnWrestlerClicked?.Invoke(0);
            }

            else if (Input.GetMouseButtonUp(1))
            {
                BoogieWrestler boogieClicked = hit.transform.GetComponent<BoogieWrestler>();
                OnWrestlerClicked += boogieClicked.WrestlerClicked;
                OnWrestlerClicked?.Invoke(1);
            }
        }

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Body", "Npc")) && selectingBodyToCover)
        {
            if (FindObjectOfType<SelectorMouseBehavior>() != null)
            {
                FindObjectOfType<SelectorMouseBehavior>().transform.position = hit.point;
            }
            else
            {
                UIShowMouseSelector(SELECTION_TYPE.SquadCover);
            }
            if (Input.GetMouseButtonDown(0) && (hit.transform.GetComponent<InteractableBody>()))
            {
                OnInteractableBodyPressed?.Invoke(hit.transform.gameObject.GetComponent<InteractableBody>());  
            }
        }
        else if (selectingBodyToCover)
        {
            if (FindObjectOfType<SelectorMouseBehavior>() != null)
            {
                Destroy(FindObjectOfType<SelectorMouseBehavior>().gameObject);
            }
        }
    }

    public void UIHandleKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (FindObjectOfType<GenericPanelController>() != null)
            {
                Destroy(FindObjectOfType<GenericPanelController>().gameObject);
            }
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
                    UISpawnSquadDisabled();
                }
                BoogiesSpawner.SetCanSpawnSquad(false);
            }
            else
            {
                BoogiesSpawner.SpawnSquadDisabled();
                BoogiesSpawner.SetCanSpawnSquad(true);
                UISpawnSquadDisabled();
            }
            UISquadSelectorController.Create();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (OnMoveSquadPositionSelected != null)
            {
                OnMoveSquadPositionSelected -= BoogiesSpawner.CommanderSelected.MoveToPosition;
                BoogiesSpawner.CommanderSelected = null;
            }
            if (this.selectingBodyToCover)
            {
                this.selectingBodyToCover = false;
            }
            if (FindObjectOfType<SelectorMouseBehavior>() != null)
            {
                Destroy(FindObjectOfType<SelectorMouseBehavior>().gameObject);
                
            }

            if (FindObjectOfType<GenericPanelController>() != null)
            {
                Destroy(FindObjectOfType<GenericPanelController>().gameObject);
            }
        }
    }

    public void HandleSpawnSquadLogic()
    {
        BoogiesSpawner.SpawnSquadEnabled();
        OnSpawnSquadEnabled?.Invoke();
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

    public void OnSpawnSquadPositionSelected(Vector3 position)
    {
        FindObjectOfType<BoogiesSpawner>().CreateSquad(FindObjectOfType<BoogiesSpawner>().currentFormationSelected, position);
        UIHideMouseSelector();
        OnGroundClicked -= OnSpawnSquadPositionSelected;
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
