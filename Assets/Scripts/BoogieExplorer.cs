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
    public bool canCarryClue = true;
    public float timeToCarryAgain;
    private BoogieExplorerAnimationController _beac;
    public Transform chargingPoint;
    public float probabilityFollowClue;

    public override void Awake()
    {
        base.Awake();
        _beac = GetComponent<BoogieExplorerAnimationController>();
    }

    public override void Load()
    {
        base.Load();
        string key = "BoogieExplorer" + uniqueId;
        int animState = SaverManager.I.saveData[key + "AnimState"];
        if (animState == -1)
        {
            _agent.enabled = false;
            this.transform.position = SaverManager.I.saveData[key + "Position"];
            this.transform.rotation = SaverManager.I.saveData[key + "Rotation"];
            _agent.enabled = true;
        }
        else
        {
            this.transform.position = SaverManager.I.saveData[key + "Position"];
            this.transform.rotation = SaverManager.I.saveData[key + "Rotation"];
        }
        _anim.SetInteger("state", animState);
        this.currentPath = SaverManager.I.saveData[key + "CurrentPath"];
        this.distanceTraveled = SaverManager.I.saveData[key + "DistanceTraveled"];
        this.currentState = SaverManager.I.saveData[key + "CurrentState"];
        this.currentCorridorIndex = SaverManager.I.saveData[key + "CurrentCorridorIndex"];
        this.clueCarried = SaverManager.I.saveData[key + "ClueCarried"];
        this.placingClue = SaverManager.I.saveData[key + "PlacingClue"];
        this.canCarryClue = SaverManager.I.saveData[key + "CanCarryClue"];
        this.currentObjective = SaverManager.I.saveData[key + "CurrentObjective"];
        this.backToPlayer = SaverManager.I.saveData[key + "BackToPlayer"];

        if (this.clueCarried != null)
        {
            clueCarried.transform.SetParent(this.transform.Find("ChargingPoint"), false);
            clueCarried.transform.localPosition = Vector3.zero;
        }
    }

    public override void Save()
    {
        base.Save();
        string key = "BoogieExplorer" + uniqueId;
        SaverManager.I.saveData.Add(key + "AnimState", _anim.GetInteger("state"));
        SaverManager.I.saveData.Add(key + "Position", this.transform.position);
        SaverManager.I.saveData.Add(key + "Rotation", this.transform.rotation);
        SaverManager.I.saveData.Add(key + "CurrentPath", currentPath);
        SaverManager.I.saveData.Add(key + "DistanceTraveled", distanceTraveled);
        SaverManager.I.saveData.Add(key + "CurrentState", currentState);
        SaverManager.I.saveData.Add(key + "CurrentCorridorIndex", currentCorridorIndex);
        SaverManager.I.saveData.Add(key + "ClueCarried", clueCarried);
        SaverManager.I.saveData.Add(key + "PlacingClue", placingClue);
        SaverManager.I.saveData.Add(key + "CanCarryClue", canCarryClue);
        SaverManager.I.saveData.Add(key + "CurrentObjective", currentObjective);
        SaverManager.I.saveData.Add(key + "BackToPlayer", backToPlayer);

    }

    public override void BackToPlayer()
    {
        _anim.SetInteger("state", 0);
        backToPlayer = true;
        randomPoint = FindObjectOfType<BoogiesSpawner>().transform.position;
        if (_agent.isOnNavMesh)
        {
            _agent.isStopped = false;
            _agent.SetDestination(randomPoint);
        }
        
        if (clueCarried != null)
        {
            clueCarried.carriedBy = null;
            Destroy(clueCarried.gameObject);
            _anim.SetInteger("state", 0);
        }
        currentState = CURRENT_STATE.GoingToPlayer;
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
        UnityEngine.AI.NavMeshPath nmp = new UnityEngine.AI.NavMeshPath();
        while (!_agent.CalculatePath(randomPoint, nmp))
        {
            Debug.Log("buscando nuevo camino");
            randomPoint = GetRandomPointAroundCircle(initialPoint);
        }
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
        _agent.speed = Random.Range(minSpeed, maxSpeed);
        probabilityChangeSpeed = Ccfg.probabilityVariateSpeed;
        timeToChangeSpeed = Ccfg.timeTryVariateSpeed;
        timeToCarryAgain = Ccfg.timeToCarryAgain;

        probabilityFollowClue = Ccfg.probabilityFollowClue;
    }

    private void Update()
    {
        if (!backToPlayer && currentState == CURRENT_STATE.FindingMultipathBegin && RandomPointDestinationReached())
        {
            randomPoint = GetRandomPointAroundCircle(initialPoint);
            UnityEngine.AI.NavMeshPath nmp = new UnityEngine.AI.NavMeshPath();
            while (!_agent.CalculatePath(randomPoint, nmp))
            {
                randomPoint = GetRandomPointAroundCircle(initialPoint);
            }
            _agent.SetDestination(randomPoint);
        }
        else if (currentState == CURRENT_STATE.GoingToPlayer && RandomPointDestinationReached())
        {
            randomPoint = FindObjectOfType<BoogiesSpawner>().transform.position;
            _agent.SetDestination(randomPoint);
            BoogiesSpawner.ExplorersAmount++;
            BoogiesSpawner.ExplorersSpawned--;
            FindObjectOfType<FastSelectorBoogiesController>().explorersAmountSlider.onValueChanged.RemoveAllListeners();
            FindObjectOfType<FastSelectorBoogiesController>().explorersAmountSlider.value++;
            FindObjectOfType<FastSelectorBoogiesController>().explorersAmountText.text = BoogiesSpawner.ExplorersAmount.ToString();
            FindObjectOfType<FastSelectorBoogiesController>().explorersAmountSlider.GetComponent<SliderController>().AddListener();
            Destroy(this.gameObject);
        }

        else if (currentState == CURRENT_STATE.GoingToEnd && RandomPointDestinationReached())
        {
            _anim.SetInteger("state", -1);
            _agent.isStopped = true;
        }

        if (BoogiesSpawner.ExplorersBackButtonPressed)
        {
            BackToPlayer();
        }
    }

    private bool PlayerDestinationReached()
    {
        return Vector3.Distance(this.transform.position, FindObjectOfType<BoogiesSpawner>().gameObject.transform.position) < 1f;
    }

    private bool RandomPointDestinationReached()
    {
        return Vector3.Distance(this.transform.position, randomPoint) < 1f;
    }

    IEnumerator PickClue()
    {
        _agent.isStopped = true;
        _anim.SetInteger("state", 2);
        currentState = CURRENT_STATE.CarryingClue;
        yield return new WaitForSeconds(_beac.pickClueAnimation.length);
        _anim.SetInteger("state", 3);
        _agent.isStopped = false;
        randomPoint = currentPath.GetComponentInParent<MultipathController>().firstPath.begin.transform.position;
        _agent.SetDestination(randomPoint);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!backToPlayer && other.GetComponent<ClueBehavior>()) //collide with a clue
        {
            ClueBehavior cb = other.GetComponent<ClueBehavior>();
            if (!cb.placed && canCarryClue/*&& cb.placedBy != this */&& cb.carriedBy == null && this.clueCarried == null)
            {
                Vector3 direction = cb.transform.position - this.transform.position;
                direction.y = 0;
                this.transform.rotation = Quaternion.LookRotation(direction);
                cb.carriedBy = this;
                this.clueCarried = cb;
                other.transform.SetParent(this.chargingPoint);
                other.transform.localPosition = Vector3.zero;
                StartCoroutine(PickClue());
            }
        }

        if (!backToPlayer && currentState != CURRENT_STATE.CarryingClue && other.GetComponent<CorridorBegin>()) //init of the path
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

        if (!backToPlayer && currentState == CURRENT_STATE.CarryingClue && other.GetComponent<CorridorBegin>())
        {
            if (placingClue) return;
            PathBehavior pb = other.transform.parent.GetComponentInChildren<PathBehavior>();
            if (pb.FirstPath)
            {
                currentPath = pb;
                StartCoroutine(PlaceClue());
            }
        }

        else if (!backToPlayer && currentState != CURRENT_STATE.CarryingClue && other.GetComponent<CorridorEnd>()) //end of the path
        {
            MultipathController mc = other.GetComponentInParent<MultipathController>();
            PathBehavior pb = other.transform.parent.GetComponentInChildren<PathBehavior>();
            if (this.currentCorridorIndex == pb.CorridorCorrectIndex)
            {
                if (mc.clues[currentPath.PathIndex] == null)
                {
                    StartCoroutine(DropClue(this.currentCorridorIndex));
                }
                else
                {
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
            }
            else
            {
                RestartMultipath();
            }
        }

        if (!backToPlayer && currentState != CURRENT_STATE.CarryingClue && other.GetComponent<PathCorridorTrigger>())
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

        if (mc.clues[currentPath.PathIndex] != null && Random.value < probabilityFollowClue)
        {
            int indexCorridor = mc.clues[currentPath.PathIndex].corridorIndex;
            randomPoint = currentPath.corridors[indexCorridor].transform.position;
        }
        else
        {
            randomPoint = currentPath.corridors[Random.Range(0, currentPath.corridors.Length)].transform.position;
        }

        _agent.SetDestination(randomPoint);
    }

    IEnumerator DropClue(int correctIndex)
    {
        canCarryClue = false;
        _agent.isStopped = true;
        _anim.SetInteger("state", 1);
        yield return new WaitForSeconds(_beac.dropClueAnimation.length);
        GameObject clue = Instantiate(Resources.Load("Prefabs/Clues/" + correctIndex + "Clue"), this.transform.position, Quaternion.identity) as GameObject;
        clue.GetComponent<ClueBehavior>().placedBy = this;
        clue.GetComponent<ClueBehavior>().corridorIndex = this.currentCorridorIndex;
        clue.GetComponent<ClueBehavior>().pathIndex = currentPath.PathIndex;
        currentState = CURRENT_STATE.GoingToInitNextPath;
        _agent.isStopped = false;
        _anim.SetInteger("state", 0);
        if (currentPath.nextInitCorridor != null)
        {
            randomPoint = currentPath.nextInitCorridor.transform.position;
            _agent.SetDestination(randomPoint);
        }
        else
        {
            currentState = CURRENT_STATE.GoingToEnd;
        }
        StartCoroutine(CanCarryClueAgain());

    }

    private IEnumerator CanCarryClueAgain()
    {
        yield return new WaitForSeconds(timeToCarryAgain);
        canCarryClue = true;
    }

    private IEnumerator PlaceClue()
    {
        placingClue = true;
        yield return StartCoroutine(GoToPlacingPosition());

        if (currentObjective.GetComponentInChildren <MultipathController>().clues[clueCarried.pathIndex] == null && 
            clueCarried.corridorIndex == int.Parse(currentObjective.GetComponentInChildren<MultipathController>().combination[clueCarried.pathIndex].ToString()))
        {
            yield return StartCoroutine(PlaceClueInSite());
        }
        else
        {
            if (clueCarried != null)
            {
                Destroy(this.clueCarried.gameObject);
                this.clueCarried = null;
                _anim.SetInteger("state", 0);
            }
        }

        SelectCorridor();
        currentState = CURRENT_STATE.GoingToOneCorridor;
        placingClue = false;
    }

    IEnumerator PlaceClueInSite()
    {
        _agent.isStopped = true;
        _anim.SetInteger("state", 4);
        currentObjective.GetComponentInChildren<MultipathController>().clues[clueCarried.pathIndex] = this.clueCarried;
        this.clueCarried.placed = true;
        this.clueCarried.transform.SetParent(null, false);
        yield return new WaitForSeconds(_beac.depositClueAnimation.length);
        Vector3 position = new Vector3(currentPath.begin.transform.position.x, 0.4f + (clueCarried.pathIndex * clueCarried.GetComponent<Collider>().bounds.size.y), currentPath.begin.transform.position.z);
        this.clueCarried.transform.position = position;
        this.clueCarried.carriedBy = null;
        this.clueCarried = null;
        _anim.SetInteger("state", 0);
        _agent.isStopped = false;
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
        _anim.SetInteger("state", 0);
        if (this.clueCarried != null)
        {
            Destroy(this.clueCarried);
        }
        randomPoint = GetRandomPointAroundCircle(initialPoint);
        UnityEngine.AI.NavMeshPath nmp = new UnityEngine.AI.NavMeshPath();
        while (!_agent.CalculatePath(randomPoint, nmp))
        {
            randomPoint = GetRandomPointAroundCircle(initialPoint);
        }
        this.transform.localPosition = randomPoint;
        currentState = CURRENT_STATE.FindingMultipathBegin;
        currentPath = currentObjective.GetComponentInChildren<MultipathController>().firstPath;
        this.currentCorridorIndex = -1;
        randomPoint = GetRandomPointAroundCircle(initialPoint);
        UnityEngine.AI.NavMeshPath nmp2 = new UnityEngine.AI.NavMeshPath();
        while (!_agent.CalculatePath(randomPoint, nmp2))
        {
            randomPoint = GetRandomPointAroundCircle(initialPoint);
        }
        _agent.SetDestination(randomPoint);
    }

    IEnumerator CheckIfWorkFinished()
    {
        if (currentObjective.GetComponentInChildren<MultipathController>().playerCompleteMultipath)
        {
            BackToPlayer();
        }
        yield return new WaitForSeconds(timeToCheckIfWorkFinished);
        StartCoroutine(CheckIfWorkFinished());
    }

    public override void Die()
    {
        throw new System.NotImplementedException();
    }
}
