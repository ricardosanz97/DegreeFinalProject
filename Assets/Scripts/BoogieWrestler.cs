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
    public GameObject leader;

    private bool followPlayer = false;

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
                UISquadSelectorController.Create(commander.formation);
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
        UIController.I.selectingWrestlerToChange = false;
        UIController.I.UIHideMouseSelector();
        UIController.OnSelectingWrestlerChange -= ChangeSelectedWrestler;

        int i = this.indexs.i;
        int j = this.indexs.j;

        this.indexs = new SquadConfiguration.Index(otherWrestler.indexs.i, otherWrestler.indexs.j);
        if (otherWrestler.gameObject == commander.gameObject)
        {
            commander.leaderIndex = new SquadConfiguration.Index(i, j);
        }
        else
        {
            otherWrestler.indexs = new SquadConfiguration.Index(i, j);
        }
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
        followPlayer = true;
        TakeInitialPosition();
    }

    public void ChangeIndexsRelativeToLeader()
    {
        indexs.i -= commander.leaderIndex.i;
        indexs.j -= commander.leaderIndex.j;
    }

    public void TakeInitialPosition()
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
