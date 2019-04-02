using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class UIController : Singleton<UIController>
{
    public static event Action<Vector3> OnGroundClicked;
    public static event Action OnSpawnBoogiesEnabled;
    public static event Action OnSpawnSquadEnabled;
    public static event Action<int> OnWrestlerClicked;
    public static event Action<Vector3> OnMoveSquadPositionSelected;
    public static event Action<InteractableBody> OnInteractableBodyPressed;
    public static event Action<BoogieWrestler> OnSelectingWrestlerChange;
    public static event Action<BoogieWrestlerCommander> OnSelectingSquadJoin;
    public static event Action<Vector3> OnMovePositionSelected;

    public bool selectingBodyToCover = false;
    public bool selectingWrestlerToChange = false;
    public bool selectingSquadToJoin = false;
    public bool writing = false;

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

    public void ResetEvents()
    {
        OnGroundClicked = null;
        OnSpawnBoogiesEnabled = null;
        OnSpawnSquadEnabled = null;
        OnWrestlerClicked = null;
        OnMoveSquadPositionSelected = null;
        OnInteractableBodyPressed = null;
        OnSelectingWrestlerChange = null;
        OnSelectingSquadJoin = null;
        OnMovePositionSelected = null;
    }

    public void UIHandleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")) && !selectingBodyToCover && !selectingWrestlerToChange)
        {
            if (SelectorMouseBehavior.IsActive())
            {
                FindObjectOfType<SelectorMouseBehavior>().transform.position = hit.point;
            }
            if (Input.GetMouseButtonDown(0))
            {
                OnGroundClicked?.Invoke(hit.point);
                OnMoveSquadPositionSelected?.Invoke(hit.point);
                OnMovePositionSelected?.Invoke(hit.point);
            }
        }
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Npc")) && hit.transform.gameObject.GetComponent<BoogieWrestler>() && !selectingSquadToJoin && !selectingBodyToCover && !selectingWrestlerToChange)
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

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Body", "Npc")) && !selectingSquadToJoin && selectingBodyToCover && !selectingWrestlerToChange)
        {
            if (SelectorMouseBehavior.IsActive())
            {
                FindObjectOfType<SelectorMouseBehavior>().transform.position = hit.point;
            }
            else
            {
                UIShowMouseSelector(SELECTION_TYPE.SquadCover);
            }
            if (Input.GetMouseButtonDown(0) && (hit.transform.GetComponent<InteractableBody>()))
            {
                if ((hit.transform.GetComponent<BoogieWrestler>() && hit.transform.GetComponent<BoogieWrestler>().leader == hit.transform.gameObject) || 
                    (hit.transform.GetComponent<BoogieWrestler>() && hit.transform.GetComponent<BoogieWrestler>().commander == null))
                {
                    return;
                }
                OnInteractableBodyPressed?.Invoke(hit.transform.gameObject.GetComponent<InteractableBody>());  
            }
        }
        else if (selectingBodyToCover && !selectingWrestlerToChange)
        {
            if (SelectorMouseBehavior.IsActive())
            {
                Debug.Log("destroyed");
                SelectorMouseBehavior.Destroy();
            }
        }
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Npc")) && !selectingSquadToJoin && !selectingBodyToCover && selectingWrestlerToChange)
        {
            BoogieWrestler bw = hit.transform.GetComponent<BoogieWrestler>() ?? null;
            if (bw == null || bw.currentWState != W_STATE.OnSquad)//if is not a boogie wrestler or it is not in a squad or it is a squad leader...
            {
                Debug.Log("destroyed");
                SelectorMouseBehavior.Destroy();
                return;
            }

            if (SelectorMouseBehavior.IsActive())
            {
                FindObjectOfType<SelectorMouseBehavior>().transform.position = hit.point;
            }
            else
            {
                UIShowMouseSelector(SELECTION_TYPE.SelectingSquadWrestlerChange);
            }
            if (Input.GetMouseButtonDown(0))
            {
                OnSelectingWrestlerChange?.Invoke(hit.transform.GetComponent<BoogieWrestler>());
            }
            
        }
        else if (!selectingBodyToCover && selectingWrestlerToChange)
        {
            if (SelectorMouseBehavior.IsActive())
            {
                Debug.Log("destroyed");
                SelectorMouseBehavior.Destroy();
            }
        }

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Npc")) && selectingSquadToJoin && !selectingBodyToCover && !selectingWrestlerToChange)
        {
            BoogieWrestlerCommander bwc = hit.transform.GetComponent<BoogieWrestlerCommander>() ?? null;
            if (bwc == null)
            {
                Debug.Log("destroyed");
                SelectorMouseBehavior.Destroy();
                return;
            }

            if (SelectorMouseBehavior.IsActive())
            {
                FindObjectOfType<SelectorMouseBehavior>().transform.position = hit.point;
            }
            else
            {
                UIShowMouseSelector(SELECTION_TYPE.SelectingSquadWrestlerChange);
            }
            if (Input.GetMouseButtonDown(0))
            {
                OnSelectingSquadJoin?.Invoke(bwc);
            }
        }
        /*
        else if (!selectingSquadToJoin)
        {
            if (SelectorMouseBehavior.IsActive())
            {
                Debug.Log("destroyed");
                SelectorMouseBehavior.Destroy();
            }
        }
        */
    }

    public void UIHandleKeyInput()
    {
        if (writing)
        {
            return;
        }
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
            Debug.Log("Open popup formations because we press G key");
            UISquadSelectorController.Create();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ResetEvents();
            if (this.selectingBodyToCover)
            {
                this.selectingBodyToCover = false;
            }
            if (this.selectingSquadToJoin)
            {
                this.selectingSquadToJoin = false;
            }
            if (this.selectingWrestlerToChange)
            {
                this.selectingSquadToJoin = false;
            }

            if (OnMoveSquadPositionSelected != null)
            {
                OnMoveSquadPositionSelected = null;
                BoogiesSpawner.CommanderSelected = null;
            }
            if (OnSelectingWrestlerChange != null)
            {
                OnSelectingWrestlerChange = null;
                this.selectingWrestlerToChange = false;
                if (SelectorMouseBehavior.IsActive())
                {
                    Debug.Log("destroy");
                    SelectorMouseBehavior.Destroy();
                }
            }
            if (this.selectingBodyToCover)
            {
                this.selectingBodyToCover = false;
            }
            if (SelectorMouseBehavior.IsActive())
            {
                Debug.Log("destroyed");
                SelectorMouseBehavior.Destroy();
                
            }

            if (FindObjectOfType<GenericPanelController>() != null)
            {
                FindObjectOfType<GenericPanelController>().ClosePanel();
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
        if (SelectorMouseBehavior.IsActive())
        {
            Debug.Log("destroying selectorMouse");
            SelectorMouseBehavior.Destroy();
        }
    }

    public void UIShowMouseSelector(SELECTION_TYPE type)
    {
        if (!SelectorMouseBehavior.IsActive())
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
        //FindObjectOfType<BoogiesSpawner>().CreateSquad(FindObjectOfType<BoogiesSpawner>().currentFormationSelected, position);
        FindObjectOfType<BoogiesSpawner>().SpawnSquad(position, FindObjectOfType<SquadConfiguration>().currentSquadSelected);
        FindObjectOfType<SquadConfiguration>().currentSquadSelected = null;
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
