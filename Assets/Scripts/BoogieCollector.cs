using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class BoogieCollector : Boogie
{
    public float timeToCollect;
    public float timeToCollectAgain;
    public float timeToFollowMarkerAgain;
    public float minTimeChangeDirection;
    public float maxTimeChangeDirection;
    public float probabilityChangeDirection;
    public float timeToReleaseMarker;
    public bool canCollect = true;
    public bool canFollowMarker = true;
    public bool followingMarker = false;
    public ElixirObstacleStone currentElixirStone;
    public GameObject marker;
    private GameObject lastMarker;
    private GameObject currentMarker;
    private GameObject lastMarkerSpawned;
    private bool coroutineChangeDirectionStarted = false;
    [HideInInspector]public List<MarkerBehaviour> markers;
    public CURRENT_STATE currentState;

    public enum CURRENT_STATE
    {
        GoingToCollectorMachine,
        WanderingAroundObjective,
        WanderingAroundCircle,
        CollectingElixir,
        FollowingMarkersToElixirStone,
        FollowingMarkersToCollectorMachine,
        GoingToPlayer
    }

    private enum DIRECTION
    {
        ElixirStone,
        CollectorMachine
    }

    public override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        AssignConfiguration();

        type = BoogieType.Collector;
        randomPoint = GetRandomPointAroundCircle(initialPoint);
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
        timeToCollectAgain = Ccfg.timeToCollectAgain;
        timeToFollowMarkerAgain = Ccfg.timeToFollowMarkerAgain;
        minTimeChangeDirection = Ccfg.minTimeChangeDirection;
        maxTimeToFindObjective = Ccfg.maxTimeChangeDirection;
        probabilityChangeDirection = Ccfg.probabilityChangeDirection;

        minSpeed = Ccfg.minSpeed;
        maxSpeed = Ccfg.maxSpeed;
        probabilityChangeSpeed = Ccfg.probabilityVariateSpeed;
        timeToChangeSpeed = Ccfg.timeTryVariateSpeed;
    }

    private void Update()
    {
        if (currentState == CURRENT_STATE.WanderingAroundCircle && RandomPointDestinationReached()) //we don't have an objective yet.
        {
            randomPoint = GetRandomPointAroundCircle(initialPoint);
            _agent.SetDestination(randomPoint);
            if (!objectiveNotFoundTimerEnabled)
            {
                StartCoroutine(ObjectiveNotFound());
                objectiveNotFoundTimerEnabled = true;
            }
        }

        if (currentState == CURRENT_STATE.WanderingAroundObjective && RandomPointDestinationReached()) //we have an objective but we are not carrying elixir.
        {
            randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
            _agent.SetDestination(randomPoint);
        }

        if (BoogiesSpawner.BoogiesKnowCollectorMachinePosition && currentState == CURRENT_STATE.GoingToCollectorMachine && CollectorMachineDestinationReached())
        {
            DepositElixir();
        }

        if (!BoogiesSpawner.BoogiesKnowCollectorMachinePosition && currentState == CURRENT_STATE.GoingToCollectorMachine && RandomPointDestinationReached())
        {
            randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
            _agent.SetDestination(randomPoint);
            if (!objectiveNotFoundTimerEnabled)
            {
                StartCoroutine(ObjectiveNotFound());
                objectiveNotFoundTimerEnabled = true;
            }
        }

        if (backToPlayer && PlayerDestinationReached())
        {
            Destroy(this.gameObject);
            BoogiesSpawner.CollectorsAmount++;
        }
    }

    private void DepositElixir()
    {
        canCollect = false;
        canFollowMarker = false;
        CollectorMachineBehavior.ElixirGot++;
        currentElixirStone = null;
        StartCoroutine(HandleCanCollectTrue());
        StartCoroutine(HandleCanFollowMarkerTrue());
        if (markers.Count > 0)
        {
            foreach (MarkerBehaviour mb in markers)
            {
                if (mb != null)
                {
                    mb.GetComponent<MeshRenderer>().material.color = Color.green;
                    mb.valid = true;
                }                
            }
            markers.Clear();
        }

        
        randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
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

    private bool Wandering()
    {
        return currentState == CURRENT_STATE.WanderingAroundObjective || currentState == CURRENT_STATE.WanderingAroundCircle;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((Wandering() || currentState == CURRENT_STATE.FollowingMarkersToElixirStone) && other.GetComponent<ElixirObstacleStone>() && StoneIsAvailable(other.GetComponent<ElixirObstacleStone>()) && canCollect)
            //if we get a non-full stone
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

            //canFollowMarker = false;
            currentElixirStone = other.GetComponent<ElixirObstacleStone>();
            StartCoroutine(OnCollectingElixir());
        }

        if ((Wandering() || currentState == CURRENT_STATE.FollowingMarkersToElixirStone) && other.GetComponent<ElixirObstacleStone>() && !StoneIsAvailable(other.GetComponent<ElixirObstacleStone>()) && canCollect)
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
            _agent.SetDestination(randomPoint);
            currentState = CURRENT_STATE.WanderingAroundObjective;
        }

        if (other.GetComponent<MarkerBehaviour>() && Wandering() && currentElixirStone == null && canFollowMarker)
        {
            currentMarker = other.gameObject;
            StartCoroutine(OnFollowingMarkers(DIRECTION.ElixirStone));
        }

        if (other.GetComponent<MarkerBehaviour>() && canFollowMarker && !BoogiesSpawner.BoogiesKnowCollectorMachinePosition && other.GetComponent<MarkerBehaviour>().valid)
        {
            if (currentState == CURRENT_STATE.GoingToCollectorMachine && other.GetComponent<MarkerBehaviour>().markerCreator != this) //carrying elixir
            {
                currentMarker = other.gameObject;
                StartCoroutine(OnFollowingMarkers(DIRECTION.CollectorMachine));
            }
        }

        if (other.GetComponent<CollectorMachineBehavior>() && !BoogiesSpawner.BoogiesKnowCollectorMachinePosition && (currentState == CURRENT_STATE.GoingToCollectorMachine || currentState == CURRENT_STATE.FollowingMarkersToCollectorMachine))
        {
            DepositElixir();
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
            while (currentMarker != null && currentState != CURRENT_STATE.CollectingElixir)
            {
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
            while (currentMarker != null)
            {
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
        while (true)
        {
            if (currentState != CURRENT_STATE.FollowingMarkersToElixirStone && currentState != CURRENT_STATE.FollowingMarkersToCollectorMachine)
            {
                break;
            }
            if (currentMarker != null && Vector3.Distance(transform.position, currentMarker.transform.position) <= 0.1f)
            {
                break;
            }
            yield return null;
        }
    }

    IEnumerator HandleChangeDirectionRandomly()
    {
        while (true)
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
                    randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
                    _agent.SetDestination(randomPoint);
                }
                else
                {
                    randomPoint = GetRandomPointAroundCircle(initialPoint);
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
            currentElixirStone.elixirAvailable--;
            if (currentElixirStone.elixirAvailable == 0)
            {
                currentElixirStone.empty = true;
            }
            yield return new WaitForSeconds(timeToCollect);
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
            currentElixirStone.bCollectorsIn--;
            BringElixirToCollectorMachine();
            //change color
        }
        else
        {
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
            _agent.SetDestination(randomPoint);
            _agent.isStopped = false;
        }
        StartCoroutine(MarkRoad());
    }

    IEnumerator MarkRoad()
    {
        lastMarker = Instantiate(marker, transform.position, Quaternion.identity);
        lastMarker.GetComponent<MarkerBehaviour>().previousMarker = null;
        lastMarker.GetComponent<MarkerBehaviour>().markerCreator = this;
        markers.Add(lastMarker.GetComponent<MarkerBehaviour>());    
        while (currentElixirStone != null)
        {
            yield return new WaitForSeconds(timeToReleaseMarker);
            GameObject currentMarker = Instantiate(marker, transform.position, Quaternion.identity);
            MarkerBehaviour mb = currentMarker.GetComponent<MarkerBehaviour>();
            mb.markerCreator = this;
            mb.previousMarker = lastMarker;
            mb.previousMarker.GetComponent<MarkerBehaviour>().nextMarker = currentMarker;
            mb.valid = false;
            markers.Add(mb);
            lastMarker = currentMarker;
        }
        lastMarker = null;
    }

    public override void BackToPlayer()
    {
        backToPlayer = true;
        randomPoint = Vector3.positiveInfinity;
        _agent.SetDestination(FindObjectOfType<BoogiesSpawner>().gameObject.transform.position);
        currentState = CURRENT_STATE.GoingToPlayer;
    }

    IEnumerator CheckIfWorkFinished()
    {
        Debug.Log("checking. ");
        yield return new WaitForSeconds(timeToCheckIfWorkFinished);
        if (CollectorMachineBehavior.ElixirGot == currentObjective.GetComponent<ElixirObstacle>().totalElixirAvailable)
        {
            BackToPlayer();
        }
        StartCoroutine(CheckIfWorkFinished());
    }

    public override void OnObjectiveSelected()
    {
        StartCoroutine(CheckIfWorkFinished());
    }
}
