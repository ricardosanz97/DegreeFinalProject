using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum BoogieType
{
    Cleaner,
    Wrestler,
    Explorer,
    Collector
}

public abstract class Boogie : MonoBehaviour
{
    public BoogieType type;
    [HideInInspector]public Vector3 initialPoint;
    public Vector3 randomPoint;
    public Obstacle currentObjective;
    public float maxTimeToFindObjective = 10f;
    public bool objectiveNotFoundTimerEnabled = false;
    public bool backToPlayer = false;

    [HideInInspector]public NavMeshAgent _agent;

    public virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();   
    }

    public void SetObjective(Obstacle obs)
    {
        currentObjective = obs;
        OnObjectiveSelected();
    }

    public IEnumerator ObjectiveNotFound()
    {
        yield return new WaitForSeconds(maxTimeToFindObjective);
        if (currentObjective == null)
        {
            BackToPlayer();
        }
        yield return null;
        
    }
    public abstract void OnObjectiveSelected();
    public abstract void BackToPlayer();

    public Vector3 GetRandomPointAroundObjective(Vector3 centerPoint)
    {
        return new Vector3(Random.Range(centerPoint.x - currentObjective._col.bounds.size.x, centerPoint.x + currentObjective._col.bounds.size.x),
             -0.5f,
             Random.Range(centerPoint.z - currentObjective._col.bounds.size.z, centerPoint.z + currentObjective._col.bounds.size.z));
    }

    public Vector3 GetRandomPointAroundCircle(Vector3 centerPoint)
    {
        return new Vector3(Random.Range(initialPoint.x - BoogiesSpawner.RadiusCircle, initialPoint.x + BoogiesSpawner.RadiusCircle),
            -0.5f,
            Random.Range(initialPoint.z - BoogiesSpawner.RadiusCircle, initialPoint.z + BoogiesSpawner.RadiusCircle));
    }

    public Vector3 GetRandomPointAroundPlayer(Vector3 centerPoint)
    {
        return new Vector3(Random.Range(centerPoint.x - 5f, centerPoint.x + 5f),
             -0.5f,
             Random.Range(centerPoint.z - 5f, centerPoint.z + 5f));
    }
}
