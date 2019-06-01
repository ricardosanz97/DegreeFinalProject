using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Events;

public enum TEAM
{
    A,
    B
}

public enum STATE
{
    OnSquadObserving,
    OnSquadAttacking,
    OnSquadCovering,
    OnSquadCoveringMoving,
    OnSquadMoving,
    OnSquadRunningAway,
    AloneObserving,
    AloneAttacking,
    AloneFollowingPlayer,
    AloneMoving,
    BackToPlayer
}

public class BoogieWrestler : Boogie
{
    public WRESTLER_TYPE wrestlerType;
    public SquadConfiguration.Index initialIndexs;
    public SquadConfiguration.Index indexs;
    [HideInInspector]public SquadConfiguration.Index lastPos;
    public BoogieWrestlerCommander commander;
    public int listPosition;
    public GameObject leader;
    public bool isLeaderPosition;

    public TEAM team;
    public STATE currentState;
    [HideInInspector] public float attackSpeed;
    [HideInInspector] public float visionDistance;
    [HideInInspector] public float fieldView;
    [HideInInspector] public float attackRange;
    [HideInInspector] public int attackDamage;

    [HideInInspector] public float minOffset;
    [HideInInspector] public float maxOffset;
    [HideInInspector] public float probabilityVariateOffset;
    [HideInInspector] public float timeVariateOffset;

    [HideInInspector] public float timeToFindTargets;

    [HideInInspector] public float minTimeChangeObjective;
    [HideInInspector] public float maxTimeChangeObjective;
    [HideInInspector] public float probChangeObjective;

    [HideInInspector] public float probabilityPreferences;
    [HideInInspector] public float minTimeCoverAgain;
    [HideInInspector] public float maxTimeCoverAgain;

    public bool closeEnoughToAttack = false;
    [HideInInspector]public float percentHpToCover;

    public WRESTLER_TYPE[] Preferences;

    public List<Transform> visibleTargets = new List<Transform>();
    public AttackTarget targetSelected = null;
    public List<BoogieWrestler> wrestlersAttackingMe = new List<BoogieWrestler>();

    public BoogieWrestler helpingBoogie = null;

    public bool playerSelected = false;

    public int coroutinesAttackEnabled = 0;
    public int coroutinesGoToEnemyEnabled = 0;

    public UnityEvent OnDieEvent;

    public string key;
    //public bool focusOtherCommander = false;

    public override void OnEnable()
    {
        base.OnEnable();
        PlayerHealth.OnPlayerDead += PlayerDead;
    }

    private void PlayerDead()
    {
        playerSelected = false;
        if (targetSelected != null && targetSelected.GetComponent<PlayerHealth>())
        {
            targetSelected = null;
        }

        if (commander != null)
        {
            currentState = STATE.OnSquadObserving;
        }
        else
        {
            currentState = STATE.AloneObserving;
        }
    }

    private void AssignConfiguration()
    {
        WrestlersConfiguration Wcfg = commander.squadInfo.customSquadConfiguration;

        minOffset = Wcfg.minOffset;
        maxOffset = Wcfg.maxOffset;
        
        probabilityVariateOffset = Wcfg.probabilityVariateOffset;
        timeVariateOffset = Wcfg.timeVariateOffset;
        minTimeCoverAgain = Wcfg.minTimeCoverAgain;
        maxTimeCoverAgain = Wcfg.maxTimeCoverAgain;

        switch (wrestlerType)
        {
            case WRESTLER_TYPE.Close:
                health = Wcfg.closeHealth;
                attackSpeed = Wcfg.closeAttackSpeed;
                visionDistance = Wcfg.closeVisionDistance;
                fieldView = Wcfg.closeFieldView;
                attackDamage = Wcfg.closeAttackDamage;
                minSpeed = Wcfg.closeMinSpeed;
                maxSpeed = Wcfg.closeMaxSpeed;
                probabilityChangeSpeed = Wcfg.closeProbabilityVariateSpeed;
                timeToChangeSpeed = Wcfg.closeTimeVariateSpeed;
                probabilityPreferences = Wcfg.closeProbabilityPreferences;
                Preferences = Wcfg.ClosePreferences;
                timeToFindTargets = Wcfg.closeTimeFindTargets;
                minTimeChangeObjective = Wcfg.closeMinTimeChange;
                maxTimeChangeObjective = Wcfg.closeMaxTimeChange;
                probChangeObjective = Wcfg.closeProbChange;
                percentHpToCover = Wcfg.percentHpCoverClose;
                OnDieEvent = Wcfg.closeOnDieCallback;

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
                minTimeChangeObjective = Wcfg.distanceMinTimeChange;
                maxTimeChangeObjective = Wcfg.distanceMaxTimeChange;
                probChangeObjective = Wcfg.distanceProbChange;
                percentHpToCover = Wcfg.percentHpCoverDistance;
                OnDieEvent = Wcfg.distanceOnDieCallback;
                break;
            case WRESTLER_TYPE.Commander:
                health = Wcfg.commanderHealth;
                attackSpeed = Wcfg.commanderAttackSpeed;
                visionDistance = Wcfg.commanderVisionDistance;
                fieldView = Wcfg.commanderFieldView;
                attackDamage = Wcfg.commanderAttackDamage;
                minSpeed = Wcfg.commanderMinSpeed;
                maxSpeed = Wcfg.commanderMaxSpeed;
                probabilityChangeSpeed = Wcfg.commanderProbabilityVariateSpeed;
                timeToChangeSpeed = Wcfg.commanderTimeVariateSpeed;
                probabilityPreferences = Wcfg.commanderProbabilityPreferences;
                Preferences = Wcfg.CommanderPreferences;
                timeToFindTargets = Wcfg.commanderTimeFindTargets;
                minTimeChangeObjective = Wcfg.commanderMinTimeChange;
                maxTimeChangeObjective = Wcfg.commanderMaxTimeChange;
                probChangeObjective = Wcfg.commanderProbChange;
                percentHpToCover = Wcfg.percentHpCoverCommander;
                OnDieEvent = Wcfg.commanderOnDieCallback;
                break;
            case WRESTLER_TYPE.Giant:
                health = Wcfg.giantHealth;
                attackSpeed = Wcfg.giantAttackSpeed;
                visionDistance = Wcfg.giantVisionDistance;
                fieldView = Wcfg.giantFieldView;
                attackDamage = Wcfg.giantAttackDamage;
                minSpeed = Wcfg.giantMinSpeed;
                maxSpeed = Wcfg.giantMaxSpeed;
                probabilityChangeSpeed = Wcfg.giantProbabilityVariateSpeed;
                timeToChangeSpeed = Wcfg.giantTimeVariateSpeed;
                probabilityPreferences = Wcfg.giantProbabilityPreferences;
                Preferences = Wcfg.CommanderPreferences;
                timeToFindTargets = Wcfg.giantTimeFindTargets;
                minTimeChangeObjective = Wcfg.giantMinTimeChange;
                maxTimeChangeObjective = Wcfg.giantMaxTimeChange;
                probChangeObjective = Wcfg.giantProbChange;
                percentHpToCover = Wcfg.percentHpCoverGiant;
                OnDieEvent = Wcfg.giantOnDieCallback;
                break;
        }

        SetConfiguration();
    }

    public virtual void SetConfiguration()
    {
        GetComponent<FieldOfView>().viewRadius = visionDistance;
        GetComponent<FieldOfView>().viewAngle = fieldView;
        GetComponent<FieldOfView>().StartVision(timeToFindTargets);

        speed = Random.Range(minSpeed, maxSpeed);
        _agent.speed = speed;
        this.initialHealth = health;
        //_agent.stoppingDistance = attackRange;
    }

    public void OnTriggerEnter(Collider other)
    {
        if ((other.GetComponent<BoogieWrestler>() != null && other.GetComponent<BoogieWrestler>().team != this.team) || other.GetComponent<PlayerHealth>() != null && other.GetComponent<PlayerHealth>().team != this.team)
        {
            //boogie wrestler from the other team.
            if (other.GetComponent<BoogieWrestler>() == targetSelected || other.GetComponent<PlayerHealth>() == targetSelected)
            {
                closeEnoughToAttack = true;
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if ((other.GetComponent<BoogieWrestler>() != null && other.GetComponent<BoogieWrestler>().team != this.team) || other.GetComponent<PlayerHealth>() != null && other.GetComponent<PlayerHealth>().team != this.team)
        {
            //boogie wrestler from the other team.
            if (other.GetComponent<BoogieWrestler>() == targetSelected || other.GetComponent<PlayerHealth>() == targetSelected)
            {
                closeEnoughToAttack = false;
            }
        }
    }

    public virtual void WrestlerClicked(int clickButton)
    { 
        BoogiesSpawner.CommanderSelected = commander;
        UIController.OnWrestlerClicked -= WrestlerClicked;
        if (clickButton == 0 && (currentState == STATE.OnSquadCovering || currentState == STATE.OnSquadMoving || currentState == STATE.OnSquadObserving || currentState == STATE.OnSquadAttacking))
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
                UISquadSelectorController.Create(this.team, this.commander);
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
            commander.UncoverBody,
            commander.ResetDefaultFormation,
            commander.BackToPlayer
            );
        } 
        else if (clickButton == 0 && (currentState == STATE.AloneAttacking || currentState == STATE.AloneFollowingPlayer || currentState == STATE.AloneObserving || currentState == STATE.AloneMoving))
        {
            UIIndividualWrestlerOptionsController.Create(() =>
            {
                UIController.I.selectingSquadToJoin = true;
                UIController.OnSelectingSquadJoin += SquadSelected;
            },
            FollowPlayer,
            () =>
            {
                UIController.I.UIShowMouseSelector(SELECTION_TYPE.SquadMovingPosition);
                UIController.OnMovePositionSelected += MoveToPosition;
            },
            ()=>
            {
                this.BackToPlayer();
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
        this.currentState = STATE.AloneMoving;
        this._agent.stoppingDistance = 0f;
        if (this.targetSelected != null)
        {
            if (targetSelected.GetComponent<BoogieWrestler>().wrestlersAttackingMe.Contains(this))
            {
                targetSelected.GetComponent<BoogieWrestler>().wrestlersAttackingMe.Remove(this);
            }
            targetSelected = null;
        }
    }

    private void SquadSelected (BoogieWrestlerCommander bwc)
    {
        if (bwc.team == this.team)
        {
            JoinSquad(bwc);
        }
    }

    public virtual bool JoinSquad(BoogieWrestlerCommander bwc)
    {
        SquadConfiguration.Index randIndexs = bwc.neededIndexs[Random.Range(0, bwc.neededIndexs.Count)];
        SquadConfiguration.Index newIndexs = new SquadConfiguration.Index(randIndexs.i, randIndexs.j);
      
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
        this.currentState = commander.currentState;

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
        if (targetSelected != null)
        {
            if (targetSelected.GetComponent<BoogieWrestler>().wrestlersAttackingMe.Contains(this))
            {
                wrestlersAttackingMe.Remove(this);
            }
            targetSelected = null;
        }
        currentState = STATE.BackToPlayer;
        backToPlayer = true;
        _agent.stoppingDistance = 0f;
        _agent.SetDestination(FindObjectOfType<BoogiesSpawner>().transform.position);
    }

    public void FollowPlayer()
    {
        if (currentState == STATE.AloneFollowingPlayer) //already following
        {
            return;
        }
        
        if (commander != null)
        {
            commander.currentSquadList[listPosition] = SquadConfiguration.SQUAD_ROL.None;
        }
        
        //listPosition = -1;
        currentState = STATE.AloneFollowingPlayer;
        _agent.stoppingDistance = 0;
        this.transform.SetParent(null);
        if (commander != null)
        {
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

            if (targetSelected != null)
            {
                if (targetSelected.GetComponent<BoogieWrestler>().wrestlersAttackingMe.Contains(this))
                {
                    targetSelected.GetComponent<BoogieWrestler>().wrestlersAttackingMe.Remove(this);
                }
                targetSelected = null;
            }
        }
        
        StartCoroutine(MoveAroundPlayer());
    }

    public void BreakFormation()
    {
        if (leader == this.gameObject)
        {
            return;
        }
        Debug.Log("listPosition = " + listPosition);
        Debug.Log("trying to break formation of " + wrestlerType.ToString());
        commander.currentSquadList[listPosition] = SquadConfiguration.SQUAD_ROL.None;
        if (this.targetSelected != null)
        {
            currentState = STATE.AloneAttacking;
        }
        else
        {
            currentState = STATE.AloneObserving;
        }

        _agent.stoppingDistance = 0;
        this.transform.SetParent(null);
        commander.neededIndexs.Add(new SquadConfiguration.Index(this.indexs.i, this.indexs.j));
        commander.RemoveWrestlerSquad(this);
        leader = null;
        commander = null;
    }

    public void ChangePosition()
    {
        UIController.I.selectingWrestlerToChange = true;
        UIController.I.UIShowMouseSelector(SELECTION_TYPE.SelectingSquadWrestlerChange);
        UIController.OnSelectingWrestlerChange += ChangeSelectedWrestler;
    }

    public void ChangeSelectedWrestler(BoogieWrestler otherWrestler)
    {
        if (this.commander != otherWrestler && !this.commander.squadWrestlers.Contains(otherWrestler))//if it is not from our squad.
        {
            return;
        }
        if (otherWrestler == this)//if it is not from our squad.
        {
            return;
        }
        SquadConfiguration.SQUAD_ROL myRol = this.commander.currentSquadList[this.listPosition];
        SquadConfiguration.SQUAD_ROL hisRol = otherWrestler.commander.currentSquadList[otherWrestler.listPosition];
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
            if (otherWrestler.gameObject == commander.leader)
            {
                commander.leader = this.gameObject;
            }
            else if (this.gameObject == commander.leader)
            {
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
            SquadConfiguration.Index oldIndexs = 
                new SquadConfiguration.Index(this.indexs.i, this.indexs.j);
            
            foreach (BoogieWrestler bw in commander.squadWrestlers)
            {
                if (bw == commander)
                {
                    bw.leader = this.commander.gameObject;
                    continue;
                }
                if (commander.coveringBody.GetComponent<BoogieWrestler>() != null && 
                    commander.coveringBody.GetComponent<BoogieWrestler>() == bw)
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
        if (this.transform.parent != null && this.transform.parent.gameObject.GetComponentInChildren<BoogieWrestlerCommander>())
        {
            initialIndexs = new SquadConfiguration.Index(indexs.i, indexs.j);

            //Debug.Log(this.wrestlerType.ToString() + " -> commander assigned");
            commander = this.transform.parent.gameObject.GetComponentInChildren<BoogieWrestlerCommander>();
            currentState = STATE.OnSquadObserving;
            leader = commander.gameObject;
            ChangeIndexsRelativeToLeader();
            TakeSquadPosition();
        }
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

    public void TakeSquadPosition() //take position in the squad.
    {
        if (leader != null && (currentState == STATE.OnSquadCovering || currentState == STATE.OnSquadObserving))
        {
            if (targetSelected == null)
            {
                this.transform.rotation = leader.transform.rotation;
            }
            Vector3 offset = new Vector3(indexs.j * commander.wrestlersOffset, this.transform.position.y, indexs.i * commander.wrestlersOffset);
            randomPoint = leader.transform.position + this.transform.TransformDirection(offset);
        }

       

        /*
        if (currentState == STATE.OnSquadCovering && commander.coveringBody.GetComponent<BoogiesSpawner>())
        {
            _agent.transform.position = randomPoint;
        }
        */
        _agent.SetDestination(randomPoint);
    }

    public void MoveHere(Vector3 where)
    {
        Vector3 offset = new Vector3(indexs.j * commander.wrestlersOffset, this.transform.position.y, indexs.i * commander.wrestlersOffset);
        randomPoint = where + this.transform.TransformDirection(offset);
        if (_agent.isOnNavMesh)
        {
            _agent.SetDestination(randomPoint);
        }
        
    }

    public IEnumerator MoveAroundPlayer()
    {
        while (currentState == STATE.AloneFollowingPlayer)
        {
            Vector3 playerPos = FindObjectOfType<BoogiesSpawner>().gameObject.transform.position;
            Vector3 randomPos = new Vector3(Random.Range(playerPos.x - 5f, playerPos.x + 5f), playerPos.y, Random.Range(playerPos.z - 5f, playerPos.z + 5f));
            randomPoint = randomPos;
            _agent.SetDestination(randomPoint);
            yield return new WaitForSeconds(Random.Range(1.5f, 3f));
        }
    }

    public void StopCoroutines()
    {
        StopCoroutine(FindAnEnemy());
        StopCoroutine(ChooseEnemyObjective());
        StopCoroutine(AttackEnemy());
        StopCoroutine(GoToEnemy());
        StopCoroutine(CheckBattleEnds());
    }

    public virtual void Update()
    {
        HandlePriority();

        this.visibleTargets.RemoveAll((x) => x == null || x.GetComponent<BoogieWrestler>() == null);
        this.wrestlersAttackingMe.RemoveAll((x) => x == null);

        if (currentState == STATE.BackToPlayer)
        {
            if (targetSelected != null)
            {
                if (targetSelected.GetComponent<BoogieWrestler>().wrestlersAttackingMe.Contains(this))
                {
                    targetSelected.GetComponent<BoogieWrestler>().wrestlersAttackingMe.Remove(this);
                }
                targetSelected = null;
            }
            _agent.SetDestination(FindObjectOfType<BoogiesSpawner>().transform.position);
            if (Vector3.Distance(this.transform.position, FindObjectOfType<BoogiesSpawner>().transform.position) <= 2f)
            {
                Destroy(this.gameObject);
            }
            return;
        }

        if (helpingBoogie != null)
        {
            if (helpingBoogie.wrestlersAttackingMe.Count == 0)
            {
                helpingBoogie = null;
            }
        }
        
        if ((currentState == STATE.OnSquadCovering || currentState == STATE.OnSquadObserving || currentState == STATE.OnSquadRunningAway) //COVERING WILL ENTER HERE
        && this.leader != this.gameObject)
        {
            if (currentState == STATE.OnSquadCovering && commander.coveringBody.GetComponent<BoogiesSpawner>())
            {
                _agent.speed = commander.coveringBody.GetComponent<NavMeshAgent>().speed;
                _agent.acceleration = 50;
            }
            else
            {
                _agent.speed = speed;
                _agent.acceleration = 8;
            }
            TakeSquadPosition();
        }
            

        if (currentState == STATE.AloneMoving/*|| currentState == STATE.OnSquadRunningAway*/)
        {
            if (Vector3.Distance(this.transform.position, randomPoint) <= 0.5f)
            {
                if (FindObjectOfType<xMarkerBehavior>())
                {
                    Destroy(FindObjectOfType<xMarkerBehavior>().gameObject);
                }
                currentState = STATE.AloneObserving;
            }
        }

        if (currentState == STATE.OnSquadObserving)
        {
            if (commander.visibleTargets.Count > 0)
            {
                if (this.GetComponentInParent<SquadTeam>().enemyWrestlers.Count == 0)
                {
                    for (int i = 0; i<commander.visibleTargets.Count; i++)
                    {
                        if (!this.GetComponentInParent<SquadTeam>().enemyWrestlers.Contains(commander.visibleTargets[i].GetComponent<BoogieWrestler>()))
                        {
                            this.GetComponentInParent<SquadTeam>().enemyWrestlers.Add(commander.visibleTargets[i].GetComponent<BoogieWrestler>());
                        }
                        if (commander.visibleTargets[i].GetComponent<BoogieWrestler>().commander != null)
                        {
                            this.GetComponentInParent<SquadTeam>().enemyWrestlers = new List<BoogieWrestler>(commander.visibleTargets[i].GetComponent<BoogieWrestler>().commander.squadWrestlers);
                            break;
                        }
                    }

                    if (commander.otherCommander == null)
                    {
                        BoogieWrestlerCommander otherBwc = (BoogieWrestlerCommander)this.GetComponentInParent<SquadTeam>().enemyWrestlers.Find((x) => x.wrestlerType == WRESTLER_TYPE.Commander);
                        if (otherBwc != null)
                        {
                            commander.otherCommander = otherBwc;
                        }
                    }
                    
                }

                Debug.Log("HOLA");
                currentState = STATE.OnSquadAttacking;
                StartCoroutine(FindAnEnemy());
                StartCoroutine(ChooseEnemyObjective());
                StartCoroutine(CheckBattleEnds());
            }
        }

        if (currentState == STATE.OnSquadAttacking || currentState == STATE.AloneAttacking)
        {
            if (targetSelected != null)
            {
                Vector3 direction = targetSelected.transform.position - this.transform.position;
                direction.y = 0;
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            }
        }

        if (currentState == STATE.OnSquadAttacking || currentState == STATE.OnSquadObserving || currentState == STATE.AloneAttacking){
            if (targetSelected == null) //si no tenemos a nadie y nos pega alguien
            {
                if (wrestlersAttackingMe.Count > 0)
                {
                    BoogieWrestler bw = wrestlersAttackingMe[Random.Range(0, wrestlersAttackingMe.Count)];
                    EnemySelected(bw);
                    if (currentState == STATE.OnSquadObserving || currentState == STATE.AloneObserving)
                    {
                        StartCoroutine(ChooseEnemyObjective());
                        StartCoroutine(CheckBattleEnds());
                    }

                    currentState = STATE.OnSquadAttacking;
                }
            }
        }

        if ((currentState == STATE.OnSquadAttacking || currentState == STATE.OnSquadObserving) && health <= initialHealth * percentHpToCover)
        {
            if (commander != null)
            {
                if (!commander.needHelpBoogies.Contains(this))
                {
                    commander.INeedHelp(this);
                }
            }
            
            //currentState = STATE.OnSquadCovering;
        }
        
        if (currentState == STATE.AloneAttacking)
        {
            if (targetSelected == null)
            {
                currentState = STATE.AloneObserving;
            }
        }

        if (currentState == STATE.AloneObserving)
        {
            //this.transform.Rotate(Vector3.up, 20f * Time.deltaTime);
            if (this.visibleTargets.Count > 0)
            {
                currentState = STATE.AloneAttacking;
                if (coroutinesGoToEnemyEnabled == 0)
                {
                    StartCoroutine(GoToEnemy());
                }
                if (coroutinesAttackEnabled == 0)
                {
                    StartCoroutine(AttackEnemy());
                }
                EnemySelected(visibleTargets[Random.Range(0, visibleTargets.Count)].GetComponent<BoogieWrestler>());
            }
        }

        //TODO: falta controlar el caso en que estemos cubriendo y alguien nos ataque. Hay que atacar pero sin abandonar la posición

        //TODO: this is for debuging
        if (GetComponentInParent<SquadTeam>() != null)
        {
            this.team = GetComponentInParent<SquadTeam>().team;
        }
    }

    private void HandlePriority()
    {
       
        if (currentState == STATE.OnSquadAttacking || currentState == STATE.AloneAttacking)
        {
            if (this.wrestlerType == WRESTLER_TYPE.Commander)
            {
                _agent.avoidancePriority = 0;
            }
            else
            {
                _agent.avoidancePriority = 0;
            }
        }
        else
        {
            if (this.wrestlerType == WRESTLER_TYPE.Commander)
            {
                _agent.avoidancePriority = 10;
            }
            else
            {
                _agent.avoidancePriority = 50;
            }
        }
    
    }

    public IEnumerator GoToEnemy()
    {
        coroutinesGoToEnemyEnabled++;
        while (targetSelected != null)
        {
            yield return new WaitForSeconds(0.3f);
            if (currentState == STATE.OnSquadCovering)
            {
                continue;
            }
            if (targetSelected != null)
            {
                if (_agent.isOnNavMesh)
                {
                    _agent.SetDestination(targetSelected.transform.position);
                }
            }
        }
        coroutinesGoToEnemyEnabled--;
    }

    IEnumerator FindAnEnemy()
    {
        if (this.targetSelected != null && targetSelected.GetComponent<PlayerHealth>())
        {
            yield break;
        }

        if (this.helpingBoogie != null)
        {
            yield break;
        }
        if (targetSelected != null)
        {
            if (targetSelected.GetComponent<BoogieWrestler>() && targetSelected.GetComponent<BoogieWrestler>().wrestlersAttackingMe.Contains(this))
            {
                targetSelected.GetComponent<BoogieWrestler>().wrestlersAttackingMe.Remove(this);
            }
            targetSelected = null;
        }

        //Debug.Log(wrestlerType.ToString() + " from team " + team.ToString() + " is trying to find an enemy");
        if (this.visibleTargets.Count > 0)
        {
            TryFindingInOurList(0);
        }
        else
        {
            if (commander == null)
            {
                TryFindingInRemainList(0);
            }
            else
            {
                TryFindingInCommonList(0);
            }
        }
    }

    IEnumerator ChooseEnemyObjective()
    {
        while (!backToPlayer)
        {
            if ((currentState == STATE.OnSquadAttacking || currentState == STATE.AloneAttacking))
            {
                if (commander != null)
                {
                    foreach (BoogieWrestler bw in commander.wrestlersAttackingMe) //todo aquel que ataque al commander debe tener a alguien atacandole a el.
                    {
                        if (bw.wrestlersAttackingMe.Count == 0 && helpingBoogie == null)
                        {
                            if (targetSelected != null)
                            {
                                if (targetSelected.GetComponent<BoogieWrestler>() && targetSelected.GetComponent<BoogieWrestler>().wrestlersAttackingMe.Contains(this))
                                {
                                    targetSelected.GetComponent<BoogieWrestler>().wrestlersAttackingMe.Remove(this);
                                }
                                targetSelected = null;
                            }
                            EnemySelected(bw);
                            continue;
                        }
                    }
                }
                
                if (targetSelected != null) //si tenemos objetivo esperamos.
                {
                    float time = Random.Range(minTimeChangeObjective, maxTimeChangeObjective);
                    yield return new WaitForSeconds(time);
                }

                if (targetSelected != null && Random.value > probChangeObjective) //si tenemos objetivo y la probabilidad lo permite cambiamos de objetivo.
                {
                    yield return null;
                }
                else if (helpingBoogie == null)
                {
                    StartCoroutine(FindAnEnemy());
                }
            }
            yield return null;
        }
    }

    void TryFindingInRemainList(int i)
    {
        if (this.GetComponentInParent<SquadTeam>() != null && this.GetComponentInParent<SquadTeam>().enemyWrestlers.Count > 0)
        {
            if (i >= Preferences.Length)
            {
                return;
            }
            BoogieWrestler boogieTry = this.GetComponentInParent<SquadTeam>().enemyWrestlers.Find((x) => x != null 
            && x.GetComponent<BoogieWrestler>() != null && x.GetComponent<BoogieWrestler>().wrestlerType == Preferences[i]);
            if (boogieTry != null)
            {
                Transform aTry = boogieTry.transform;
                if (targetSelected != null)
                {
                    if (targetSelected.GetComponent<BoogieWrestler>().wrestlersAttackingMe.Contains(this))
                    {
                        targetSelected.GetComponent<BoogieWrestler>().wrestlersAttackingMe.Remove(this);
                    }
                    targetSelected = null;
                }
                EnemySelected(aTry.GetComponent<BoogieWrestler>());
                return;
            }
            else
            {
                TryFindingInRemainList(i + 1);
            }
        }
    }

    void TryFindingInOurList(int i)
    {
        if (this.visibleTargets.Count == 0 && commander != null) TryFindingInCommonList(0);
        if (Random.value <= probabilityPreferences || commander == null)
        {
            if (i >= Preferences.Length)
            {
                if (this.GetComponentInParent<SquadTeam>() != null)
                    TryFindingInRemainList(0);
                return;
            }
            Transform aTry = this.visibleTargets.Find((x) => x != null && x.GetComponent<BoogieWrestler>() != null 
            && x.GetComponent<BoogieWrestler>().wrestlerType == Preferences[i]);
            if (aTry != null)
            {
                if (targetSelected == null)
                {
                    EnemySelected(aTry.GetComponent<BoogieWrestler>());
                    return;
                }
            }
            else TryFindingInOurList(i + 1);
        }
        else TryFindingInCommonList(0);
    }

    void TryFindingInCommonList(int i)
    {
        //Debug.Log(wrestlerType.ToString() + " is finding in common list. ");
        if (Random.value > probabilityPreferences || this.visibleTargets.Count == 0)
        {
            //find in common
            if (i >= Preferences.Length)
            {
                if (commander != null) //TODO: esto no se si esta bien
                {
                    TryFindingInRemainList(0);
                }
                return;
            }
           
            Transform aTry = this.commander.visibleTargets.Find((x) => x != null && x.GetComponent<BoogieWrestler>() != null && x.GetComponent<BoogieWrestler>().wrestlerType == Preferences[i]);
            if (aTry != null)
            {
                if (targetSelected == null)
                {
                    
                    //if (helpingBoogie != null) //TODO: esto no se si esta bien
                    //{
                        EnemySelected(aTry.GetComponent<BoogieWrestler>());
                    //}
                    return;
                }
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

    public void EnemySelected(BoogieWrestler bw)
    {
        FindObjectOfType<BoogiesSpawner>().wrestlersWrestling = true;
        if (this.GetComponentInParent<SquadTeam>() != null && !this.GetComponentInParent<SquadTeam>().enemyWrestlers.Contains(bw))
        {
            this.GetComponentInParent<SquadTeam>().enemyWrestlers.Add(bw);
        }

        if (helpingBoogie != null)
        {
            return;
        }
        targetSelected = bw; 
        if (this.wrestlerType == WRESTLER_TYPE.Distance)
        {
            _agent.stoppingDistance = attackRange;
        }
        if (!bw.wrestlersAttackingMe.Contains(bw))
        {
            bw.wrestlersAttackingMe.Add(this);
        }
        if (coroutinesAttackEnabled == 0)
        {
            StartCoroutine(AttackEnemy());
        }
        if (coroutinesGoToEnemyEnabled == 0)
        {
            StartCoroutine(GoToEnemy());
        }
        
    }

    public IEnumerator AttackEnemy()
    {
        coroutinesAttackEnabled++;
        while (targetSelected != null)
        {
            if (targetSelected != null)
            {
                if (this.wrestlerType == WRESTLER_TYPE.Distance)
                {
                    yield return new WaitForSeconds(attackSpeed);
                    if (targetSelected != null)
                    {
                        if (CloseEnough(targetSelected.transform.position))
                        {
                            Attack();
                        }
                    }     
                }
                else
                {
                    if (targetSelected != null)
                    {
                        yield return new WaitForSeconds(attackSpeed);
                        if (closeEnoughToAttack)
                        {
                            Attack();
                        }
                    }
                }
            }
            yield return null;
        }
        coroutinesAttackEnabled--;
    }

    void Attack()
    {
        if (targetSelected != null)
        {
            GetComponent<AnimationController>().Attack();
            targetSelected.GetDamage(attackDamage);
        }
    }

    public override void Die()
    {
        foreach (BoogieWrestler bw in wrestlersAttackingMe)
        {
            bw.visibleTargets.Remove(this.transform);

            if (bw.targetSelected != null)
            {
                if (bw.wrestlersAttackingMe.Contains(this))
                {
                    bw.wrestlersAttackingMe.Remove(this);
                }
                bw.targetSelected = null;
            }
        }

        //si era el commander y el commander era el leader darle leader a otro random (o el que mas vida tenga)
        //a partir de este momento no se pueden mover, solo pueden cubrirse entre ellos pero no moverse.
        if (this.commander != this && commander != null) //si no era commander
        {
            commander.currentSquadList[listPosition] = SquadConfiguration.SQUAD_ROL.None;
            commander.squadWrestlers.Remove(this);
            switch (wrestlerType){
                case WRESTLER_TYPE.Close:
                    commander.closeWrestlers.RemoveAt(0);
                    break;
                case WRESTLER_TYPE.Distance:
                    commander.distanceWrestlers.RemoveAt(0);
                    break;
                case WRESTLER_TYPE.Giant:
                    commander.giantWrestlers.RemoveAt(0);
                    break;
            }
        }
        else if (commander != null)
        {
            foreach (BoogieWrestler bw in commander.squadWrestlers)
            {
                if (bw.currentState == STATE.OnSquadAttacking)
                {
                    bw.currentState = STATE.AloneAttacking;
                }
            }
        }

        if (currentState == STATE.OnSquadCovering)
        {
            if (this.leader == this.gameObject) //eramos a quien estaban cubriendo
            {
                if (commander != null)
                {
                    Debug.Log("all attack again");
                    foreach (BoogieWrestler bw in commander.squadWrestlers)
                    {
                        bw.currentState = STATE.OnSquadAttacking;
                    }
                    commander.AssignAsLeader();
                }

            }
            
        }

        if (commander == this && currentState == STATE.OnSquadMoving)
        {
            foreach (BoogieWrestler bw in commander.squadWrestlers)
            {
                bw.currentState = STATE.OnSquadAttacking;
            }
        }

        if (helpingBoogie == this)
        {
            foreach (BoogieWrestler bw in commander.squadWrestlers)
            {
                helpingBoogie = null;
            }
        }
        if (this.commander != null && this.commander.needHelpBoogies.Contains(this))
        {
            this.commander.needHelpBoogies.Remove(this);
        }
        Debug.Log("i die with " + health + " health points. ");
        OnDieEvent.Invoke();

        _anim.SetInteger("closeEnoughToAttack", -2);
        Destroy(this.gameObject, 6f);
        Destroy(GetComponent<AnimationController>());
        Destroy(GetComponent<Collider>());
        Destroy(GetComponent<FieldOfView>());
        Destroy(GetComponent<NavMeshAgent>());
        Destroy(this);
    }

    IEnumerator CheckBattleEnds()
    {
        while (true)
        {
            if (this.GetComponentInParent<SquadTeam>() != null && this.GetComponentInParent<SquadTeam>().enemyWrestlers.Count == 0)
            {
                if (commander != null)
                {
                    foreach (BoogieWrestler bw in commander.squadWrestlers)
                    {
                        if (bw.targetSelected != null)
                        {
                            yield break;
                        }
                        bw.currentState = STATE.OnSquadObserving;
                        _agent.stoppingDistance = 0f;
                    }
                }
                else
                {
                    this.currentState = STATE.AloneObserving;
                    _agent.stoppingDistance = 0f;
                }
                break;
            }
            yield return null;
        }
    }

    public bool CloseEnough(Vector3 position)
    {
        if (targetSelected == null)
        {
            return false;
        }
        else
        {
            float distance = Vector3.Distance(this.transform.position, position);
            return distance <= attackRange;
        }
    }

    public override void Save()
    {
        if (this == null || this.gameObject == null)
        {
            return;
        }

        if (commander != null)
        {
            key = listPosition.ToString() + this.wrestlerType.ToString() + GetComponentInParent<SquadTeam>().uniqueId;
        }
        else
        {
            key = listPosition.ToString() + this.wrestlerType.ToString() + uniqueId;
        }

        //Debug.Log("my wrestler key is " + key);

        SaverManager.I.saveData[key + "position"] = this.transform.position;
        SaverManager.I.saveData[key + "rotation"] = this.transform.rotation;
        SaverManager.I.saveData[key + "state"] = (int)currentState;
        SaverManager.I.saveData[key + "commander"] = this.commander;
        SaverManager.I.saveData[key + "leader"] = this.leader;
        SaverManager.I.saveData[key + "parent"] = this.transform.parent;
        SaverManager.I.saveData[key + "indexs"] = this.indexs.i.ToString() + "," + this.indexs.j.ToString();
    }

    public override void Load()
    {
        try
        {
            _agent.enabled = false;
            this.transform.position = SaverManager.I.saveData[key + "position"];
            this.transform.rotation = SaverManager.I.saveData[key + "rotation"];
            _agent.enabled = true;
            this.currentState = (STATE)SaverManager.I.saveData[key + "state"];
            this.commander = (BoogieWrestlerCommander)SaverManager.I.saveData[key + "commander"];
            this.leader = SaverManager.I.saveData[key + "leader"];
            this.transform.SetParent(SaverManager.I.saveData[key + "parent"], true);
            string indexs = SaverManager.I.saveData[key + "indexs"];
            string index1 = ""; string index2 = "";
            bool commaEncounter = false;
            for (int i = 0; i < indexs.Length; i++)
            {
                if (indexs[i] == ',')
                {
                    commaEncounter = true;
                    continue;
                }
                if (!commaEncounter)
                {
                    index1 += indexs[i];
                }
                else
                {
                    index2 += indexs[i];
                }
            }
            this.indexs = new SquadConfiguration.Index(int.Parse(index1), int.Parse(index2));
        }
        catch
        {
            if (this != null && gameObject != null)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
