using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class BoogieCollector : Boogie
{
    public float timeToCollect = 5f;
    public bool canCollect = true;
    public bool canFollowMarker = true;
    public bool followingMarker = false;
    public ElixirObstacleStone currentElixirStone;
    public GameObject marker;
    private GameObject lastMarker;
    private GameObject currentMarker;
    private bool coroutineChangeDirectionStarted = false;
    public enum CURRENT_PATH
    {
        RandomCircle,
        RandomObjective,
        Marker,
        CollectorMachine,
        Player,
        None
    }

    public enum CURRENT_STATE
    {
        GoingToCollectorMachine,
        WanderingAroundObjective,
        WanderingAroundCircle,
        CollectingElixir,
        FollowingMarkers,
        GoingToPlayer
    }

    public CURRENT_PATH currentPath;
    public CURRENT_STATE currentState;

    private void Start()
    {
        type = BoogieType.Collector;
        randomPoint = GetRandomPointAroundCircle(initialPoint);
        bool a = _agent.SetDestination(randomPoint);
        currentPath = CURRENT_PATH.RandomCircle;
        currentState = CURRENT_STATE.WanderingAroundCircle;
        if (a == false)
        {
            Debug.LogError("Destination cant be reached. ");
        }

        StartCoroutine(HandleChangeDirectionRandomly());
    }

    private void Update()
    {
        if (currentObjective == null && RandomPointDestinationReached())
        {
            randomPoint = GetRandomPointAroundCircle(initialPoint);
            currentPath = CURRENT_PATH.RandomCircle;
            currentState = CURRENT_STATE.WanderingAroundCircle;
            bool a = _agent.SetDestination(randomPoint);
            if (a == false)
            {
                Debug.LogError("Destination cant be reached. ");
            }
            if (!objectiveNotFoundTimerEnabled)
            {
                StartCoroutine(ObjectiveNotFound());
                objectiveNotFoundTimerEnabled = true;
            }
        }

        if (currentObjective != null && currentElixirStone == null && RandomPointDestinationReached())
        {
            randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
            currentPath = CURRENT_PATH.RandomObjective;
            currentState = CURRENT_STATE.WanderingAroundObjective;
            bool a = _agent.SetDestination(randomPoint);
            if (a == false)
            {
                Debug.LogError("Destination cant be reached. ");
            }
        }

        if (BoogiesSpawner.BoogiesKnowCollectorMachinePosition && currentElixirStone != null && /*PlayerDestinationReached()*/ CollectorMachineDestinationReached())
        {
            DepositElixir();
        }
    }

    private void DepositElixir()
    {
        CollectorMachineBehavior.ElixirGot++;
        currentElixirStone = null;
        StartCoroutine(HandleCanCollectTrue());
        StartCoroutine(HandleCanFollowMarkerTrue());
        randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
        bool a = _agent.SetDestination(randomPoint);
        currentPath = CURRENT_PATH.RandomObjective;
        currentState = CURRENT_STATE.WanderingAroundObjective;
        if (a == false)
        {
            Debug.LogError("Destination cant be reached. ");
        }
    }

    IEnumerator HandleCanCollectTrue()
    {
        yield return new WaitForSeconds(2f);
        canCollect = true;
    }

    IEnumerator HandleCanFollowMarkerTrue()
    {
        yield return new WaitForSeconds(2f);
        canFollowMarker = true;
    }

    private bool RandomPointDestinationReached()
    {
        return Vector3.Distance(this.transform.position, randomPoint) < 1f;
    }

    private bool PlayerDestinationReached()
    {
        return Vector3.Distance(this.transform.position, FindObjectOfType<BoogiesSpawner>().gameObject.transform.position) < 1.5f;
    }

    private bool CollectorMachineDestinationReached()
    {
        float distance = Vector3.Distance(this.transform.position, FindObjectOfType<CollectorMachineBehavior>().gameObject.transform.position);
        return distance < 1.5f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ElixirObstacleStone>() && StoneIsAvailable(other.GetComponent<ElixirObstacleStone>()) && canCollect)
            //if we get a non-full stone
        {
            currentPath = CURRENT_PATH.None;
            currentState = CURRENT_STATE.CollectingElixir;
            Debug.Log("tocamos una piedra y vamos a extraer elixir. ");
            canFollowMarker = false;
            if (currentObjective == null)
            {
                SetObjective(other.GetComponentInParent<Obstacle>());
            }
            currentElixirStone = other.GetComponent<ElixirObstacleStone>();
            currentElixirStone.bCollectorsIn++;
            StartCoroutine(CollectElixir());
        }

        if (other.GetComponent<ElixirObstacleStone>() && !StoneIsAvailable(other.GetComponent<ElixirObstacleStone>()) && canCollect && currentObjective != null)
        {
            randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
            bool a = _agent.SetDestination(randomPoint);
            currentPath = CURRENT_PATH.RandomObjective;
            currentState = CURRENT_STATE.WanderingAroundObjective;
            if (a == false)
            {
                Debug.LogError("Destination cant be reached. ");
            }
        }

        if (other.GetComponent<MarkerBehaviour>() && currentElixirStone == null && canFollowMarker)
        {
            Debug.Log("we can follow marker");
            currentMarker = other.gameObject;
            StartCoroutine(OnFollowingMarkers());
        }

        if (other.GetComponent<CollectorMachineBehavior>() && currentElixirStone != null)
        {
            DepositElixir();
        }
    }

    private bool StoneIsAvailable(ElixirObstacleStone stone)
    {
        return !stone.empty && stone.bCollectorsIn < 3;
    }

    IEnumerator OnFollowingMarkers()
    {
        followingMarker = true;
        canFollowMarker = false;
        while (currentMarker != null && currentState != CURRENT_STATE.CollectingElixir)
        {
            bool a = _agent.SetDestination(currentMarker.transform.position);
            currentPath = CURRENT_PATH.Marker;
            currentState = CURRENT_STATE.FollowingMarkers;
            if (a == false)
            {
                Debug.LogError("Destination cant be reached. ");
            }
            yield return StartCoroutine(OnGoingToAMarker());
            currentMarker = currentMarker.GetComponent<MarkerBehaviour>().previousMarker;
            yield return null;
        }
        followingMarker = false;
    }

    IEnumerator HandleChangeDirectionRandomly()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(2f, 4f));
            if (currentState == CURRENT_STATE.WanderingAroundObjective || currentState == CURRENT_STATE.FollowingMarkers && currentMarker == null || currentState == CURRENT_STATE.WanderingAroundCircle)
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

    IEnumerator OnGoingToAMarker()
    {
        _agent.isStopped = false;
        while (true)
        {
            if (currentState != CURRENT_STATE.FollowingMarkers)
            {
                break;
            }
            if (Vector3.Distance(transform.position, currentMarker.transform.position) <= 0.1f)
            {
                break;
            }
            yield return null;
        }
        
    }

    IEnumerator CollectElixir()
    {
        _agent.isStopped = true;
        currentElixirStone.elixirAvailable--;
        if (currentElixirStone.elixirAvailable == 0)
        {
            currentElixirStone.empty = true;
            //change color
        }
        canCollect = false;
        yield return new WaitForSeconds(5f);
        currentElixirStone.bCollectorsIn--;
        if (BoogiesSpawner.BoogiesKnowCollectorMachinePosition) BringElixirToCollectorMachine();
        else
        {
            randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
            bool a = _agent.SetDestination(randomPoint);
            if (a == false)
            {
                Debug.LogError("Destination cant be reached. ");
            }
            _agent.isStopped = false;
            StartCoroutine(MarkRoad());
            lastMarker = Instantiate(marker, transform.position, Quaternion.identity);
            lastMarker.GetComponent<MarkerBehaviour>().previousMarker = null;
        }
    }

    private void BringElixirToCollectorMachine()
    {
        bool a = _agent.SetDestination(FindObjectOfType<CollectorMachineBehavior>().transform.position);
        currentPath = CURRENT_PATH.CollectorMachine;
        currentState = CURRENT_STATE.GoingToCollectorMachine;
        if (a == false)
        {
            Debug.LogError("Destination cant be reached. ");
        }
        _agent.isStopped = false;
        StartCoroutine(MarkRoad());
        lastMarker = Instantiate(marker, transform.position, Quaternion.identity);
        //lastMarker.GetComponent<MarkerBehaviour>().markerCreator = this;
        lastMarker.GetComponent<MarkerBehaviour>().previousMarker = null;
    }

    IEnumerator MarkRoad()
    {
        while (currentElixirStone != null)
        {
            yield return new WaitForSeconds(0.05f);
            GameObject currentMarker = Instantiate(marker, transform.position, Quaternion.identity);
            //lastMarker.GetComponent<MarkerBehaviour>().markerCreator = this;
            currentMarker.GetComponent<MarkerBehaviour>().previousMarker = lastMarker;
            lastMarker = currentMarker;
        }
        lastMarker = null;
        currentMarker = null;
    }

    public override void BackToPlayer()
    {
        backToPlayer = true;
        randomPoint = Vector3.positiveInfinity;
        bool a = _agent.SetDestination(FindObjectOfType<BoogiesSpawner>().gameObject.transform.position);
        currentPath = CURRENT_PATH.Player;
        currentState = CURRENT_STATE.GoingToPlayer;
        if (a == false)
        {
            Debug.LogError("Destination cant be reached. ");
        }
    }

    public override void OnObjectiveSelected()
    {
        Debug.Log("objective selected!");
    }
}
