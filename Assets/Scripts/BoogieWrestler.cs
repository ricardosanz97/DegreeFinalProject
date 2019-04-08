using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;
using DG.Tweening;

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
    OnSquadMoving,
    OnSquadRunningAway,
    AloneObserving,
    AloneAttacking,
    AloneFollowingPlayer,
    AloneMoving
}

public class BoogieWrestler : Boogie
{
    public WRESTLER_TYPE wrestlerType;
    [HideInInspector]public SquadConfiguration.Index initialIndexs;
    [HideInInspector]public SquadConfiguration.Index indexs;
    [HideInInspector]public SquadConfiguration.Index lastPos;
    public BoogieWrestlerCommander commander;
    [HideInInspector]public int listPosition;
    public GameObject leader;
    [HideInInspector]public bool isLeaderPosition;

    public TEAM team;
    public STATE currentState;
    public float health;
    [HideInInspector] public float attackSpeed;
    [HideInInspector] public float visionDistance;
    [HideInInspector] public float fieldView;
    [HideInInspector] public float attackRange;
    [HideInInspector] public float attackDamage;

    [HideInInspector] public float minOffset;
    [HideInInspector] public float maxOffset;
    [HideInInspector] public float probabilityVariateOffset;
    [HideInInspector] public float timeVariateOffset;

    [HideInInspector] public float timeToFindTargets;

    [HideInInspector] public float minTimeChangeObjective;
    [HideInInspector] public float maxTimeChangeObjective;
    [HideInInspector] public float probChangeObjective;

    [HideInInspector] public float probabilityPreferences;
    public WRESTLER_TYPE[] Preferences;

    public List<Transform> visibleTargets = new List<Transform>();
    public BoogieWrestler wrestlerSelected = null;
    public List<BoogieWrestler> wrestlersAttackingMe = new List<BoogieWrestler>();

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
                minTimeChangeObjective = Wcfg.closeMinTimeChange;
                maxTimeChangeObjective = Wcfg.closeMaxTimeChange;
                probChangeObjective = Wcfg.closeProbChange;

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
                minTimeChangeObjective = Wcfg.commanderMinTimeChange;
                maxTimeChangeObjective = Wcfg.commanderMaxTimeChange;
                probChangeObjective = Wcfg.commanderProbChange;
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
                minTimeChangeObjective = Wcfg.giantMinTimeChange;
                maxTimeChangeObjective = Wcfg.giantMaxTimeChange;
                probChangeObjective = Wcfg.giantProbChange;
                break;
        }

        SetConfiguration();
    }

    public void SetConfiguration()
    {
        GetComponent<FieldOfView>().viewRadius = visionDistance;
        GetComponent<FieldOfView>().viewAngle = fieldView;
        GetComponent<FieldOfView>().StartVision(timeToFindTargets);

        //_agent.stoppingDistance = attackRange;
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
            commander.UncoverBody,
            commander.ResetDefaultFormation
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
        this.currentState = STATE.AloneMoving;
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
    }

    public void FollowPlayer()
    {
        if (currentState == STATE.AloneFollowingPlayer) //already following
        {
            return;
        }
        commander.currentSquadList[listPosition] = SquadConfiguration.SQUAD_ROL.None;
        //listPosition = -1;
        currentState = STATE.AloneFollowingPlayer;
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
        currentState = STATE.AloneObserving;
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
            Debug.Log("aqui entra");
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

        currentState = STATE.OnSquadObserving;
    
        leader = commander.gameObject;
        ChangeIndexsRelativeToLeader();
        initialIndexs = new SquadConfiguration.Index(indexs.i, indexs.j);
        TakeSquadPosition();

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
        if (currentState == STATE.OnSquadMoving)
        {
            return;
        }
        if (leader != null && (currentState == STATE.OnSquadCovering || currentState == STATE.OnSquadObserving))
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
        while (currentState == STATE.AloneFollowingPlayer)
        {
            Vector3 playerPos = FindObjectOfType<BoogiesSpawner>().gameObject.transform.position;
            Vector3 randomPos = new Vector3(Random.Range(playerPos.x - 5f, playerPos.x + 5f), playerPos.y, Random.Range(playerPos.z - 5f, playerPos.z + 5f));
            randomPoint = randomPos;
            _agent.SetDestination(randomPoint);
            yield return new WaitForSeconds(Random.Range(1.5f, 3f));
        }
    }

    public virtual void Update()
    {
        if ((currentState == STATE.OnSquadCovering || currentState == STATE.OnSquadMoving || currentState == STATE.OnSquadObserving || currentState == STATE.OnSquadRunningAway) 
        && this.leader != this.gameObject) 
            TakeSquadPosition();

        if (currentState == STATE.AloneMoving/*|| currentState == STATE.OnSquadRunningAway*/)
        {
            Debug.Log("checking");
            if (Vector3.Distance(this.transform.position, randomPoint) <= 0.5f)
            {
                currentState = STATE.AloneObserving;
            }
        }

        //TODO: habrá que hacer algo para cuando estemos huyendo.

        if (currentState == STATE.OnSquadObserving || currentState == STATE.OnSquadMoving)
        {
            if (commander.visibleTargets.Count > 0)
            {
                if (this.GetComponentInParent<SquadTeam>().enemyWrestlers.Count == 0)
                {
                    this.GetComponentInParent<SquadTeam>().enemyWrestlers = new List<BoogieWrestler>(commander.visibleTargets[0].GetComponent<BoogieWrestler>().commander.squadWrestlers);
                }
                currentState = STATE.OnSquadAttacking;
                StartCoroutine(FindAnEnemy());
                StartCoroutine(ChooseEnemyObjective());
                StartCoroutine(AttackEnemy());
                StartCoroutine(GoToEnemy());
                StartCoroutine(CheckBattleEnds());
            }
        }

        if (currentState == STATE.AloneAttacking)
        {
            if (commander == null)
            {
                if (this.GetComponentInParent<SquadTeam>() != null && this.GetComponentInParent<SquadTeam>().enemyWrestlers.Count > 0)//still formation
                {
                }
                else//break formation
                {
                    //TODO: proceos de mirar alrededor y buscar a por quien ir.
                }
            }
        }

        if (currentState == STATE.OnSquadAttacking)
        {
            if (wrestlerSelected != null)
            {
                Vector3 direction = wrestlerSelected.transform.position - this.transform.position;
                direction.y = 0;
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
                //Debug.Log("Distance: " + Vector3.Distance(this.transform.position, wrestlerSelected.transform.position));
            }
        }

        if (currentState == STATE.OnSquadMoving || currentState == STATE.OnSquadAttacking || currentState == STATE.OnSquadObserving || currentState == STATE.AloneAttacking){
            if (wrestlerSelected == null) //si no tenemos a nadie y nos pega alguien
            {
                if (wrestlersAttackingMe.Count > 0)
                {
                    if (currentState == STATE.OnSquadMoving || currentState == STATE.OnSquadObserving)
                    {
                        currentState = STATE.OnSquadAttacking;
                    }

                    Debug.Log("someone hit us, now we have an objective. ");

                    BoogieWrestler bw =  wrestlersAttackingMe[Random.Range(0, wrestlersAttackingMe.Count)];
                    EnemySelected(bw);
                }
            }
        }

        //TODO: falta controlar el caso en que estemos cubriendo y alguien nos ataque. Hay que atacar pero sin abandonar la posición

        //TODO: this is for debuging
        if (GetComponentInParent<SquadTeam>() != null)
        {
            this.team = GetComponentInParent<SquadTeam>().team;
        }
    }

    IEnumerator GoToEnemy()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.3f);
            if (wrestlerSelected != null)
            {
                if (_agent.isOnNavMesh)
                {
                    _agent.SetDestination(wrestlerSelected.transform.position);
                }
            }
        }
    }

    IEnumerator FindAnEnemy()
    {
        if (wrestlerSelected != null)
        {
            wrestlerSelected.wrestlersAttackingMe.Remove(this);
            wrestlerSelected = null;
        }

        yield return new WaitForSeconds(Random.Range(0.1f, 1f));
        if (this.visibleTargets.Count > 0)
        {
            TryFindingInOurList(0);
        }
        else
        {
            if (commander == null && wrestlerSelected == null)
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
        while (true)
        {
            if (currentState == STATE.OnSquadAttacking || currentState == STATE.AloneAttacking)
            {
                foreach (BoogieWrestler bw in commander.wrestlersAttackingMe)
                {
                    if (bw.wrestlersAttackingMe.Count == 0)
                    {
                        if (wrestlerSelected != null)
                        {
                            wrestlerSelected.wrestlersAttackingMe.Remove(this);
                            wrestlerSelected = null;
                        }
                        EnemySelected(bw);
                    }
                }

                if (wrestlerSelected != null)
                {
                    float time = Random.Range(minTimeChangeObjective, maxTimeChangeObjective);
                    yield return new WaitForSeconds(time);
                }

                if (Random.value > probChangeObjective)
                {
                    yield return null;
                }
                else
                {
                    StartCoroutine(FindAnEnemy());
                }
            }
            yield return null;
        }
    }

    void TryFindingInRemainList(int i)
    {
        if (this.GetComponentInParent<SquadTeam>().enemyWrestlers.Count > 0)
        {
            if (i >= Preferences.Length)
            {
                return;
            }
            Debug.Log(this.wrestlerType.ToString() + " is trying to find a " + Preferences[i].ToString() + " in the remain list. ");
            BoogieWrestler boogieTry = this.GetComponentInParent<SquadTeam>().enemyWrestlers.Find((x) => x != null && x.GetComponent<BoogieWrestler>() != null && x.GetComponent<BoogieWrestler>().wrestlerType == Preferences[i]);
            if (boogieTry != null)
            {
                Transform aTry = boogieTry.transform;
                if (wrestlerSelected == null)
                {
                    Debug.Log(this.wrestlerType.ToString() + " selects an enemy " + aTry.GetComponent<BoogieWrestler>().wrestlerType.ToString() + " from the remain list. ");
                    EnemySelected(aTry.GetComponent<BoogieWrestler>());
                    return;
                }
            }
            else
            {
                TryFindingInRemainList(i + 1);
            }
        }
    }

    void TryFindingInOurList(int i)
    {
        if (this.visibleTargets.Count == 0)
        {
            TryFindingInCommonList(0);
        }
        if (Random.value <= probabilityPreferences)
        {
            if (i >= Preferences.Length)
            {
                return;
            }
            Transform aTry = this.commander.visibleTargets.Find((x) => x != null && x.GetComponent<BoogieWrestler>() != null && x.GetComponent<BoogieWrestler>().wrestlerType == Preferences[i]);
            if (aTry != null)
            {
                if (wrestlerSelected == null)
                {
                    EnemySelected(aTry.GetComponent<BoogieWrestler>());
                    return;
                }
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
        if (Random.value > probabilityPreferences || this.visibleTargets.Count == 0)
        {
            //find in common
            if (i >= Preferences.Length)
            {
                return;
            }
            Transform aTry = this.commander.visibleTargets.Find((x) => x != null && x.GetComponent<BoogieWrestler>() != null && x.GetComponent<BoogieWrestler>().wrestlerType == Preferences[i]);
            if (aTry != null)
            {
                if (wrestlerSelected == null)
                {
                    EnemySelected(aTry.GetComponent<BoogieWrestler>());
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

    void EnemySelected(BoogieWrestler bw)
    {
        wrestlerSelected = bw;
        //_agent.stoppingDistance = attackRange;
        if (this.wrestlerType == WRESTLER_TYPE.Distance)
        {
            _agent.stoppingDistance = attackRange;
        }
        bw.wrestlersAttackingMe.Add(this);
    }

    IEnumerator AttackEnemy()
    {
        while (true)
        {
            if (wrestlerSelected != null)
            {
                float distance = Vector3.Distance(this.transform.position, wrestlerSelected.transform.position);
                if (distance <= attackRange)
                {
                    yield return new WaitForSeconds(attackSpeed);
                    Attack();
                }
            }
            yield return null;
        }
    }

    void Attack()
    {
        if (wrestlerSelected != null)
        {
            wrestlerSelected.GetDamage(attackDamage);
            //StartCoroutine(wrestlerSelected.GetDamage((wrestlerSelected.transform.position - this.transform.position).normalized, attackDamage));
        }
    }

    public void GetDamage(float amount)
    {
        this.health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        foreach (BoogieWrestler bw in wrestlersAttackingMe)
        {
            bw.wrestlerSelected = null;
            bw.visibleTargets.Remove(this.transform);
            if (bw.wrestlersAttackingMe.Contains(this))
            {
                bw.wrestlersAttackingMe.Remove(this);
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
        else
        {
            foreach (BoogieWrestler bw in commander.squadWrestlers)
            {
                if (bw.currentState == STATE.OnSquadAttacking)
                {
                    bw.currentState = STATE.AloneAttacking;
                }
            }
        }
        Destroy(this.gameObject);
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
                        bw.currentState = STATE.OnSquadObserving;
                        _agent.stoppingDistance = 0f;
                    }
                }
                else
                {
                    _agent.stoppingDistance = 0f;
                }
                break;
            }
            yield return null;
        }
    }

}
