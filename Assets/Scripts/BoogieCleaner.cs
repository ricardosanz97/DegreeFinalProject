using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

public class BoogieCleaner : Boogie
{
    public float timeToChargeAgain;
    public float timeToUnchargeAgain;
    public bool canCharge = true;
    public bool canDeposit = false;
    public DebrisObstaclePart carriedObject;

    public Transform chargePoint;

    private BoogieCleanerAnimationController _bcac;

    public override void Awake()
    {
        base.Awake();
        _bcac = GetComponent<BoogieCleanerAnimationController>();
    }

    private void Start()
    {
        AssignConfiguration();

        type = BoogieType.Cleaner;
        randomPoint = GetRandomPointAroundCircle(initialPoint);
        UnityEngine.AI.NavMeshPath nmp = new UnityEngine.AI.NavMeshPath();
        while (!_agent.CalculatePath(randomPoint, nmp))
        {
            randomPoint = GetRandomPointAroundCircle(initialPoint);
        }
        _agent.SetDestination(randomPoint);

        StartCoroutine(HandleSpeed());
    }

    private void AssignConfiguration()
    {
        CleanersConfiguration Ccfg = FindObjectOfType<BoogiesSpawner>().cleanersConfig;
        maxTimeToFindObjective = Ccfg.maxTimeToFindObjective;
        timeToCheckIfWorkFinished = Ccfg.timeToCheckIfWorkWinished;
        timeToChargeAgain = Ccfg.timeToChargeAgain;
        timeToUnchargeAgain = Ccfg.timeToUnchargeAgain;

        minSpeed = Ccfg.minSpeed;
        maxSpeed = Ccfg.maxSpeed;

        _agent.speed = Random.Range(minSpeed, maxSpeed);
        probabilityChangeSpeed = Ccfg.probabilityVariateSpeed;
        timeToChangeSpeed = Ccfg.timeTryVariateSpeed;
    }

    private void Update()
    {
        if (!backToPlayer && RandomPointDestinationReached() && currentObjective == null)
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
        else if (!backToPlayer && RandomPointDestinationReached() && currentObjective != null)
        {
            randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
            UnityEngine.AI.NavMeshPath nmp = new UnityEngine.AI.NavMeshPath();
            while (!_agent.CalculatePath(randomPoint, nmp))
            {
                randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
            }
            _agent.SetDestination(randomPoint);
        }

        if (PlayerDestinationReached() && backToPlayer)
        {
            BoogiesSpawner.CleanersAmount++;
            BoogiesSpawner.CleanersSpawned--;
            FindObjectOfType<FastSelectorBoogiesController>().cleanersAmountSlider.onValueChanged.RemoveAllListeners();
            FindObjectOfType<FastSelectorBoogiesController>().cleanersAmountSlider.value++;
            FindObjectOfType<FastSelectorBoogiesController>().cleanersAmountText.text = BoogiesSpawner.CleanersAmount.ToString();
            FindObjectOfType<FastSelectorBoogiesController>().cleanersAmountSlider.GetComponent<SliderController>().AddListener();
            Destroy(this.gameObject);
        }

        if (BoogiesSpawner.CleanersBackButtonPressed || backToPlayer)
        {
            BackToPlayer();
        }
    }

    private bool RandomPointDestinationReached()
    {
        return Vector3.Distance(this.transform.position, randomPoint) < 1f;
    }

    private bool PlayerDestinationReached()
    {
        return Vector3.Distance(this.transform.position, FindObjectOfType<BoogiesSpawner>().gameObject.transform.position) < 1f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!backToPlayer && other.GetComponent<DebrisObstaclePart>() && other.GetComponent<DebrisObstaclePart>().charger == null) //debris on the ground
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
                Vector3 direction = other.transform.position - this.transform.position;
                direction.y = 0f;
                this.transform.rotation = Quaternion.LookRotation(direction);
                StartCoroutine(GrabDebrisPart());
            }
            else //if we have a debris part charged and touch another one
            {
                if (!canDeposit) return;
                if (other.GetComponent<DebrisObstaclePart>().settledDown)
                {
                    carriedObject.GetComponent<DebrisObstaclePart>().settledDown = true;
                }
                Vector3 direction = other.transform.position - this.transform.position;
                direction.y = 0f;
                this.transform.rotation = Quaternion.LookRotation(direction);
                StartCoroutine(DepositDebrisPart(new Vector3(other.gameObject.transform.position.x, carriedObject.initialY, 
                    other.gameObject.transform.position.z)));
            }
        }

        if (!backToPlayer && other.GetComponent<BoogieCleaner>()) //other cleaner, we say him which is the objective ;)
        {
            if (other.GetComponent<BoogieCleaner>().currentObjective == null && currentObjective != null)
            {
                other.GetComponent<BoogieCleaner>().SetObjective(currentObjective);
            }
        }
    }

    IEnumerator TimeToCanChargeAgainElapse()
    {
        yield return new WaitForSeconds(timeToChargeAgain);
        canCharge = true;
    }

    IEnumerator TimeToCanUnchargeAgainElapse()
    {
        yield return new WaitForSeconds(timeToUnchargeAgain);
        canDeposit = true;
    }

    IEnumerator GrabDebrisPart()
    {
        canCharge = false;
        canDeposit = false;
        _anim.SetInteger("state", 1);
        _agent.isStopped = true;
        carriedObject.charger = this;
        carriedObject.transform.SetParent(this.chargePoint, true);
        yield return new WaitForSeconds(_bcac.grabDebrisPartAnimation.length);
        if (carriedObject != null)
        {
            carriedObject.gameObject.transform.DOLocalMove(Vector3.zero, 0f);
        }
        
        StartCoroutine(TimeToCanUnchargeAgainElapse());
        if (currentObjective != null)
        {
            randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
            UnityEngine.AI.NavMeshPath nmp = new UnityEngine.AI.NavMeshPath();
            while (!_agent.CalculatePath(randomPoint, nmp))
            {
                randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
            }
        }
        else
        {
            randomPoint = GetRandomPointAroundCircle(initialPoint);
            UnityEngine.AI.NavMeshPath nmp = new UnityEngine.AI.NavMeshPath();
            while (!_agent.CalculatePath(randomPoint, nmp))
            {
                randomPoint = GetRandomPointAroundCircle(initialPoint);
            }
        }
        _anim.SetInteger("state", 2);
        _agent.SetDestination(randomPoint);
        _agent.isStopped = false;
        canCharge = true;
    }

    IEnumerator DepositDebrisPart(Vector3 position)
    {
        canCharge = false;
        canDeposit = false;
        _anim.SetInteger("state", 3);
        _agent.isStopped = true;
        carriedObject.transform.SetParent(currentObjective.transform, true);
        carriedObject.transform.DOMove(position, 2.5f);
        yield return new WaitForSeconds(_bcac.depositDebrisPartAnimation.length);
        if (carriedObject != null)
        {
            carriedObject.charger = null;
            carriedObject = null;
        }
        StartCoroutine(TimeToCanChargeAgainElapse());
        if (currentObjective != null)
        {
            randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
            UnityEngine.AI.NavMeshPath nmp = new UnityEngine.AI.NavMeshPath();
            while (!_agent.CalculatePath(randomPoint, nmp))
            {
                randomPoint = GetRandomPointAroundObjective(currentObjective.transform.position);
            }
        }
        else
        {
            randomPoint = GetRandomPointAroundCircle(initialPoint);
            UnityEngine.AI.NavMeshPath nmp = new UnityEngine.AI.NavMeshPath();
            while (!_agent.CalculatePath(randomPoint, nmp))
            {
                randomPoint = GetRandomPointAroundCircle(initialPoint);
            }
        }
        _anim.SetInteger("state", 0);
        _agent.SetDestination(randomPoint);
        _agent.isStopped = false;
        canDeposit = true;
    }

    public IEnumerator CheckIfWorkFinished()
    {
        yield return new WaitForSeconds(timeToCheckIfWorkFinished);
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
            if (!this.currentObjective.completed)
            {
                this.currentObjective.completed = true;
            }
            
            BackToPlayer();
        }
        else
        {
            StartCoroutine(CheckIfWorkFinished());
        }
    }

    public override void BackToPlayer()
    {
        if (!backToPlayer)
        {
            _anim.SetInteger("state", 0);
        }
        backToPlayer = true;
        randomPoint = FindObjectOfType<BoogiesSpawner>().gameObject.transform.position;

        if (_agent.isOnNavMesh)
        {
            _agent.isStopped = false;
            _agent.SetDestination(randomPoint);
        }
        

        if (carriedObject != null)
        {
            carriedObject.transform.SetParent(currentObjective.transform, true);
            carriedObject.transform.position = new Vector3(carriedObject.transform.position.x, carriedObject.initialY, carriedObject.transform.position.z);
            carriedObject.charger = null;
            carriedObject = null;
        }

        StopCoroutine(DepositDebrisPart(Vector3.zero));
        StopCoroutine(GrabDebrisPart());
    }

    public override void OnObjectiveSelected()
    {
        Debug.Log("init coroutine");
        StartCoroutine(CheckIfWorkFinished());
    }

    public override void Die()
    {
        throw new System.NotImplementedException();
    }
}
