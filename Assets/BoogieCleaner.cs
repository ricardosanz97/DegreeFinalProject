using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class BoogieCleaner : Boogie
{
    public bool canCharge = true;
    public bool canDeposit = false;
    public DebrisObstaclePart carriedObject;

    private void Start()
    {
        type = BoogieType.Cleaner;
        randomPoint = GetRandomPointAroundCircle(initialPoint);
        _agent.SetDestination(randomPoint);
    }

    private void Update()
    {
        if (RandomPointDestinationReached() && currentObjective == null)
        {
            randomPoint = GetRandomPointAroundCircle(initialPoint);
            _agent.SetDestination(randomPoint);
            if (!objectiveNotFoundTimerEnabled)
            {
                StartCoroutine(ObjectiveNotFound());
                objectiveNotFoundTimerEnabled = true;
            }
        }
        else if (RandomPointDestinationReached() && currentObjective != null)
        {
            randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
            _agent.SetDestination(randomPoint);
        }

        if (PlayerDestinationReached() && backToPlayer)
        {
            BoogiesSpawner.CleanersAmount++;
            Destroy(this.gameObject);
        }
    }

    private bool RandomPointDestinationReached()
    {
        return Vector3.Distance(this.transform.position, randomPoint) < 0.5f;
    }

    private bool PlayerDestinationReached()
    {
        return Vector3.Distance(this.transform.position, FindObjectOfType<BoogiesSpawner>().gameObject.transform.position) < 1f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<DebrisObstaclePart>() && other.GetComponent<DebrisObstaclePart>().charger == null) //debris on the ground
        {
            if (currentObjective == null)
            {
                SetObjective(other.GetComponentInParent<Obstacle>());
            }
            if (carriedObject == null)
            {
                if (!canCharge) return;
                if (other.GetComponent<DebrisObstaclePart>().settledDown) return;
                carriedObject = other.GetComponent<DebrisObstaclePart>();
                carriedObject.GetComponent<DebrisObstaclePart>().charger = this;
                carriedObject.transform.SetParent(this.transform, false);
                carriedObject.gameObject.transform.DOMove(this.transform.Find("ChargingPoint").transform.position, 0f);
                canDeposit = false;
                StartCoroutine(TimeToCanUnchargeAgainElapse());
            }
            else //if we have a debris part charged and touch another one
            {
                if (!canDeposit) return;
                if (other.GetComponent<DebrisObstaclePart>().settledDown)
                {
                    carriedObject.GetComponent<DebrisObstaclePart>().settledDown = true;
                }
                carriedObject.transform.DOMove(other.gameObject.transform.position, 0f);
                carriedObject.GetComponent<DebrisObstaclePart>().charger = null;
                carriedObject.transform.SetParent(currentObjective.transform, false);
                carriedObject = null;
                canCharge = false;
                StartCoroutine(TimeToCanChargeAgainElapse());
            }
        }

        if (other.GetComponent<BoogieCleaner>()) //other cleaner, we say him which is the objective ;)
        {
            if (other.GetComponent<BoogieCleaner>().currentObjective == null && currentObjective != null)
            {
                other.GetComponent<BoogieCleaner>().SetObjective(currentObjective);
            }
        }

        IEnumerator TimeToCanChargeAgainElapse()
        {
            yield return new WaitForSeconds(2f);
            canCharge = true;
        }

        IEnumerator TimeToCanUnchargeAgainElapse()
        {
            yield return new WaitForSeconds(2f);
            canDeposit = true;
        }
    }

    public IEnumerator CheckIfWorkFinished()
    {
        yield return new WaitForSeconds(2f);
        int counter = 0;
        DebrisObstaclePart[] parts = currentObjective.GetComponent<DebrisObstacle>().debrisParts;
        for (int i = 0; i < parts.Length; i++)
        {
            if (parts[i].settledDown)
            {
                counter++;
            }
        }

        if (counter == parts.Length)
        {
            BackToPlayer();
        }
        else
        {
            StartCoroutine(CheckIfWorkFinished());
        }
    }

    public override void BackToPlayer()
    {
        backToPlayer = true;
        randomPoint = Vector3.positiveInfinity;
        _agent.SetDestination(FindObjectOfType<BoogiesSpawner>().gameObject.transform.position);
    }

    public override void OnObjectiveSelected()
    {
        Debug.Log("init coroutine");
        StartCoroutine(CheckIfWorkFinished());
        //StopCoroutine(ObjectiveNotFound());
    }
}
