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
    public SquadConfiguration.Index initialIndexs;
    public SquadConfiguration.Index indexs;
    public SquadConfiguration.Index lastPos;
    public BoogieWrestlerCommander commander;
    public int listPosition;
    public GameObject leader;
    public bool isLeaderPosition;

    public virtual void WrestlerClicked(int clickButton)
    { 
        BoogiesSpawner.CommanderSelected = commander;
        UIController.OnWrestlerClicked -= WrestlerClicked;
        if (clickButton == 0 && currentWState == W_STATE.OnSquad)
        {
            UISquadOptionsController.Create(
            commander,
            () =>
            {
                UIController.I.UIShowMouseSelector(SELECTION_TYPE.SquadMovingPosition);
                UIController.OnMoveSquadPositionSelected += commander.MoveToPosition;
            },
            () =>
            {
                UISquadSelectorController.Create(this.commander);
            },
            () =>
            {
                this.leader.transform.Rotate(new Vector3(0, 1, 0) * 90f);
            },
            () =>
            {
                UIController.OnInteractableBodyPressed += commander.InteractableBodySelected;
                UIController.I.selectingBodyToCover = true;
                UIController.I.UIShowMouseSelector(SELECTION_TYPE.SquadCover);
            },
            () =>
            {
                commander.UncoverBody();
            },
            () =>
            {
                commander.ResetDefaultFormation();
            }
            );
        }
        else if (clickButton == 0 && currentWState != W_STATE.OnSquad)
        {
            UIIndividualWrestlerOptionsController.Create(() =>
            {
                UIController.I.selectingSquadToJoin = true;
                UIController.OnSelectingSquadJoin += SquadSelected;
            },
            () =>
            {
                Debug.Log("follow player. ");
            },
            () =>
            {
                Debug.Log("move position. ");
            },
            ()=>
            {
                Debug.Log("attack. ");
            });
        }
    }

    private void SquadSelected (BoogieWrestlerCommander bwc)
    {
        JoinSquad(bwc);
    }

    public virtual bool JoinSquad(BoogieWrestlerCommander bwc)
    {
        SquadConfiguration.Index newIndexs = new SquadConfiguration.Index(bwc.neededIndexs[Random.Range(0, bwc.neededIndexs.Count)].i, bwc.neededIndexs[Random.Range(0, bwc.neededIndexs.Count)].j);
        bwc.neededIndexs.Remove(bwc.neededIndexs.Find((x) => x.i == newIndexs.i && x.j == newIndexs.j));
        this.indexs = new SquadConfiguration.Index(newIndexs.i, newIndexs.j);
        this.commander = bwc;
        this.leader = bwc.leader.gameObject;
        this.transform.SetParent(bwc.transform.parent);
        bwc.squadWrestlers.Add(this);
        switch (this.wrestlerType)
        {
            case WRESTLER_TYPE.Close:
                bwc.currentSquadList[listPosition] = SquadConfiguration.SQUAD_ROL.Close;
                break;
            case WRESTLER_TYPE.Distance:
                bwc.currentSquadList[listPosition] = SquadConfiguration.SQUAD_ROL.Distance;
                break;
            case WRESTLER_TYPE.Giant:
                bwc.currentSquadList[listPosition] = SquadConfiguration.SQUAD_ROL.Giant;
                break;
        }
        bwc.neededIndexs.Remove(bwc.neededIndexs.Find((x) => x.i == this.indexs.i && x.j == this.indexs.j));
        this.currentWState = W_STATE.OnSquad;
        UIController.I.selectingSquadToJoin = false;
        UIController.OnSelectingSquadJoin -= SquadSelected;
        UIController.I.UIHideMouseSelector();
        return true;
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
        commander.neededIndexs.Add(new SquadConfiguration.Index(this.indexs.i, this.indexs.j));
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
        if (leader == this.gameObject)
        {
            return;
        }
        commander.currentSquadList[listPosition] = SquadConfiguration.SQUAD_ROL.None;
        //listPosition = -1;
        currentWState = W_STATE.CoveringPosition;
        this.transform.SetParent(null);
        commander.neededIndexs.Add(new SquadConfiguration.Index(this.indexs.i, this.indexs.j));

        commander.RemoveWrestlerSquad(this);
        leader = null;
        commander = null;
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
        Debug.Log("other = " + otherWrestler.wrestlerType.ToString());
        Debug.Log("we = " + this.wrestlerType.ToString());
        if (this.commander != otherWrestler && !this.commander.squadWrestlers.Contains(otherWrestler))//if it is not from our squad.
        {
            Debug.Log("hola1");
            return;
        }
        if (otherWrestler == this)//if it is not from our squad.
        {
            Debug.Log("hola2");
            return;
        }

        SquadConfiguration.SQUAD_ROL myRol = this.commander.currentSquadList[this.listPosition];
        SquadConfiguration.SQUAD_ROL hisRol = otherWrestler.commander.currentSquadList[otherWrestler.listPosition];

        Debug.Log("hola");
        this.commander.currentSquadList[this.listPosition] = hisRol;
        otherWrestler.commander.currentSquadList[otherWrestler.listPosition] = myRol;

        int auxListPos = this.listPosition;
        this.listPosition = otherWrestler.listPosition;
        otherWrestler.listPosition = auxListPos;

        UIController.I.selectingWrestlerToChange = false;
        UIController.I.UIHideMouseSelector();
        UIController.OnSelectingWrestlerChange -= ChangeSelectedWrestler;

        int i = this.indexs.i;
        int j = this.indexs.j;

        this.indexs = new SquadConfiguration.Index(otherWrestler.indexs.i, otherWrestler.indexs.j);

        otherWrestler.indexs = new SquadConfiguration.Index(i, j);

        bool aux = this.isLeaderPosition;
        this.isLeaderPosition = otherWrestler.isLeaderPosition;
        otherWrestler.isLeaderPosition = aux;
        if (otherWrestler.gameObject == commander.leader || this.gameObject == commander.leader)
        {
            Debug.Log("aqui entra");
            if (otherWrestler.gameObject == commander.leader)
            {
                //otherWrestler.isLeaderPosition = false;
                //this.isLeaderPosition = true;
                commander.leader = this.gameObject;
            }
            else if (this.gameObject == commander.leader)
            {
                //this.isLeaderPosition = false;
                //otherWrestler.isLeaderPosition = true;
                commander.leader = otherWrestler.gameObject;
            }
            SquadConfiguration.Index leadIndexs = new SquadConfiguration.Index(commander.leader.GetComponent<BoogieWrestler>().indexs.i, commander.leader.GetComponent<BoogieWrestler>().indexs.j);
            foreach (BoogieWrestler bw in commander.squadWrestlers)
            {
                bw.leader = commander.leader;
                bw.indexs.i -= leadIndexs.i;
                bw.indexs.j -= leadIndexs.j;
            }
        }
    }

    public void AssignAsLeader()
    {
        if (commander.coveringBody == null)
        {
            BoogieWrestler oldLeader = this.leader.GetComponent<BoogieWrestler>();
            if (oldLeader == this)
            {
                return;
            }
            ChangeSelectedWrestler(oldLeader);
        }

        else
        {
            SquadConfiguration.Index oldIndexs = new SquadConfiguration.Index(this.indexs.i, this.indexs.j);
            
            foreach (BoogieWrestler bw in commander.squadWrestlers)
            {
                if (bw == commander)
                {
                    bw.leader = this.commander.gameObject;
                    continue;
                }
                if (commander.coveringBody.GetComponent<BoogieWrestler>() != null && commander.coveringBody.GetComponent<BoogieWrestler>() == bw)
                {
                    bw.leader = this.commander.gameObject;
                    continue;
                }
                bw.leader = this.commander.gameObject;
                bw.indexs.i -= commander.indexs.i;
                bw.indexs.j -= commander.indexs.j;
            }
            commander.coveringBody = null;
            commander.indexs = new SquadConfiguration.Index(0, 0);
            AssignAsLeader();
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
        None
    }

    public virtual void Start()
    {
        commander = this.transform.parent.gameObject.GetComponentInChildren<BoogieWrestlerCommander>();

        currentWState = W_STATE.OnSquad;
    
        leader = commander.gameObject;
        ChangeIndexsRelativeToLeader();
        initialIndexs = new SquadConfiguration.Index(indexs.i, indexs.j);
        TakeInitialPosition();
    }

    public void ChangeIndexsRelativeToLeader()
    {
        indexs.i -= commander.leaderIndex.i;
        indexs.j -= commander.leaderIndex.j;
    }

    public void ChangeIndexsRelativeToBody()
    {
        indexs.i -= commander.bodyIndex.i;
        indexs.j -= commander.bodyIndex.j;
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
