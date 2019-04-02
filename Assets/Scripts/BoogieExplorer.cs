using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoogieExplorer : Boogie
{
    public PathBehavior currentPath;
    [HideInInspector]public Vector3 initialPosition;
    [HideInInspector]public Vector3 distanceTraveled;
    public CURRENT_STATE currentState;
    public int currentCorridorIndex;
    public ClueBehavior clueCarried;
    public bool placingClue;

    public override void BackToPlayer()
    {
        randomPoint = FindObjectOfType<BoogiesSpawner>().transform.position;
        _agent.SetDestination(randomPoint);
    }

    public override void OnObjectiveSelected()
    {
        StartCoroutine(CheckIfWorkFinished());
    }
    public enum CURRENT_STATE
    {
        CarryingClue,
        GoingToInitNextPath,
        GoingToEndPath,
        GoingToOneCorridor,
        FindingMultipathBegin,
        GoingToEnd,
        GoingToPlayer
    }

    private void Start()
    {
        AssignConfiguration();

        type = BoogieType.Explorer;
        randomPoint = GetRandomPointAroundCircle(initialPoint);
        _agent.SetDestination(randomPoint);
        currentState = CURRENT_STATE.FindingMultipathBegin;

        StartCoroutine(HandleSpeed());
    }

    private void AssignConfiguration()
    {
        ExplorersConfiguration Ccfg = FindObjectOfType<BoogiesSpawner>().explorersConfig;
        maxTimeToFindObjective = Ccfg.maxTimeToFindObjective;
        timeToCheckIfWorkFinished = Ccfg.timeToCheckIfWorkWinished;

        minSpeed = Ccfg.minSpeed;
        maxSpeed = Ccfg.maxSpeed;
        probabilityChangeSpeed = Ccfg.probabilityVariateSpeed;
        timeToChangeSpeed = Ccfg.timeTryVariateSpeed;
    }

    private void Update()
    {
        if (currentState == CURRENT_STATE.FindingMultipathBegin && RandomPointDestinationReached())
        {
            randomPoint = GetRandomPointAroundCircle(initialPoint);
            _agent.SetDestination(randomPoint);
        }
        else if (currentState == CURRENT_STATE.GoingToPlayer && RandomPointDestinationReached())
        {
            randomPoint = FindObjectOfType<BoogiesSpawner>().transform.position;
            _agent.SetDestination(randomPoint);
            Destroy(this.gameObject);
            BoogiesSpawner.ExplorersAmount++;
        }
    }

    private bool RandomPointDestinationReached()
    {
        return Vector3.Distance(this.transform.position, randomPoint) < 1f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<ClueBehavior>()) //collide with a clue
        {
            ClueBehavior cb = other.GetComponent<ClueBehavior>();
            if (!cb.placed && cb.placedBy != this && cb.carriedBy == null && this.clueCarried == null)
            {
                cb.carriedBy = this;
                this.clueCarried = cb;
                other.transform.SetParent(this.gameObject.transform);
                currentState = CURRENT_STATE.CarryingClue;
                randomPoint = currentPath.GetComponentInParent<MultipathController>().firstPath.begin.transform.position;
                _agent.SetDestination(randomPoint);
            }
        }

        if (currentState != CURRENT_STATE.CarryingClue && other.GetComponent<CorridorBegin>()) //init of the path
        {
            currentState = CURRENT_STATE.GoingToOneCorridor;
            MultipathController mc = other.GetComponentInParent<MultipathController>();
            PathBehavior pb = other.transform.parent.GetComponentInChildren<PathBehavior>();

            if (pb.FirstPath)
            {
                if (currentObjective == null)
                {
                    SetObjective(other.GetComponentInParent<MultipathObstacle>());
                    if (!objectiveNotFoundTimerEnabled)
                    {
                        StartCoroutine(ObjectiveNotFound());
                        objectiveNotFoundTimerEnabled = true;
                    }
                } 
            }
            currentPath = pb;
            SelectCorridor();
        }

        if (currentState == CURRENT_STATE.CarryingClue && other.GetComponent<CorridorBegin>())
        {
            if (placingClue) return;
            PathBehavior pb = other.transform.parent.GetComponentInChildren<PathBehavior>();
            if (pb.FirstPath)
            {
                currentPath = pb;
                StartCoroutine(PlaceClue());
            }
        }

        else if (currentState != CURRENT_STATE.CarryingClue && other.GetComponent<CorridorEnd>()) //end of the path
        {
            MultipathController mc = other.GetComponentInParent<MultipathController>();
            PathBehavior pb = other.transform.parent.GetComponentInChildren<PathBehavior>();
            if (this.currentCorridorIndex == pb.CorridorCorrectIndex)
            {
                if (mc.clues[currentPath.PathIndex] == null)
                {
                    DropClue(this.currentCorridorIndex);
                }
                currentState = CURRENT_STATE.GoingToInitNextPath;
                if (currentPath.nextInitCorridor != null)
                {
                    randomPoint = currentPath.nextInitCorridor.transform.position;
                    _agent.SetDestination(randomPoint);
                }
                else
                {
                    currentState = CURRENT_STATE.GoingToEnd;
                    //BackToPlayer();
                }
            }
            else
            {
                RestartMultipath();
            }
        }

        if (currentState != CURRENT_STATE.CarryingClue && other.GetComponent<PathCorridorTrigger>())
        {
            this.currentCorridorIndex = other.GetComponent<PathCorridorTrigger>().CorriderIndex;
            currentState = CURRENT_STATE.GoingToEndPath;
            randomPoint = currentPath.end.transform.position;
            _agent.SetDestination(randomPoint);
        }
    }

    private void SelectCorridor()
    {
        MultipathController mc = currentObjective.GetComponentInChildren<MultipathController>();
        if (mc.clues[currentPath.PathIndex] == null)
        {
            randomPoint = currentPath.corridors[Random.Range(0, currentPath.corridors.Length)].transform.position;
        }
        else
        {
            int indexCorridor = mc.clues[currentPath.PathIndex].corridorIndex;
            randomPoint = currentPath.corridors[indexCorridor].transform.position;
        }
        _agent.SetDestination(randomPoint);
    }

    private void DropClue(int correctIndex)
    {
        GameObject clue = Instantiate(Resources.Load("Prefabs/Clues/" + correctIndex + "Clue"), this.transform.position, Quaternion.identity) as GameObject;
        clue.GetComponent<ClueBehavior>().placedBy = this;
        clue.GetComponent<ClueBehavior>().corridorIndex = this.currentCorridorIndex;
        clue.GetComponent<ClueBehavior>().pathIndex = currentPath.PathIndex;
    }

    private IEnumerator PlaceClue()
    {
        placingClue = true;
        yield return StartCoroutine(GoToPlacingPosition());

        if (currentObjective.GetComponentInChildren <MultipathController>().clues[clueCarried.pathIndex] == null)
        {
            currentObjective.GetComponentInChildren<MultipathController>().clues[clueCarried.pathIndex] = this.clueCarried;
            this.clueCarried.placed = true;
            this.clueCarried.transform.SetParent(null, false);
            Vector3 position = new Vector3(currentPath.begin.transform.position.x, currentPath.begin.transform.position.y + (clueCarried.pathIndex * clueCarried.GetComponent<Collider>().bounds.size.y), currentPath.begin.transform.position.z);
            this.clueCarried.transform.position = position;
            this.clueCarried.carriedBy = null;
            this.clueCarried = null;
            
        }
        else
        {
            Destroy(this.clueCarried.gameObject);
            this.clueCarried = null;
        }

        SelectCorridor();
        currentState = CURRENT_STATE.GoingToOneCorridor;
        placingClue = false;
    }

    private IEnumerator GoToPlacingPosition()
    {
        Vector3 destination = currentPath.begin.transform.position;
        _agent.SetDestination(destination);
        while (Vector3.Distance(this.transform.position, destination) > 0.5f) {
            yield return null;
        }
    }

    private void RestartMultipath()
    {
        if (this.clueCarried != null)
        {
            Destroy(this.clueCarried);
        }
        this.transform.localPosition = GetRandomPointAroundCircle(initialPoint);
        currentState = CURRENT_STATE.FindingMultipathBegin;
        currentPath = currentObjective.GetComponentInChildren<MultipathController>().firstPath;
        this.currentCorridorIndex = -1;
        randomPoint = GetRandomPointAroundCircle(initialPoint);
        _agent.SetDestination(randomPoint);
    }

    IEnumerator CheckIfWorkFinished()
    {
        if (currentObjective.GetComponentInChildren<MultipathController>().playerCompleteMultipath)
        {
            currentState = CURRENT_STATE.GoingToPlayer;
            backToPlayer = true;
            BackToPlayer();
        }
        yield return new WaitForSeconds(timeToCheckIfWorkFinished);
        StartCoroutine(CheckIfWorkFinished());
    }

}
