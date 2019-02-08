using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class BoogieCleaner : Boogie
{
    private NavMeshAgent _agent;
    private Vector3 initialPoint;
    private Vector3 randomPoint;

    public bool canCharge = true;
    public bool canDeposit = false;
    public bool backToPlayer = false;
    public DebrisObstaclePart carriedObject;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        StartCoroutine(CheckIfWorkFinished());
        type = BoogieType.Cleaner;
        initialPoint = currentObjective.transform.position;
        randomPoint = new Vector3(Random.Range(initialPoint.x - currentObjective._boxCol.size.x, initialPoint.x + currentObjective._boxCol.size.x), -0.5f, 
            Random.Range(initialPoint.z - currentObjective._boxCol.size.z, initialPoint.z + currentObjective._boxCol.size.z));
        _agent.SetDestination(randomPoint);
    }

    private void Update()
    {
        if (RandomPointDestinationReached())
        {
            randomPoint = new Vector3(Random.Range(initialPoint.x - currentObjective._boxCol.size.x, initialPoint.x + currentObjective._boxCol.size.x), -0.5f,
            Random.Range(initialPoint.z - currentObjective._boxCol.size.z, initialPoint.z + currentObjective._boxCol.size.z));
            _agent.SetDestination(randomPoint);
        }

        if (PlayerDestinationReached())
        {
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
        if (other.GetComponent<DebrisObstaclePart>() && other.GetComponent<DebrisObstaclePart>().charger == null)
        {
            if (carriedObject == null)
            {
                if (!canCharge) return;
                if (other.GetComponent<DebrisObstaclePart>().settledDown) return;
                carriedObject = other.GetComponent<DebrisObstaclePart>();
                carriedObject.GetComponent<DebrisObstaclePart>().charger = this;
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
                carriedObject = null;
                canCharge = false;
                StartCoroutine(TimeToCanChargeAgainElapse());
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

    IEnumerator CheckIfWorkFinished()
    {
        yield return new WaitForSeconds(2f);
        int counter = 0;
        for (int i = 0; i<currentObjective.gameObject.transform.childCount; i++)
        {
            Debug.Log("checking");
            if (currentObjective.gameObject.transform.GetChild(i).GetComponent<DebrisObstaclePart>().settledDown)
            {
                counter++;
            }
        }

        if (counter == currentObjective.gameObject.transform.childCount)
        {
            BackToPlayer();
        }
        else
        {
            StartCoroutine(CheckIfWorkFinished());
        }
    }

    public void BackToPlayer()
    {
        Debug.Log("back to player!");
        backToPlayer = true;
        randomPoint = Vector3.positiveInfinity;
        _agent.SetDestination(FindObjectOfType<BoogiesSpawner>().gameObject.transform.position);
    }


}
