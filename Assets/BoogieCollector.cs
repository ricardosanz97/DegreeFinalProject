using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using System;

public class BoogieCollector : Boogie
{
    public float timeToCollect = 5f;
    public bool canCollect = true;
    public ElixirObstacleStone currentElixirStone;
    public GameObject marker;

    private void Start()
    {
        type = BoogieType.Collector;
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

        if (PlayerDestinationReached() && currentElixirStone != null)
        {
            BoogiesSpawner.CurrentElixir++; //TODO: this is not definitive;
            currentElixirStone = null;
            canCollect = true;
            randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
            _agent.SetDestination(randomPoint);
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
        if (other.GetComponent<ElixirObstacleStone>() && !other.GetComponent<ElixirObstacleStone>().empty && other.GetComponent<ElixirObstacleStone>().bCollectorsIn < 3 && canCollect)
            //if we get a non-full stone
        {
            if (currentObjective == null)
            {
                SetObjective(other.GetComponentInParent<Obstacle>());
            }
            currentElixirStone = other.GetComponent<ElixirObstacleStone>();
            currentElixirStone.bCollectorsIn++;
            StartCoroutine(CollectElixir());
        }
    }

    IEnumerator CollectElixir()
    {
        _agent.isStopped = true;
        if (currentElixirStone.elixirAvailable == 0)
        {
            currentElixirStone.empty = true;
            //change color
        }
        canCollect = false;
        yield return new WaitForSeconds(5f);
        BringElixirToPlayer();
    }

    private void BringElixirToPlayer()
    {
        _agent.SetDestination(FindObjectOfType<BoogiesSpawner>().transform.position);
        _agent.isStopped = false;
        StartCoroutine(MarkRoad());
    }

    IEnumerator MarkRoad()
    {
        while (currentElixirStone != null)
        {
            yield return new WaitForSeconds(0.5f);
            Instantiate(marker, transform.position, Quaternion.identity);
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
        Debug.Log("objective selected!");
    }
}
