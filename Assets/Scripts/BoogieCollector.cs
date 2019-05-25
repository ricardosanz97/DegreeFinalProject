using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class BoogieCollector : Boogie
{
    public float timeToCollect;
    public float timeToDeposit;
    public float timeToCollectAgain;
    public float timeToFollowMarkerAgain;
    public float minTimeChangeDirection;
    public float maxTimeChangeDirection;
    public float probabilityChangeDirection;
    public float timeToReleaseMarker;
    public float markerLifePeriod;
    public bool canCollect = true;
    public bool canFollowMarker = true;
    public bool followingMarker = false;
    public ElixirObstacleStone currentElixirStone;
    public GameObject marker;
    private GameObject lastMarker;
    public GameObject currentMarker;
    private GameObject lastMarkerSpawned;
    public ElixirObstacleStone.TYPE collectingType = ElixirObstacleStone.TYPE.None;
    //private bool coroutineChangeDirectionStarted = false;
    [HideInInspector]public List<MarkerBehaviour> markers;
    GameObject carriedPart;
    public CURRENT_STATE currentState;

    private BoogieCollectorAnimationController _bcac;

    public enum CURRENT_STATE
    {
        GoingToCollectorMachine,
        WanderingAroundObjective,
        WanderingAroundCircle,
        CollectingElixir,
        FollowingMarkersToElixirStone,
        FollowingMarkersToCollectorMachine,
        DepositingElixir,
        GoingToPlayer
    }

    private enum DIRECTION
    {
        ElixirStone,
        CollectorMachine
    }

    public override void Save()
    {
        base.Save();
        string key = "BoogieCollector" + uniqueId;
        SaverManager.I.saveData.Add(key + "Position", this.transform.position);
        SaverManager.I.saveData.Add(key + "Rotation", this.transform.rotation);
        SaverManager.I.saveData.Add(key + "AnimState", _anim.GetInteger("state"));
        SaverManager.I.saveData.Add(key + "CanCollect", canCollect);
        SaverManager.I.saveData.Add(key + "CanFollowMarker", canFollowMarker);
        SaverManager.I.saveData.Add(key + "FollowingMarker", followingMarker);
        SaverManager.I.saveData.Add(key + "CurrentMarker", currentMarker);
        SaverManager.I.saveData.Add(key + "LastMarkerSpawned", lastMarkerSpawned);
        SaverManager.I.saveData.Add(key + "LastMarker", lastMarker);
        SaverManager.I.saveData.Add(key + "CollectingType", collectingType);
        SaverManager.I.saveData.Add(key + "Markers", markers);
        SaverManager.I.saveData.Add(key + "CurrentElixirStone", currentElixirStone);
        SaverManager.I.saveData.Add(key + "CurrentObjective", currentObjective);
        SaverManager.I.saveData.Add(key + "CurrentState", currentState);
        SaverManager.I.saveData.Add(key + "BackToPlayer", backToPlayer);
        SaverManager.I.saveData.Add(key + "PartCarried", carriedPart);
    }

    public override void Load()
    {
        base.Load();
        string key = "BoogieCollector" + uniqueId;
        try
        {
            int animState = SaverManager.I.saveData[key + "AnimState"];
            if (animState == -1)
            {
                _agent.enabled = false;
                this.transform.position = SaverManager.I.saveData[key + "Position"];
                this.transform.rotation = SaverManager.I.saveData[key + "Rotation"];
                _agent.enabled = true;
            }
            else
            {
                this.transform.position = SaverManager.I.saveData[key + "Position"];
                this.transform.rotation = SaverManager.I.saveData[key + "Rotation"];
            }
            _anim.SetInteger("state", animState);
            this.canCollect = SaverManager.I.saveData[key + "CanCollect"];
            this.canFollowMarker = SaverManager.I.saveData[key + "CanFollowMarker"];
            this.followingMarker = SaverManager.I.saveData[key + "FollowingMarker"];
            this.currentMarker = SaverManager.I.saveData[key + "CurrentMarker"];
            this.lastMarkerSpawned = SaverManager.I.saveData[key + "LastMarkerSpawned"];
            this.lastMarker = SaverManager.I.saveData[key + "LastMarker"];
            this.collectingType = SaverManager.I.saveData[key + "CollectingType"];
            this.markers = SaverManager.I.saveData[key + "Markers"];
            this.currentElixirStone = SaverManager.I.saveData[key + "CurrentElixirStone"];
            this.currentObjective = SaverManager.I.saveData[key + "CurrentObjective"];
            this.currentState = SaverManager.I.saveData[key + "CurrentState"];
            this.backToPlayer = SaverManager.I.saveData[key + "BackToPlayer"];
            this.carriedPart = SaverManager.I.saveData[key + "PartCarried"];

            if (this.carriedPart != null)
            {
                carriedPart.transform.SetParent(this.transform.Find("ChargingPoint"), false);
                carriedPart.transform.localPosition = Vector3.zero;
            }
        }
        catch
        {
            Destroy(this.gameObject);
        }
    }

    public override void Awake()
    {
        base.Awake();
        _bcac = GetComponent<BoogieCollectorAnimationController>();
    }

    private void Start()
    {
        AssignConfiguration();

        type = BoogieType.Collector;
        randomPoint = GetRandomPointAroundCircle(initialPoint);
        UnityEngine.AI.NavMeshPath nmp = new UnityEngine.AI.NavMeshPath();
        while (!_agent.CalculatePath(randomPoint, nmp))
        {
            randomPoint = GetRandomPointAroundCircle(initialPoint);
        }
        _agent.SetDestination(randomPoint);
        currentState = CURRENT_STATE.WanderingAroundCircle;
        StartCoroutine(HandleChangeDirectionRandomly());

        StartCoroutine(HandleSpeed());
    }

    private void AssignConfiguration()
    {
        CollectorsConfiguration Ccfg = FindObjectOfType<BoogiesSpawner>().collectorsConfig;
        maxTimeToFindObjective = Ccfg.maxTimeToFindObjective;
        timeToCheckIfWorkFinished = Ccfg.timeToCheckIfWorkWinished;
        timeToCollect = Ccfg.timeToCollect;
        timeToDeposit = Ccfg.timeToDeposit;
        timeToCollectAgain = Ccfg.timeToCollectAgain;
        timeToFollowMarkerAgain = Ccfg.timeToFollowMarkerAgain;
        minTimeChangeDirection = Ccfg.minTimeChangeDirection;
        maxTimeChangeDirection = Ccfg.maxTimeChangeDirection;
        probabilityChangeDirection = Ccfg.probabilityChangeDirection;
        timeToReleaseMarker = Ccfg.timeToReleaseMarker;

        minSpeed = Ccfg.minSpeed;
        maxSpeed = Ccfg.maxSpeed;
        _agent.speed = Random.Range(minSpeed, maxSpeed);
        probabilityChangeSpeed = Ccfg.probabilityVariateSpeed;
        timeToChangeSpeed = Ccfg.timeTryVariateSpeed;
        
        markerLifePeriod = Ccfg.markersLifePeriod;
    }

    private void Update()
    {
        if (!backToPlayer && currentState == CURRENT_STATE.WanderingAroundCircle && RandomPointDestinationReached()) //we don't have an objective yet.
        {
            randomPoint = GetRandomPointAroundCircle(initialPoint);
            UnityEngine.AI.NavMeshPath nmp = new UnityEngine.AI.NavMeshPath();
            while (!_agent.CalculatePath(randomPoint, nmp))
            {
                randomPoint = GetRandomPointAroundCircle(initialPoint);
            }
            _agent.SetDestination(randomPoint);
            if (!objectiveNotFoundTimerEnabled)
            {
                StartCoroutine(ObjectiveNotFound());
                objectiveNotFoundTimerEnabled = true;
            }
        }

        if (!backToPlayer && currentState == CURRENT_STATE.WanderingAroundObjective && RandomPointDestinationReached()) //we have an objective but we are not carrying elixir.
        {
            randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
            UnityEngine.AI.NavMeshPath nmp = new UnityEngine.AI.NavMeshPath();
            while (!_agent.CalculatePath(randomPoint, nmp))
            {
                randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
            }
            _agent.SetDestination(randomPoint);
        }

        if (!backToPlayer && BoogiesSpawner.BoogiesKnowCollectorMachinePosition && currentState == CURRENT_STATE.GoingToCollectorMachine && CollectorMachineDestinationReached())
        {
            if (FindObjectOfType<CollectorMachineBehavior>().type == this.collectingType)
            {
                Vector3 direction = FindObjectOfType<CollectorMachineBehavior>().transform.position - this.transform.position;
                direction.y = 0f;
                this.transform.rotation = Quaternion.LookRotation(direction);
                StartCoroutine(DepositElixir());
            }
            else
            {
                if (currentObjective == null)
                {
                    randomPoint = GetRandomPointAroundCircle(initialPoint);
                    UnityEngine.AI.NavMeshPath nmp = new UnityEngine.AI.NavMeshPath();
                    while (!_agent.CalculatePath(randomPoint, nmp))
                    {
                        randomPoint = GetRandomPointAroundCircle(initialPoint);
                    }
                    _agent.SetDestination(randomPoint);
                }
                else
                {
                    randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
                    UnityEngine.AI.NavMeshPath nmp = new UnityEngine.AI.NavMeshPath();
                    while (!_agent.CalculatePath(randomPoint, nmp))
                    {
                        randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
                    }
                    _agent.SetDestination(randomPoint);
                }
            }
            
        }

        if (!backToPlayer && !BoogiesSpawner.BoogiesKnowCollectorMachinePosition && currentState == CURRENT_STATE.GoingToCollectorMachine && RandomPointDestinationReached())
        {
            randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
            UnityEngine.AI.NavMeshPath nmp = new UnityEngine.AI.NavMeshPath();
            while (!_agent.CalculatePath(randomPoint, nmp))
            {
                randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
            }
            _agent.SetDestination(randomPoint);
            if (!objectiveNotFoundTimerEnabled)
            {
                StartCoroutine(ObjectiveNotFound());
                objectiveNotFoundTimerEnabled = true;
            }
        }

        if (backToPlayer && PlayerDestinationReached())
        {
            BoogiesSpawner.CollectorsAmount++;
            BoogiesSpawner.CollectorsSpawned--;
            FindObjectOfType<FastSelectorBoogiesController>().collectorsAmountSlider.onValueChanged.RemoveAllListeners();
            FindObjectOfType<FastSelectorBoogiesController>().collectorsAmountSlider.value++;
            FindObjectOfType<FastSelectorBoogiesController>().collectorsAmountText.text = BoogiesSpawner.CollectorsAmount.ToString();
            FindObjectOfType<FastSelectorBoogiesController>().collectorsAmountSlider.GetComponent<SliderController>().AddListener();
            Destroy(this.gameObject);
        }

        if (BoogiesSpawner.CollectorsBackButtonPressed || backToPlayer)
        {
            BackToPlayer();
        }
    }

    IEnumerator DepositElixir()
    {
        currentMarker = null;
        currentState = CURRENT_STATE.DepositingElixir;
        _agent.isStopped = true;
        Destroy(this.transform.Find("ChargingPoint").GetChild(0).gameObject);
        _anim.SetInteger("state", 3);
        canCollect = false;
        canFollowMarker = false;
        yield return new WaitForSeconds(timeToDeposit);
        if (!backToPlayer)
        {
            CollectorMachineBehavior.ElixirGot++;
        }
        
        currentElixirStone = null;
        StartCoroutine(HandleCanCollectTrue());
        StartCoroutine(HandleCanFollowMarkerTrue());
        if (markers.Count > 0)
        {
            foreach (MarkerBehaviour mb in markers)
            {
                if (mb != null)
                {
                    mb.GetComponentInChildren<SpriteRenderer>().color = Color.green;
                    mb.valid = true;
                }                
            }
            markers.Clear();
        }
        _anim.SetInteger("state", 0);
        _agent.isStopped = false;
        randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
        UnityEngine.AI.NavMeshPath nmp = new UnityEngine.AI.NavMeshPath();
        while (!_agent.CalculatePath(randomPoint, nmp))
        {
            randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
        }
        _agent.SetDestination(randomPoint);
        currentState = CURRENT_STATE.WanderingAroundObjective;
    }

    IEnumerator HandleCanCollectTrue()
    {
        yield return new WaitForSeconds(timeToCollectAgain);
        canCollect = true;
    }

    IEnumerator HandleCanFollowMarkerTrue()
    {
        yield return new WaitForSeconds(timeToFollowMarkerAgain);
        canFollowMarker = true;
    }

    private bool RandomPointDestinationReached()
    {
        return Vector3.Distance(this.transform.position, randomPoint) < 1f;
    }

    private bool PlayerDestinationReached()
    {
        return Vector3.Distance(this.transform.position, FindObjectOfType<BoogiesSpawner>().gameObject.transform.position) < 1f;
    }

    private bool CollectorMachineDestinationReached()
    {
        float distance = Vector3.Distance(this.transform.position, FindObjectOfType<CollectorMachineBehavior>().gameObject.transform.position);
        return distance < 1.5f;
    }

    public bool Wandering()
    {
        return currentState == CURRENT_STATE.WanderingAroundObjective || currentState == CURRENT_STATE.WanderingAroundCircle;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!backToPlayer && Wandering() && currentObjective == null && other.GetComponent<BoogieCollector>() && other.GetComponent<BoogieCollector>().currentObjective != null)
        {
            this.SetObjective(other.GetComponent<BoogieCollector>().currentObjective);
            if (currentState == CURRENT_STATE.WanderingAroundCircle)
            {
                randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
                UnityEngine.AI.NavMeshPath nmp = new UnityEngine.AI.NavMeshPath();
                while (!_agent.CalculatePath(randomPoint, nmp))
                {
                    randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
                }
                _agent.SetDestination(randomPoint);
                currentState = CURRENT_STATE.WanderingAroundObjective;
            }
        }

        if (!backToPlayer && (Wandering() || currentState == CURRENT_STATE.FollowingMarkersToElixirStone) && other.GetComponent<ElixirObstacleStone>() 
            && StoneIsAvailable(other.GetComponent<ElixirObstacleStone>()) && canCollect)
            //if we get a non-full stone
        {
            if (other.GetComponent<ElixirObstacleStone>().type == this.collectingType || this.collectingType == ElixirObstacleStone.TYPE.None)
            {
                if (this.collectingType == ElixirObstacleStone.TYPE.None)
                {
                    this.collectingType = other.GetComponent<ElixirObstacleStone>().type;
                    if (this.collectingType == ElixirObstacleStone.TYPE.Elixir)
                    {
                        this.marker = Resources.Load("Prefabs/Obstacles/ElixirEnergyObstacle/ElixirMarker") as GameObject;
                    }
                    else if (this.collectingType == ElixirObstacleStone.TYPE.Energy)
                    {
                        this.marker = Resources.Load("Prefabs/Obstacles/ElixirEnergyObstacle/EnergyMarker") as GameObject;
                    }

                    this.marker.GetComponent<MarkerBehaviour>().lifePeriod = markerLifePeriod;
                }
                if (currentObjective == null)
                {
                    SetObjective(other.GetComponentInParent<Obstacle>());
                    if (!objectiveNotFoundTimerEnabled)
                    {
                        StartCoroutine(ObjectiveNotFound());
                        objectiveNotFoundTimerEnabled = true;
                    }
                }

                //canFollowMarker = false;
                currentElixirStone = other.GetComponent<ElixirObstacleStone>();
                Vector3 direction = currentElixirStone.transform.position - this.transform.position;
                direction.y = 0f;
                this.transform.rotation = Quaternion.LookRotation(direction);
                StartCoroutine(OnCollectingElixir());
            }
            
        }

        if (!backToPlayer && (Wandering() || currentState == CURRENT_STATE.FollowingMarkersToElixirStone) && other.GetComponent<ElixirObstacleStone>() && 
            (other.GetComponent<ElixirObstacleStone>().type == this.collectingType || this.collectingType == ElixirObstacleStone.TYPE.None) && !StoneIsAvailable(other.GetComponent<ElixirObstacleStone>()) && canCollect)
        {

            if (currentObjective == null)
            {
                SetObjective(other.GetComponentInParent<Obstacle>());
                if (!objectiveNotFoundTimerEnabled)
                {
                    StartCoroutine(ObjectiveNotFound());
                    objectiveNotFoundTimerEnabled = true;
                }
            }

            randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
            UnityEngine.AI.NavMeshPath nmp = new UnityEngine.AI.NavMeshPath();
            while (!_agent.CalculatePath(randomPoint, nmp))
            {
                randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
            }
            _agent.SetDestination(randomPoint);
            currentState = CURRENT_STATE.WanderingAroundObjective;
        }

        if (!backToPlayer && other.GetComponent<MarkerBehaviour>() && (other.GetComponent<MarkerBehaviour>().type == this.collectingType || 
            other.GetComponent<MarkerBehaviour>().type == ElixirObstacleStone.TYPE.None) && Wandering() && currentElixirStone == null 
            && canFollowMarker && currentState != CURRENT_STATE.DepositingElixir)
        {
            if (this.collectingType == ElixirObstacleStone.TYPE.None)
            {
                this.collectingType = other.GetComponent<MarkerBehaviour>().type;
            }
            currentMarker = other.gameObject;
            StartCoroutine(OnFollowingMarkers(DIRECTION.ElixirStone));
        }

        if (!backToPlayer && other.GetComponent<MarkerBehaviour>() && other.GetComponent<MarkerBehaviour>().type == this.collectingType 
            && canFollowMarker && !BoogiesSpawner.BoogiesKnowCollectorMachinePosition && other.GetComponent<MarkerBehaviour>().valid)
        {
            if (currentState == CURRENT_STATE.GoingToCollectorMachine && other.GetComponent<MarkerBehaviour>().markerCreator != this) //carrying elixir
            {
                currentMarker = other.gameObject;
                StartCoroutine(OnFollowingMarkers(DIRECTION.CollectorMachine));
            }
        }

        if (!backToPlayer && other.GetComponent<CollectorMachineBehavior>() != null && other.GetComponent<CollectorMachineBehavior>().type == this.collectingType 
            && !BoogiesSpawner.BoogiesKnowCollectorMachinePosition 
            && (currentState == CURRENT_STATE.GoingToCollectorMachine || currentState == CURRENT_STATE.FollowingMarkersToCollectorMachine))
        {
            Vector3 direction = other.transform.position - this.transform.position;
            direction.y = 0f;
            this.transform.rotation = Quaternion.LookRotation(direction);
            StartCoroutine(DepositElixir());
        }
    }

    private bool StoneIsAvailable(ElixirObstacleStone stone)
    {
        return !stone.empty && stone.bCollectorsIn < stone.maxCollectorsIn;
    }

    IEnumerator OnFollowingMarkers(DIRECTION dir)
    {
        followingMarker = true;
        canFollowMarker = false;
        

        if (dir == DIRECTION.ElixirStone)
        {
            while (currentMarker != null && currentState != CURRENT_STATE.CollectingElixir && currentState != CURRENT_STATE.DepositingElixir && !backToPlayer)
            {
                Debug.Log("following markers to elixir stone");
                _agent.SetDestination(currentMarker.transform.position);
                currentState = CURRENT_STATE.FollowingMarkersToElixirStone;
                yield return StartCoroutine(OnGoingToAMarker());
                if (currentMarker != null && currentMarker.GetComponent<MarkerBehaviour>().previousMarker != null)
                {
                    currentMarker = currentMarker.GetComponent<MarkerBehaviour>().previousMarker;
                }
                else
                {
                    break;
                }
                yield return null;
            }
            followingMarker = false;
        }
        else if (dir == DIRECTION.CollectorMachine)
        {
            while (currentMarker != null && currentState != CURRENT_STATE.DepositingElixir)
            {
                Debug.Log("following markers to collecting machine");
                _agent.SetDestination(currentMarker.transform.position);
                currentState = CURRENT_STATE.FollowingMarkersToCollectorMachine;
                yield return StartCoroutine(OnGoingToAMarker());
                if (currentMarker != null && currentMarker.GetComponent<MarkerBehaviour>().nextMarker != null)
                {
                    currentMarker = currentMarker.GetComponent<MarkerBehaviour>().nextMarker;
                }
                else
                {
                    break;
                }
                yield return null;
            }
            followingMarker = false;
        }   
    }

    IEnumerator OnGoingToAMarker()
    {
        _agent.isStopped = false;
        while (true && !backToPlayer)
        {
            if (currentState != CURRENT_STATE.FollowingMarkersToElixirStone && currentState != CURRENT_STATE.FollowingMarkersToCollectorMachine)
            {
                break;
            }
            if (currentMarker != null && Vector3.Distance(transform.position, currentMarker.transform.position) <= 01f)
            {
                break;
            }
            yield return null;
        }
    }

    IEnumerator HandleChangeDirectionRandomly()
    {
        while (true && !backToPlayer)
        {
            yield return new WaitForSeconds(Random.Range(minTimeChangeDirection, maxTimeChangeDirection));
            if (Random.value > probabilityChangeDirection)
            {
                yield return null;
            }
            if (Wandering() || 
                (currentState == CURRENT_STATE.FollowingMarkersToElixirStone && currentMarker == null) || 
                (currentState == CURRENT_STATE.FollowingMarkersToElixirStone && currentMarker != null && currentMarker.GetComponent<MarkerBehaviour>().previousMarker == null) ||
                (!BoogiesSpawner.BoogiesKnowCollectorMachinePosition && currentState == CURRENT_STATE.FollowingMarkersToCollectorMachine && currentMarker != null && currentMarker.GetComponent<MarkerBehaviour>().nextMarker == null) ||
                (!BoogiesSpawner.BoogiesKnowCollectorMachinePosition && currentState == CURRENT_STATE.FollowingMarkersToCollectorMachine && currentMarker == null) ||
                (!BoogiesSpawner.BoogiesKnowCollectorMachinePosition && currentState == CURRENT_STATE.GoingToCollectorMachine && currentMarker != null && currentMarker.GetComponent<MarkerBehaviour>().nextMarker != null))
            {
                if (currentObjective != null)
                {
                    if (currentState == CURRENT_STATE.FollowingMarkersToCollectorMachine)
                    {
                        currentState = CURRENT_STATE.GoingToCollectorMachine;
                        if (_anim.GetInteger("state") != 2)
                        {
                            _anim.SetInteger("state", 2);
                        }
                    }
                    else if (currentState == CURRENT_STATE.FollowingMarkersToElixirStone)
                    {
                        currentState = CURRENT_STATE.WanderingAroundObjective;
                        if (_anim.GetInteger("state") != 0)
                        {
                            _anim.SetInteger("state", 0);
                        }
                    }
                    else if (currentState == CURRENT_STATE.GoingToCollectorMachine)
                    {
                        if (_anim.GetInteger("state") != 2)
                        {
                            _anim.SetInteger("state", 2);
                        }
                    }
                    if (_agent.isStopped)
                    {
                        _agent.isStopped = false;
                    }
                    randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
                    UnityEngine.AI.NavMeshPath nmp = new UnityEngine.AI.NavMeshPath();
                    while (!_agent.CalculatePath(randomPoint, nmp))
                    {
                        randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
                    }
                    _agent.SetDestination(randomPoint);
                }
                else
                {
                    if (currentState == CURRENT_STATE.FollowingMarkersToElixirStone)
                    {
                        currentState = CURRENT_STATE.WanderingAroundCircle;
                        if (_anim.GetInteger("state") != 0)
                        {
                            _anim.SetInteger("state", 0);
                        }
                    }
                    randomPoint = GetRandomPointAroundCircle(initialPoint);
                    UnityEngine.AI.NavMeshPath nmp = new UnityEngine.AI.NavMeshPath();
                    while (!_agent.CalculatePath(randomPoint, nmp))
                    {
                        randomPoint = GetRandomPointAroundCircle(initialPoint);
                    }
                    _agent.SetDestination(randomPoint);
                }
            }
        }
    }

    IEnumerator OnCollectingElixir()
    {
        if (currentElixirStone.elixirAvailable > 0)
        {
            currentState = CURRENT_STATE.CollectingElixir;
            currentElixirStone.bCollectorsIn++;
            _agent.isStopped = true;
            
            if (currentElixirStone.elixirAvailable <= 0)
            {
                currentElixirStone.empty = true;
            }
            
            _anim.SetInteger("state", 1);
            yield return new WaitForSeconds(timeToCollect);

            carriedPart = null;
            if (!backToPlayer)
            {
                if (collectingType == ElixirObstacleStone.TYPE.Elixir && !backToPlayer)
                {
                    Debug.Log("elixir");
                    carriedPart = Instantiate(Resources.Load("Prefabs/Obstacles/ElixirEnergyObstacle/ElixirStonePart"), this.transform.position, Quaternion.identity) as GameObject;
                }
                else if (collectingType == ElixirObstacleStone.TYPE.Energy && !backToPlayer)
                {
                    Debug.Log("energy");
                    carriedPart = Instantiate(Resources.Load("Prefabs/Obstacles/ElixirEnergyObstacle/EnergyStonePart"), this.transform.position, Quaternion.identity) as GameObject;
                }

                carriedPart.transform.SetParent(this.transform.Find("ChargingPoint"), false);
                carriedPart.transform.localPosition = Vector3.zero;
                carriedPart.GetComponent<ElixirStonePart>().AssignKey("BoogieCollector" + uniqueId);
            }
            
            
            if (BoogiesSpawner.BoogiesKnowCollectorMachinePosition)
            {
                canFollowMarker = false;
                canCollect = false;
            }
            else
            {
                canFollowMarker = true;
                canCollect = false;
            }
            _anim.SetInteger("state", 2);
            if (!backToPlayer)
            {
                if (currentElixirStone != null && currentElixirStone.elixirAvailable > 0)
                {
                    currentElixirStone.elixirAvailable--;
                    currentElixirStone.bCollectorsIn--;
                }
                else
                {
                    CollectorMachineBehavior.ElixirGot--;
                }
                BringElixirToCollectorMachine();
                //change color
            }
        }
        else
        {
            if (currentState == CURRENT_STATE.FollowingMarkersToElixirStone)
            {
                if (currentObjective != null)
                {
                    currentState = CURRENT_STATE.WanderingAroundObjective;
                    randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
                    UnityEngine.AI.NavMeshPath nmp = new UnityEngine.AI.NavMeshPath();
                    while (!_agent.CalculatePath(randomPoint, nmp))
                    {
                        randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
                    }
                    _agent.SetDestination(randomPoint);
                }
                else
                {
                    currentState = CURRENT_STATE.WanderingAroundCircle;
                    randomPoint = GetRandomPointAroundCircle(initialPoint);
                    UnityEngine.AI.NavMeshPath nmp = new UnityEngine.AI.NavMeshPath();
                    while (!_agent.CalculatePath(randomPoint, nmp))
                    {
                        randomPoint = GetRandomPointAroundCircle(initialPoint);
                    }
                    _agent.SetDestination(randomPoint);
                }
            }
            currentElixirStone = null;
            yield return null;
        }

    }

    private void BringElixirToCollectorMachine()
    {
        currentState = CURRENT_STATE.GoingToCollectorMachine;
        if (BoogiesSpawner.BoogiesKnowCollectorMachinePosition)
        {
            _agent.SetDestination(FindObjectOfType<CollectorMachineBehavior>().transform.position);
            _agent.isStopped = false;
        }
        else
        {
            randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
            UnityEngine.AI.NavMeshPath nmp = new UnityEngine.AI.NavMeshPath();
            while (!_agent.CalculatePath(randomPoint, nmp))
            {
                randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
            }
            _agent.SetDestination(randomPoint);
            _agent.isStopped = false;
        }
        StartCoroutine(MarkRoad());
    }

    IEnumerator MarkRoad()
    {
        lastMarker = Instantiate(marker, new Vector3(transform.position.x, 0.11f, transform.position.z), Quaternion.Euler(new Vector3(0,Random.Range(0,360), 0)));
        lastMarker.GetComponent<MarkerBehaviour>().previousMarker = null;
        lastMarker.GetComponent<MarkerBehaviour>().markerCreator = this;
        markers.Add(lastMarker.GetComponent<MarkerBehaviour>());    
        while (currentElixirStone != null && !backToPlayer)
        {
            yield return new WaitForSeconds(timeToReleaseMarker);
            if (currentState == CURRENT_STATE.GoingToCollectorMachine || currentState == CURRENT_STATE.FollowingMarkersToCollectorMachine)
            {
                GameObject currentMarker = Instantiate(marker, new Vector3(transform.position.x, 0.11f, transform.position.z), Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)));
                MarkerBehaviour mb = currentMarker.GetComponent<MarkerBehaviour>();
                mb.markerCreator = this;
                mb.previousMarker = lastMarker;
                mb.previousMarker.GetComponent<MarkerBehaviour>().nextMarker = currentMarker;
                mb.valid = false;
                markers.Add(mb);
                lastMarker = currentMarker;
            }
            
        }
        lastMarker = null;
    }

    public override void BackToPlayer()
    {

        backToPlayer = true;
        canCollect = false;
        canFollowMarker = false;
        randomPoint = FindObjectOfType<BoogiesSpawner>().gameObject.transform.position;
        if (_agent.isOnNavMesh)
        {
            _agent.isStopped = false;
            _agent.SetDestination(randomPoint);
        }

        if (currentElixirStone != null)
        {
            if (currentState == CURRENT_STATE.FollowingMarkersToCollectorMachine || currentState == CURRENT_STATE.GoingToCollectorMachine || Wandering())
            {
                currentElixirStone.elixirAvailable++;
            }
            else if (currentState == CURRENT_STATE.DepositingElixir)
            {
                currentElixirStone.elixirAvailable++;
                StopCoroutine(DepositElixir());
            }
            else if (currentState == CURRENT_STATE.CollectingElixir)
            {
                currentElixirStone.bCollectorsIn--;
                StopCoroutine(OnCollectingElixir());
            }

            if (this.transform.Find("ChargingPoint").childCount > 0)
            {
                Debug.Log("HIJOS = " + this.transform.Find("ChargingPoint").childCount);
                Destroy(this.transform.Find("ChargingPoint").GetChild(0).gameObject);
            }

            currentElixirStone = null;
        }

        _anim.SetInteger("state", 0);
        currentState = CURRENT_STATE.GoingToPlayer;
    }

    IEnumerator CheckIfWorkFinished()
    {
        yield return new WaitForSeconds(timeToCheckIfWorkFinished);
        if (CollectorMachineBehavior.ElixirGot == currentObjective.GetComponent<ElixirObstacle>().totalElixirAvailable)
        {
            currentObjective.completed = true;
            Debug.Log("work finished");
            BackToPlayer();
        }
        StartCoroutine(CheckIfWorkFinished());
    }

    public override void OnObjectiveSelected()
    {
        StartCoroutine(CheckIfWorkFinished());
    }

    public override void Die()
    {
        throw new System.NotImplementedException();
    }
}
