using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public enum W_STATE
{
    OnSquad,
    Moving,
    FollowingPlayer,
    CoveringPosition,
    Attacking
}

public class BoogieWrestler : Boogie
{
    public WRESTLER_TYPE wrestlerType;
    public W_STATE currentWState;
    public SquadConfiguration.Index indexs;
    public BoogieWrestlerCommander commander;
    public int listPosition;
    public GameObject leader;

    public virtual void WrestlerClicked(int clickButton)
    {
        CancelEventSubscriptions();
        BoogiesSpawner.CommanderSelected = commander;
        UIController.OnWrestlerClicked -= WrestlerClicked;
        if (clickButton == 0)
        {
            UISquadOptionsController.Create(() =>
            {
                Debug.Log("moving squad. ");
                UIController.I.UIShowMouseSelector(SELECTION_TYPE.SquadMovingPosition);
                UIController.OnMoveSquadPositionSelected += commander.MoveToPosition;
            },
            () =>
            {
                Debug.Log("changing formation. ");
                UISquadSelectorController.Create(this.commander.currentSquadList);
            },
            () =>
            {
                Debug.Log("rotation formation. ");
                this.leader.transform.Rotate(new Vector3(0, 1, 0) * 90f);
            },
            () =>
            {
                Debug.Log("cover boogie. ");
                UIController.OnInteractableBodyPressed += commander.InteractableBodySelected;
                UIController.I.selectingBodyToCover = true;
                UIController.I.UIShowMouseSelector(SELECTION_TYPE.SquadCover);
            }
            );
        }
    }

    private void CancelEventSubscriptions()
    {
        if (UIController.I.OnInteractableBodyPressedNull()) UIController.OnInteractableBodyPressed -= commander.InteractableBodySelected;
        if (UIController.I.OnMoveSquadPositionSelectedNull()) UIController.OnMoveSquadPositionSelected -= commander.MoveToPosition;
    }

    public override void BackToPlayer()
    {
    }

    public void FollowPlayer()
    {
        commander.currentSquadList[listPosition] = SquadConfiguration.SQUAD_ROL.None;
        listPosition = -1;
        currentWState = W_STATE.FollowingPlayer;
        this.transform.SetParent(null);
        if (leader == this.gameObject)
        {
            foreach (BoogieWrestler bw in this.commander.squadWrestlers)
            {
                if (bw != this)
                {
                    bw.leader = this.commander.gameObject;
                    bw.ChangeIndexsRelativeToLeader();
                }
                else
                {
                    commander.RemoveWrestlerSquad(this);
                }
            }
            leader = null;
            commander = null;
        }
        else
        {
            commander.RemoveWrestlerSquad(this);
            leader = null;
            commander = null;
        }
        StartCoroutine(MoveAroundPlayer());
    }

    public void BreakFormation()
    {
        commander.currentSquadList[listPosition] = SquadConfiguration.SQUAD_ROL.None;
        listPosition = -1;
        currentWState = W_STATE.CoveringPosition;
        this.transform.SetParent(null);
        if (leader == this.gameObject)
        {
            foreach (BoogieWrestler bw in this.commander.squadWrestlers)
            {
                if (bw != this)
                {
                    bw.leader = this.commander.gameObject;
                    bw.ChangeIndexsRelativeToLeader();
                }
                else
                {
                    commander.RemoveWrestlerSquad(this);
                }
            }
            leader = null;
            commander = null;
        }
        else
        {
            commander.RemoveWrestlerSquad(this);
            leader = null;
            commander = null;
        }
    }

    public void ChangePosition()
    {
        UIController.I.selectingWrestlerToChange = true;
        currentWState = W_STATE.OnSquad;
        UIController.I.UIShowMouseSelector(SELECTION_TYPE.SelectingSquadWrestlerChange);
        UIController.OnSelectingWrestlerChange += ChangeSelectedWrestler;
    }

    public void ChangeSelectedWrestler(BoogieWrestler otherWrestler)
    {
        if ((this.commander != otherWrestler && !this.commander.squadWrestlers.Contains(otherWrestler)) || otherWrestler == this)//if it is not from our squad.
        {
            return;
        }
        if (otherWrestler.wrestlerType == WRESTLER_TYPE.Commander && this.commander != otherWrestler)
        {
            return;
        }

        UIController.I.selectingWrestlerToChange = false;
        UIController.I.UIHideMouseSelector();
        UIController.OnSelectingWrestlerChange -= ChangeSelectedWrestler;

        int i = this.indexs.i;
        int j = this.indexs.j;

        int auxListPosition = this.listPosition;

        SquadConfiguration.SQUAD_ROL myRol = commander.currentSquadList[this.listPosition];
        SquadConfiguration.SQUAD_ROL hisRol = otherWrestler.commander.currentSquadList[otherWrestler.listPosition];

        this.indexs = new SquadConfiguration.Index(otherWrestler.indexs.i, otherWrestler.indexs.j);
        commander.currentSquadList[this.listPosition] = hisRol;
        commander.currentSquadList[otherWrestler.listPosition] = myRol;

        this.listPosition = otherWrestler.listPosition;
        otherWrestler.indexs = new SquadConfiguration.Index(i, j);
        otherWrestler.listPosition = auxListPosition;
    }

    public void AssignAsLeader()
    {
        BoogieWrestler oldLeader = this.leader.GetComponent<BoogieWrestler>();
        int i = oldLeader.indexs.i;
        int j = oldLeader.indexs.j;

        SquadConfiguration.SQUAD_ROL myRol = this.commander.currentSquadList[this.listPosition];
        SquadConfiguration.SQUAD_ROL hisRol = oldLeader.commander.currentSquadList[oldLeader.listPosition];

        this.commander.currentSquadList[listPosition] = hisRol;
        this.commander.currentSquadList[oldLeader.listPosition] = myRol;

        int auxListPos = listPosition;
        this.listPosition = oldLeader.listPosition;
        oldLeader.listPosition = auxListPos;
        oldLeader.indexs = new SquadConfiguration.Index(this.indexs.i, this.indexs.j);
        this.indexs = new SquadConfiguration.Index(i, j);

        this.commander.leader = this.gameObject;


        foreach (BoogieWrestler bw in commander.squadWrestlers)
        {
            bw.leader = this.gameObject;
        }

        //commander.ChangeIndexsRelativeToLeader();
        commander.TakeInitialPosition();
    }

    public override void OnObjectiveSelected()
    {

    }

    public enum WRESTLER_TYPE
    { 
        Commander,
        Distance,
        Close,
        Giant,
        None
    }

    public virtual void Start()
    {
        commander = this.transform.parent.gameObject.GetComponentInChildren<BoogieWrestlerCommander>();

        currentWState = W_STATE.OnSquad;

        if (commander.hasPlayer)
        {
            leader = FindObjectOfType<BoogiesSpawner>().gameObject;
        }
        else
        {
            leader = commander.gameObject;
        }
        ChangeIndexsRelativeToLeader();
        TakeInitialPosition();
    }

    public void ChangeIndexsRelativeToLeader()
    {
        indexs.i -= commander.leaderIndex.i;
        indexs.j -= commander.leaderIndex.j;
    }

    public void TakeInitialPosition() //take position in the squad.
    {
        if (leader != null)
        {
            this.transform.rotation = leader.transform.rotation;
        }
        
        Vector3 offset = new Vector3(indexs.j * commander.distanceBetweenUs, this.transform.position.y, indexs.i * commander.distanceBetweenUs);
        randomPoint = leader.transform.position + this.transform.TransformDirection(offset);
        _agent.SetDestination(randomPoint);
    }

    public IEnumerator MoveAroundPlayer()
    {
        while (currentWState == W_STATE.FollowingPlayer)
        {
            Vector3 playerPos = FindObjectOfType<BoogiesSpawner>().gameObject.transform.position;
            Vector3 randomPos = new Vector3(Random.Range(playerPos.x - 5f, playerPos.x + 5f), playerPos.y, Random.Range(playerPos.z - 5f, playerPos.z + 5f));
            randomPoint = randomPos;
            _agent.SetDestination(randomPoint);
            yield return new WaitForSeconds(Random.Range(1.5f, 3f));
            yield return null;
        }
    }

    public virtual void Update()
    {
        if (currentWState == W_STATE.OnSquad && this.leader != this.gameObject) TakeInitialPosition();
    }
}
