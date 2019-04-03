using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;

public enum W_STATE
{
    OnSquad,
    Moving,
    FollowingPlayer,
    CoveringPosition,
    Attacking
}

public enum ON_SQUAD_STATE
{
    Observing,
    Attacking,
}

public enum TEAM
{
    A,
    B
}

public class BoogieWrestler : Boogie
{
    public WRESTLER_TYPE wrestlerType;
    public W_STATE currentWState;
    public ON_SQUAD_STATE state;
    [HideInInspector]public SquadConfiguration.Index initialIndexs;
    [HideInInspector]public SquadConfiguration.Index indexs;
    [HideInInspector]public SquadConfiguration.Index lastPos;
    public BoogieWrestlerCommander commander;
    [HideInInspector]public int listPosition;
    public GameObject leader;
    [HideInInspector]public bool isLeaderPosition;

    public TEAM team;
    public float health;
    public float attackSpeed;
    public float visionDistance;
    public float fieldView;
    public float attackRange;
    public float attackDamage;

    public float minOffset;
    public float maxOffset;
    public float probabilityVariateOffset;
    public float timeVariateOffset;

    public float timeToFindTargets;

    public float probabilityPreferences;
    public WRESTLER_TYPE[] Preferences;

    public List<Transform> visibleTargets = new List<Transform>();
    public BoogieWrestler wrestlerSelected = null;

    private void AssignConfiguration()
    {
        WrestlersConfiguration Wcfg = commander.squadInfo.customSquadConfiguration;

        minOffset = Wcfg.minOffset;
        maxOffset = Wcfg.maxOffset;
        probabilityVariateOffset = Wcfg.probabilityVariateOffset;
        timeVariateOffset = Wcfg.timeVariateOffset;

        switch (wrestlerType)
        {
            case WRESTLER_TYPE.Close:
                health = Wcfg.closeHealth;
                attackSpeed = Wcfg.closeAttackSpeed;
                visionDistance = Wcfg.closeVisionDistance;
                fieldView = Wcfg.closeFieldView;
                attackRange = Wcfg.closeAttackRange;
                attackDamage = Wcfg.closeAttackDamage;
                minSpeed = Wcfg.closeMinSpeed;
                maxSpeed = Wcfg.closeMaxSpeed;
                probabilityChangeSpeed = Wcfg.closeProbabilityVariateSpeed;
                timeToChangeSpeed = Wcfg.closeTimeVariateSpeed;
                probabilityPreferences = Wcfg.closeProbabilityPreferences;
                Preferences = Wcfg.ClosePreferences;
                timeToFindTargets = Wcfg.closeTimeFindTargets;
                break;
            case WRESTLER_TYPE.Distance:
                health = Wcfg.distanceHealth;
                attackSpeed = Wcfg.distanceAttackSpeed;
                visionDistance = Wcfg.distanceVisionDistance;
                fieldView = Wcfg.distanceFieldView;
                attackRange = Wcfg.distanceAttackRange;
                attackDamage = Wcfg.distanceAttackDamage;
                minSpeed = Wcfg.distanceMinSpeed;
                maxSpeed = Wcfg.distanceMaxSpeed;
                probabilityChangeSpeed = Wcfg.distanceProbabilityVariateSpeed;
                timeToChangeSpeed = Wcfg.distanceTimeVariateSpeed;
                probabilityPreferences = Wcfg.distanceProbabilityPreferences;
                Preferences = Wcfg.DistancePreferences;
                timeToFindTargets = Wcfg.distanceTimeFindTargets;
                break;
            case WRESTLER_TYPE.Commander:
                health = Wcfg.commanderHealth;
                attackSpeed = Wcfg.commanderAttackSpeed;
                visionDistance = Wcfg.commanderVisionDistance;
                fieldView = Wcfg.commanderFieldView;
                attackRange = Wcfg.commanderAttackRange;
                attackDamage = Wcfg.commanderAttackDamage;
                minSpeed = Wcfg.commanderMinSpeed;
                maxSpeed = Wcfg.commanderMaxSpeed;
                probabilityChangeSpeed = Wcfg.commanderProbabilityVariateSpeed;
                timeToChangeSpeed = Wcfg.commanderTimeVariateSpeed;
                probabilityPreferences = Wcfg.commanderProbabilityPreferences;
                Preferences = Wcfg.CommanderPreferences;
                timeToFindTargets = Wcfg.commanderTimeFindTargets;
                break;
            case WRESTLER_TYPE.Giant:
                health = Wcfg.giantHealth;
                attackSpeed = Wcfg.giantAttackSpeed;
                visionDistance = Wcfg.giantVisionDistance;
                fieldView = Wcfg.giantFieldView;
                attackRange = Wcfg.giantAttackRange;
                attackDamage = Wcfg.giantAttackDamage;
                minSpeed = Wcfg.giantMinSpeed;
                maxSpeed = Wcfg.giantMaxSpeed;
                probabilityChangeSpeed = Wcfg.giantProbabilityVariateSpeed;
                timeToChangeSpeed = Wcfg.giantTimeVariateSpeed;
                probabilityPreferences = Wcfg.giantProbabilityPreferences;
                Preferences = Wcfg.CommanderPreferences;
                timeToFindTargets = Wcfg.giantTimeFindTargets;
                break;
        }

        SetConfiguration();
    }

    public void SetConfiguration()
    {
        GetComponent<FieldOfView>().viewRadius = visionDistance;
        GetComponent<FieldOfView>().viewAngle = fieldView;
        GetComponent<FieldOfView>().StartVision(timeToFindTargets);
    }

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
                UIController.I.UIShowMouseSelector(SELECTION_TYPE.SquadMovingPosition);
                UIController.OnMovePositionSelected += MoveToPosition;
            },
            ()=>
            {
                Debug.Log("attack. ");
            });
        }
    }

    private void MoveToPosition(Vector3 point)
    {
       // UIController.OnMovePositionSelected -= MoveToPosition;
        randomPoint = point;
        if (FindObjectOfType<xMarkerBehavior>() != null)
        {
            Destroy(FindObjectOfType<xMarkerBehavior>().gameObject);
        }
        UIController.I.UIShowXMarker(randomPoint);
        _agent.SetDestination(randomPoint);
        this.currentWState = W_STATE.Moving;
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

        Debug.Log("CHANGING WRESTLERS POSITIONS: " + this.listPosition + ", " + otherWrestler.listPosition);
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

        AssignConfiguration();
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
        if (commander.currentSquadState == SQUAD_STATE.Moving)
        {
            return;
        }
        if (leader != null)
        {
            this.transform.rotation = leader.transform.rotation;
        }
        
        Vector3 offset = new Vector3(indexs.j * commander.wrestlersOffset, this.transform.position.y, indexs.i * commander.wrestlersOffset);
        randomPoint = leader.transform.position + this.transform.TransformDirection(offset);
        _agent.SetDestination(randomPoint);
    }

    public void MoveHere(Vector3 where)
    {
        Vector3 offset = new Vector3(indexs.j * commander.wrestlersOffset, this.transform.position.y, indexs.i * commander.wrestlersOffset);
        randomPoint = where + this.transform.TransformDirection(offset);
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
        if (currentWState == W_STATE.OnSquad && state == ON_SQUAD_STATE.Observing && this.leader != this.gameObject) TakeInitialPosition();
        if (currentWState != W_STATE.OnSquad)
        {
            if (Vector3.Distance(this.transform.position, randomPoint) <= 0.5f)
            {
                this.currentWState = W_STATE.CoveringPosition;
                if (FindObjectOfType<xMarkerBehavior>() != null)
                {
                    Destroy(FindObjectOfType<xMarkerBehavior>().gameObject);
                }
            }
        }

        if (this.state == ON_SQUAD_STATE.Observing)
        {
            if (commander.visibleTargets.Count > 0)
            {
                StartCoroutine(ChooseAnEnemy());
            }
        }

        if (this.state == ON_SQUAD_STATE.Attacking)
        {
            _agent.SetDestination(wrestlerSelected.transform.position);
        }

        //TODO: this is for debuging
        this.team = GetComponentInParent<SquadTeam>().team;
    }

    IEnumerator ChooseAnEnemy()
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 1f));
        if (this.visibleTargets.Count > 0)
        {
            TryFindingInOurList(0);
        }
        else
        {
            TryFindingInCommonList(0);
        }
    }

    void TryFindingInOurList(int i)
    {
        if (Random.value <= probabilityPreferences)
        {
            if (i > Preferences.Length)
            {
                return;
            }
            Transform aTry = this.visibleTargets.Find((x) => x.GetComponent<BoogieWrestler>().wrestlerType == Preferences[i]);
            if (aTry != null)
            {
                wrestlerSelected = aTry.GetComponent<BoogieWrestler>();
                state = ON_SQUAD_STATE.Attacking;
                _agent.SetDestination(wrestlerSelected.transform.position);
                return;
            }
            else
            {
                TryFindingInOurList(i+1);
            }
        }
        else
        {
            TryFindingInCommonList(0);
        }
    }

    void TryFindingInCommonList(int i)
    {
        if (Random.value > probabilityPreferences)
        {
            //find in common
            if (i >= Preferences.Length)
            {
                return;
            }
            Transform aTry = this.commander.visibleTargets.Find((x) => x.GetComponent<BoogieWrestler>().wrestlerType == Preferences[i]);
            if (aTry != null)
            {
                wrestlerSelected = aTry.GetComponent<BoogieWrestler>();
                state = ON_SQUAD_STATE.Attacking;
                _agent.SetDestination(wrestlerSelected.transform.position);
                return;
            }
            else
            {
                TryFindingInCommonList(i + 1);
            }
        }
        else
        {
            TryFindingInOurList(0);
        }
    }
}
