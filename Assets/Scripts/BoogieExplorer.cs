using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoogieExplorer : Boogie
{
    public PathBehavior currentPath;
    public Vector3 initialPosition;
    public Vector3 distanceTraveled;
    public CURRENT_STATE currentState;
    public int currentCorridorIndex;

    public override void BackToPlayer()
    {
        throw new System.NotImplementedException();
    }

    public override void OnObjectiveSelected()
    {
        StartCoroutine(CheckIfWorkFinished());
    }
    public enum CURRENT_STATE
    {
        CarryingClue,
        GoingToInitCorridor,
        GoingToOneCorridor,
        FindingMultipathBegin
    }

    private void Start()
    {
        type = BoogieType.Explorer;
        randomPoint = GetRandomPointAroundCircle(initialPoint);
        _agent.SetDestination(randomPoint);
        currentState = CURRENT_STATE.FindingMultipathBegin;
    }

    private void Update()
    {
        if (currentState == CURRENT_STATE.FindingMultipathBegin && RandomPointDestinationReached())
        {
            randomPoint = GetRandomPointAroundCircle(initialPoint);
            _agent.SetDestination(randomPoint);
            currentState = CURRENT_STATE.FindingMultipathBegin;
        }
        if (currentState == CURRENT_STATE.GoingToInitCorridor && RandomPointDestinationReached())
        {
            randomPoint = GetRandomPointAroundCircle(initialPoint);
            _agent.SetDestination(randomPoint);
            if (!objectiveNotFoundTimerEnabled)
            {
                StartCoroutine(ObjectiveNotFound());
                objectiveNotFoundTimerEnabled = true;
            }
        }

        if (currentState == CURRENT_STATE.GoingToOneCorridor && RandomPointDestinationReached())
        {
            if (currentPath.nextInitCorridor != null)
            {
                randomPoint = currentPath.nextInitCorridor.transform.position;
            }
            _agent.SetDestination(randomPoint);
            currentState = CURRENT_STATE.GoingToInitCorridor;
        }
    }

    private bool RandomPointDestinationReached()
    {
        return Vector3.Distance(this.transform.position, randomPoint) < 1f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CorridorBegin>())
        {
            Debug.Log("detecting collision with CorridorBegin");
            MultipathController mc = other.GetComponentInParent<MultipathController>();
            PathBehavior pb = other.transform.parent.GetComponentInChildren<PathBehavior>();
            this.distanceTraveled = (this.initialPosition - this.transform.localPosition);
            if (pb.FirstCorridor)
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
                this.initialPosition = this.transform.position;
                distanceTraveled = Vector3.zero;

                currentPath = pb;
                currentPath.bExplorersIn++;

                randomPoint = currentPath.corridors[Random.Range(0, currentPath.corridors.Length)].transform.position;
                _agent.SetDestination(randomPoint);
                currentState = CURRENT_STATE.GoingToOneCorridor;
            }
            else
            {
                currentPath.bExplorersIn--;
                currentPath = null;
                if (this.currentCorridorIndex == pb.LastCorridorCorrectIndex)
                {
                    Debug.Log("correct");
                    Debug.Log("drop clue. ");
                    currentPath = pb;

                    randomPoint = currentPath.corridors[Random.Range(0, currentPath.corridors.Length)].transform.position;
                    _agent.SetDestination(randomPoint);
                    currentState = CURRENT_STATE.GoingToOneCorridor;
                }
                else
                {
                    Debug.Log("incorrect. ");
                    this.transform.position += this.distanceTraveled;
                    this.distanceTraveled = Vector3.zero;
                }
            }
        }

        if (other.GetComponent<PathCorridorTrigger>())
        {
            this.currentCorridorIndex = other.GetComponent<PathCorridorTrigger>().CorriderIndex;
        }
    }

    IEnumerator CheckIfWorkFinished()
    {
        Debug.Log("checking. ");
        yield return new WaitForSeconds(2f);
        StartCoroutine(CheckIfWorkFinished());
    }

}
