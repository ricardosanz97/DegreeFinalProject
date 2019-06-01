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

public abstract class Boogie : AttackTarget, ISaveable
{
    #region parameters
    public BoogieType type;
    [HideInInspector]public Vector3 initialPoint;
    [HideInInspector]public Vector3 randomPoint;
    public Obstacle currentObjective;
    public float maxTimeToFindObjective;
    public float timeToCheckIfWorkFinished;
    [HideInInspector]public bool objectiveNotFoundTimerEnabled = false;
    public bool backToPlayer = false;

    [HideInInspector]public NavMeshAgent _agent;
    [HideInInspector] public Animator _anim;

    public float minSpeed;
    public float maxSpeed;
    public float speed;
    public float probabilityChangeSpeed;
    public float timeToChangeSpeed;

    public long uniqueId;
    #endregion

    public virtual void OnEnable()
    {
        SaverManager.OnLoadData += Load;
        SaverManager.OnSaveData += Save;
    }

    public virtual void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _anim = GetComponent<Animator>();

        uniqueId = GetHashCode() * Random.Range(1, 9);
        while (SaverManager.I.uniqueBoogiesIds.Contains(uniqueId))
        {
            uniqueId = GetHashCode() * Random.Range(1, 9);
        }
    }

    public void ForceIdle()
    {
        StartCoroutine(ForcingIdle());
    }

    IEnumerator ForcingIdle()
    {
        yield return new WaitForSeconds(1f);
        _anim.SetInteger("state", -1);
    }

    public void SetObjective(Obstacle obs)
    {
        currentObjective = obs;
        switch (type)
        {
            case BoogieType.Cleaner:
                FindObjectOfType<BoogiesSpawner>().cleanersCleaning = true;
                break;
            case BoogieType.Collector:
                FindObjectOfType<BoogiesSpawner>().collectorsCollecting = true;
                break;
            case BoogieType.Explorer:
                FindObjectOfType<BoogiesSpawner>().explorersExploring = true;
                break;
        }
        OnObjectiveSelected();
    }

    public IEnumerator ObjectiveNotFound()
    {
        //Debug.Log("max time to find objective = " + maxTimeToFindObjective);
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
             0.16f,
             Random.Range(centerPoint.z - currentObjective._col.bounds.size.z, centerPoint.z + currentObjective._col.bounds.size.z));
    }

    public Vector3 GetRandomPointAroundCircle(Vector3 centerPoint)
    {
        Vector3 position = new Vector3(Random.Range(initialPoint.x - BoogiesSpawner.RadiusCircle, initialPoint.x + BoogiesSpawner.RadiusCircle),
            0.16f,
            Random.Range(initialPoint.z - BoogiesSpawner.RadiusCircle, initialPoint.z + BoogiesSpawner.RadiusCircle));
        return position;
        //while (_agent.)
    }

    public Vector3 GetRandomPointAroundPlayer(Vector3 centerPoint)
    {
        return new Vector3(Random.Range(centerPoint.x - 5f, centerPoint.x + 5f),
             -0.5f,
             Random.Range(centerPoint.z - 5f, centerPoint.z + 5f));
    }

    public IEnumerator HandleSpeed()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeToChangeSpeed);
            if (Random.value < probabilityChangeSpeed)
            {
                float newSpeed = Random.Range(minSpeed, maxSpeed);
                _agent.speed = newSpeed;
            }
            yield return null;
        }
    }

    public abstract void Save();

    public abstract void Load();

    private void OnDestroy()
    {
        if (FindObjectOfType<BoogiesSpawner>() != null)
        {
            switch (type)
            {
                case BoogieType.Cleaner:
                    if (FindObjectOfType<BoogiesSpawner>().cleanersCleaning)
                    {
                        FindObjectOfType<BoogiesSpawner>().cleanersCleaning = false;
                    }
                    break;
                case BoogieType.Explorer:
                    if (FindObjectOfType<BoogiesSpawner>().explorersExploring)
                    {
                        FindObjectOfType<BoogiesSpawner>().explorersExploring = false;
                    }
                    break;
                case BoogieType.Collector:
                    if (FindObjectOfType<BoogiesSpawner>().collectorsCollecting)
                    {
                        FindObjectOfType<BoogiesSpawner>().collectorsCollecting = false;
                    }
                    break;
                case BoogieType.Wrestler:
                    if (FindObjectOfType<BoogiesSpawner>().wrestlersWrestling)
                    {
                        FindObjectOfType<BoogiesSpawner>().wrestlersWrestling = false;
                    }
                    break;
            }
        }
    }
}
