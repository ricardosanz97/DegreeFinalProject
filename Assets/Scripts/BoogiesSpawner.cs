using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
public class BoogiesSpawner : MonoBehaviour
{
    public int currentAmount = 30;
    public int leftBoogiesAmount = 0;
    public int cleanersAmount = 10;
    public int collectorsAmount = 5;
    public int explorersAmount = 10;

    public float radiusCircle;

    public bool canSpawnBoogies = false;
    public bool canSpawnSquad = false;

    public GameObject boogieCleaner;
    public GameObject boogieCollector;
    public GameObject boogieExplorer;

    public GameObject squadWrestlers;

    public int currentFormationSelected = 0;
    public BoogieWrestlerCommander commanderSquadSelected = null;

    public int cleanersSpawned;
    public int collectorsSpawned;
    public int explorersSpawned;

    public bool cleanersBackButtonPressed;
    public bool explorersBackButtonPressed;
    public bool collectorsBackButtonPressed;

    public static bool CleanersBackButtonPressed
    {
        get { return FindObjectOfType<BoogiesSpawner>().cleanersBackButtonPressed; }
        set { FindObjectOfType<BoogiesSpawner>().cleanersBackButtonPressed = value; }
    }

    public static bool ExplorersBackButtonPressed
    {
        get { return FindObjectOfType<BoogiesSpawner>().explorersBackButtonPressed; }
        set { FindObjectOfType<BoogiesSpawner>().explorersBackButtonPressed = value; }
    }

    public static bool CollectorsBackButtonPressed
    {
        get { return FindObjectOfType<BoogiesSpawner>().collectorsBackButtonPressed; }
        set { FindObjectOfType<BoogiesSpawner>().collectorsBackButtonPressed = value; }
    }

    #region configurations
    public CollectorsConfiguration collectorsConfig;
    public ExplorersConfiguration explorersConfig;
    public CleanersConfiguration cleanersConfig;
    public WrestlersConfiguration wrestlersConfig;
    #endregion

    public bool boogiesCleanersKnowCollectorMachinePosition = true;

    public float radiusSpawn = 2f;

    public static BoogieWrestlerCommander CommanderSelected
    {
        get { return FindObjectOfType<BoogiesSpawner>().commanderSquadSelected; }
        set { FindObjectOfType<BoogiesSpawner>().commanderSquadSelected = value; }
    }

    public static int CurrentTotalAmount
    {
        get { return FindObjectOfType<BoogiesSpawner>().currentAmount; }
        set { FindObjectOfType<BoogiesSpawner>().currentAmount = value; }
    }

    public static int CurrentBoogiesLeftAmount
    {
        get { return FindObjectOfType<BoogiesSpawner>().leftBoogiesAmount; }
        set { FindObjectOfType<BoogiesSpawner>().leftBoogiesAmount = value; }
    }

    public static int CleanersAmount
    {
        get { return FindObjectOfType<BoogiesSpawner>().cleanersAmount; }
        set { FindObjectOfType<BoogiesSpawner>().cleanersAmount = value; }
    }
    public static int CleanersSpawned
    {
        get { return FindObjectOfType<BoogiesSpawner>().cleanersSpawned; }
        set { FindObjectOfType<BoogiesSpawner>().cleanersSpawned = value; }
    }

    public static int CollectorsAmount
    {
        get { return FindObjectOfType<BoogiesSpawner>().collectorsAmount; }
        set { FindObjectOfType<BoogiesSpawner>().collectorsAmount = value; }
    }

    public static int CollectorsSpawned
    {
        get { return FindObjectOfType<BoogiesSpawner>().collectorsSpawned; }
        set { FindObjectOfType<BoogiesSpawner>().collectorsSpawned = value; }
    }

    public static int ExplorersAmount
    {
        get { return FindObjectOfType<BoogiesSpawner>().explorersAmount; }
        set { FindObjectOfType<BoogiesSpawner>().explorersAmount = value; }
    }

    public static int ExplorersSpawned
    {
        get { return FindObjectOfType<BoogiesSpawner>().explorersSpawned; }
        set { FindObjectOfType<BoogiesSpawner>().explorersSpawned = value; }
    }

    public static bool BoogiesKnowCollectorMachinePosition
    {
        get { return FindObjectOfType<BoogiesSpawner>().boogiesCleanersKnowCollectorMachinePosition; }
    }

    public static float RadiusCircle
    {
        get { return FindObjectOfType<BoogiesSpawner>().radiusCircle; }
        set { FindObjectOfType<BoogiesSpawner>().radiusCircle = value; }
    }

    public static bool CanSpawnBoogies
    {
        get { return FindObjectOfType<BoogiesSpawner>().canSpawnBoogies; }
    }

    public static bool CanSpawnSquad
    {
        get { return FindObjectOfType<BoogiesSpawner>().canSpawnSquad; }
    }

    public static void SetCanSpawnBoogies(bool value)
    {
        FindObjectOfType<BoogiesSpawner>().canSpawnBoogies = value;
    }

    public static void SetCanSpawnSquad(bool value)
    {
        FindObjectOfType<BoogiesSpawner>().canSpawnSquad = value;
    }

    public bool positionAccepted = false;
    public bool selectorEnabled = false;

    private void Start()
    {
        canSpawnBoogies = true;
        canSpawnSquad = true;
    }

    private void Update()
    {
        if (CleanersBackButtonPressed && cleanersSpawned == 0)
        {
            CleanersBackButtonPressed = false;
        }
        if (ExplorersBackButtonPressed && explorersSpawned == 0)
        {
            ExplorersBackButtonPressed = false;
        }
        if (CollectorsBackButtonPressed && collectorsSpawned == 0)
        {
            CollectorsBackButtonPressed = false;
        }
    }

    public static void SpawnBoogiesEnabled()
    {
        UIController.OnGroundClicked += FindObjectOfType<BoogiesSpawner>().SpawnAllBoogiesSelected;
    }

    public static void SpawnAllieSquadEnabled()
    {
        UIController.OnGroundClicked += UIController.I.OnSpawnAllieSquadPositionSelected;
    }

    public static void SpawnBoogiesDisabled()
    {
        UIController.OnGroundClicked -= FindObjectOfType<BoogiesSpawner>().SpawnAllBoogiesSelected;
    }

    public static void SpawnAllieSquadDisabled()
    {
        UIController.OnGroundClicked -= UIController.I.OnSpawnAllieSquadPositionSelected;
    }

    public static void SpawnEnemySquadEnabled()
    {
        UIController.OnGroundClicked += UIController.I.OnSpawnEnemySquadPositionSelected;
    }

    public static void SpawnEnemySquadDisabled()
    {
        UIController.OnGroundClicked -= UIController.I.OnSpawnEnemySquadPositionSelected;
    }

    public void SpawnAllBoogiesSelected(Vector3 clickPoint)
    {
        if (CleanersAmount > 0) { StartCoroutine(SpawnCleanerBoogies(clickPoint)); }
        if (CollectorsAmount > 0) { StartCoroutine(SpawnCollectorsBoogies(clickPoint)); }
        if (ExplorersAmount > 0) { StartCoroutine(SpawnExplorersBoogies(clickPoint)); }

        UIController.I.UIHideMouseSelector();
        UIController.OnGroundClicked -= SpawnAllBoogiesSelected;
        SetCanSpawnBoogies(true);
    }

    public void SpawnWrestlersSquad(Vector3 point)
    {
        GameObject parentSquad = Instantiate(squadWrestlers, point, Quaternion.identity) as GameObject;
        parentSquad.GetComponentInChildren<BoogieWrestlerCommander>().SetInitialPoint(point);
    }

    private IEnumerator SpawnCleanerBoogies(Vector3 clickPoint)
    {
        int spawnNumber = cleanersAmount;
        for (int i = 0; i<spawnNumber; i++)
        {
            yield return new WaitForSeconds(0.1f);
            SpawnBoogie(BoogieType.Cleaner, clickPoint);
            cleanersAmount--;
            Debug.Log("cleaners spawned + 1 ");
            cleanersSpawned++;
            FindObjectOfType<FastSelectorBoogiesController>().cleanersAmountSlider.onValueChanged.RemoveAllListeners();
            FindObjectOfType<FastSelectorBoogiesController>().cleanersAmountSlider.value--;
            FindObjectOfType<FastSelectorBoogiesController>().cleanersAmountText.text = cleanersAmount.ToString();
            FindObjectOfType<FastSelectorBoogiesController>().cleanersAmountSlider.GetComponent<SliderController>().AddListener();
        }
        yield return null;
    }

    private IEnumerator SpawnCollectorsBoogies(Vector3 clickPoint)
    {
        int spawnNumber = collectorsAmount;
        for (int i = 0; i<spawnNumber; i++)
        {
            yield return new WaitForSeconds(0.1f);
            SpawnBoogie(BoogieType.Collector, clickPoint);
            collectorsAmount--;
            collectorsSpawned++;
            FindObjectOfType<FastSelectorBoogiesController>().collectorsAmountSlider.onValueChanged.RemoveAllListeners();
            FindObjectOfType<FastSelectorBoogiesController>().collectorsAmountSlider.value--;
            FindObjectOfType<FastSelectorBoogiesController>().collectorsAmountText.text = collectorsAmount.ToString();
            FindObjectOfType<FastSelectorBoogiesController>().collectorsAmountSlider.GetComponent<SliderController>().AddListener();
        }
        yield return null;
    }

    private IEnumerator SpawnExplorersBoogies(Vector3 clickPoint)
    {
        int spawnNumber = explorersAmount;
        for (int i = 0; i < spawnNumber; i++)
        {
            yield return new WaitForSeconds(0.1f);
            SpawnBoogie(BoogieType.Explorer, clickPoint);
            explorersAmount--;
            explorersSpawned++;
            FindObjectOfType<FastSelectorBoogiesController>().explorersAmountSlider.onValueChanged.RemoveAllListeners();
            FindObjectOfType<FastSelectorBoogiesController>().explorersAmountSlider.value--;
            FindObjectOfType<FastSelectorBoogiesController>().explorersAmountText.text = explorersAmount.ToString();
            FindObjectOfType<FastSelectorBoogiesController>().explorersAmountSlider.GetComponent<SliderController>().AddListener();
        }
        yield return null;
    }

    public void ForceSpawnBoogies(BoogieType type, Vector3 position, int amount)
    {
        for (int i = 0; i<amount; i++)
        {
            switch (type)
            {
                case BoogieType.Cleaner:
                    ForceSpawnBoogie(BoogieType.Cleaner, position);
                    break;
                case BoogieType.Collector:
                    ForceSpawnBoogie(BoogieType.Collector, position);
                    break;
                case BoogieType.Explorer:
                    ForceSpawnBoogie(BoogieType.Explorer, position);
                    break;
            }
        }
    }

    private void ForceSpawnBoogie(BoogieType type, Vector3 clickPoint)
    {
        GameObject boogieToSpawn = null;
        //Vector3 randomPositionCenter = clickPoint + this.transform.forward * radiusSpawn * 2f;
        Vector3 position = new Vector3(Random.Range(clickPoint.x - (radiusSpawn * 2), clickPoint.x + (radiusSpawn * 3)), 0.5f, Random.Range(clickPoint.z - (radiusSpawn * 3), clickPoint.z + (radiusSpawn * 3)));
        switch (type)
        {
            case BoogieType.Cleaner:
                boogieToSpawn = boogieCleaner;
                break;
            case BoogieType.Explorer:
                boogieToSpawn = boogieExplorer;
                break;
            case BoogieType.Collector:
                boogieToSpawn = boogieCollector;
                break;
        }
        GameObject b = Instantiate(boogieToSpawn, position, LevelManager.I.collectorsSpawnPoint.rotation);
        b.GetComponent<Boogie>().initialPoint = position;
        b.GetComponent<Boogie>().maxTimeToFindObjective = Mathf.Infinity;
        b.GetComponent<Boogie>()._agent.isStopped = true;
        b.GetComponent<Boogie>().ForceIdle();
    }

    private void SpawnBoogie(BoogieType type, Vector3 clickPoint)
    {
        Vector3 randomPositionCenter = this.transform.position + this.transform.forward * radiusSpawn * 2f;
        Vector3 position = new Vector3(Random.Range(randomPositionCenter.x - radiusSpawn, randomPositionCenter.x + radiusSpawn), 0.5f, Random.Range(randomPositionCenter.z - radiusSpawn, randomPositionCenter.z + radiusSpawn));
        GameObject boogieToSpawn = null;
        switch (type)
        {
            case BoogieType.Cleaner:
                boogieToSpawn = boogieCleaner;
                break;
            case BoogieType.Explorer:
                boogieToSpawn = boogieExplorer;
                break;
            case BoogieType.Collector:
                boogieToSpawn = boogieCollector;
                break;
        }
        GameObject b = Instantiate(boogieToSpawn, position, this.transform.rotation);
        b.GetComponent<Boogie>().initialPoint = clickPoint;
    }

    public void SpawnAllieSquad(Vector3 position, Quaternion rotation, SquadConfiguration.Squad squadConfig)
    {
        //GameObject commander = Instantiate(Resources.Load("Prefabs/Wrestlers/BoogieWrestlerCommander"), position, Quaternion.identity) as GameObject;
        //GameObject commander = Instantiate(Resources.Load("Prefabs/Wrestlers/Enemies/BoogieWrestlerCommander"), position, Quaternion.identity) as GameObject;
        GameObject commander = Instantiate(Resources.Load("Prefabs/Wrestlers/Allies/BoogieWrestlerCommander"), position, rotation) as GameObject;
        //GameObject a = Instantiate(Resources.Load("Prefabs/Wrestlers/Enemies/BoogieWrestlerClose"), position, Quaternion.identity) as GameObject;
        GameObject squadGO = new GameObject("Squad");
        squadGO.AddComponent<SquadTeam>();
        squadGO.transform.position = position;
        squadGO.layer = LayerMask.NameToLayer("Squad");
        commander.transform.SetParent(squadGO.transform);

        int posI = squadConfig.leaderPosition.i;
        int posJ = squadConfig.leaderPosition.j;
        commander.GetComponent<BoogieWrestlerCommander>().leaderIndex = new SquadConfiguration.Index(posI, posJ);

        commander.GetComponent<BoogieWrestlerCommander>().squadInfo = squadConfig;
        commander.GetComponent<BoogieWrestlerCommander>().currentSquadList = new List<SquadConfiguration.SQUAD_ROL>(squadConfig.listFormation);

       

        SquadConfiguration.SquadSlot[,] squadSlots = squadConfig.squad;
        int counter = 0;
        for (int i = 0; i<squadConfig.squadRows; i++)
        {
            for (int j = 0; j<squadConfig.squadCols; j++)
            {
                GameObject wrestlerSpawned = null;
                switch (squadSlots[i,j].rol)
                {
                    case SquadConfiguration.SQUAD_ROL.Distance:
                        //wrestlerSpawned = Instantiate(Resources.Load("Prefabs/Wrestlers/BoogieWrestlerDistance"), position, Quaternion.identity, squadGO.transform) as GameObject;
                        //wrestlerSpawned = Instantiate(Resources.Load("Prefabs/Wrestlers/Enemies/BoogieWrestlerDistance"), position, Quaternion.identity, squadGO.transform) as GameObject;
                        wrestlerSpawned = Instantiate(Resources.Load("Prefabs/Wrestlers/Allies/BoogieWrestlerDistance"), position, Quaternion.identity, squadGO.transform) as GameObject;
                        wrestlerSpawned.GetComponent<BoogieWrestler>().listPosition = counter;
                        break;
                    case SquadConfiguration.SQUAD_ROL.Giant:
                        //wrestlerSpawned = Instantiate(Resources.Load("Prefabs/Wrestlers/BoogieWrestlerGiant"), position, Quaternion.identity, squadGO.transform) as GameObject;
                        //wrestlerSpawned = Instantiate(Resources.Load("Prefabs/Wrestlers/Enemies/BoogieWrestlerGiant"), position, Quaternion.identity, squadGO.transform) as GameObject;
                        wrestlerSpawned = Instantiate(Resources.Load("Prefabs/Wrestlers/Allies/BoogieWrestlerGiant"), position, Quaternion.identity, squadGO.transform) as GameObject;
                        wrestlerSpawned.GetComponent<BoogieWrestler>().listPosition = counter;
                        break;
                    case SquadConfiguration.SQUAD_ROL.Close:
                        //wrestlerSpawned = Instantiate(Resources.Load("Prefabs/Wrestlers/BoogieWrestlerClose"), position, Quaternion.identity, squadGO.transform) as GameObject;
                        //wrestlerSpawned = Instantiate(Resources.Load("Prefabs/Wrestlers/Enemies/BoogieWrestlerClose"), position, Quaternion.identity, squadGO.transform) as GameObject;
                        wrestlerSpawned = Instantiate(Resources.Load("Prefabs/Wrestlers/Allies/BoogieWrestlerClose"), position, Quaternion.identity, squadGO.transform) as GameObject;
                        wrestlerSpawned.GetComponent<BoogieWrestler>().listPosition = counter;
                        break;
                    case SquadConfiguration.SQUAD_ROL.Commander:
                        commander.GetComponent<BoogieWrestlerCommander>().indexs = squadSlots[i, j].position;
                        commander.GetComponent<BoogieWrestler>().isLeaderPosition = true;
                        commander.GetComponent<BoogieWrestlerCommander>().listPosition = counter;
                        break;
                }
                counter++;
                if (wrestlerSpawned != null)
                {
                    //wrestlerSpawned.GetComponent<BoogieWrestler>().initialIndexs = new SquadConfiguration.Index(squadSlots[i, j].position.i, squadSlots[i, j].position.j);
                    wrestlerSpawned.GetComponent<BoogieWrestler>().indexs = new SquadConfiguration.Index(squadSlots[i, j].position.i, squadSlots[i, j].position.j);
                } 
            }
        }
        currentFormationSelected = 0;
    }

    public void SpawnEnemySquad(Vector3 position, Quaternion rotation, SquadConfiguration.Squad squadConfig)
    {
        //GameObject commander = Instantiate(Resources.Load("Prefabs/Wrestlers/BoogieWrestlerCommander"), position, Quaternion.identity) as GameObject;
        GameObject commander = Instantiate(Resources.Load("Prefabs/Wrestlers/Enemies/EBoogieWrestlerCommander"), position, rotation) as GameObject;
        //GameObject commander = Instantiate(Resources.Load("Prefabs/Wrestlers/Allies/BoogieWrestlerCommander"), position, Quaternion.identity) as GameObject;
        //GameObject a = Instantiate(Resources.Load("Prefabs/Wrestlers/Enemies/BoogieWrestlerClose"), position, Quaternion.identity) as GameObject;
        GameObject squadGO = new GameObject("Squad");
        squadGO.AddComponent<SquadTeam>();
        squadGO.transform.position = position;
        squadGO.layer = LayerMask.NameToLayer("Squad");

        squadGO.GetComponent<SquadTeam>().team = TEAM.B;
        commander.transform.SetParent(squadGO.transform);

        int posI = squadConfig.leaderPosition.i;
        int posJ = squadConfig.leaderPosition.j;

        commander.GetComponent<BoogieWrestlerCommander>().squadInfo = squadConfig;
        commander.GetComponent<BoogieWrestlerCommander>().currentSquadList = new List<SquadConfiguration.SQUAD_ROL>(squadConfig.listFormation);

        commander.GetComponent<BoogieWrestlerCommander>().leaderIndex = new SquadConfiguration.Index(posI, posJ);

        SquadConfiguration.SquadSlot[,] squadSlots = squadConfig.squad;
        int counter = 0;
        for (int i = 0; i < squadConfig.squadRows; i++)
        {
            for (int j = 0; j < squadConfig.squadCols; j++)
            {
                GameObject wrestlerSpawned = null;
                switch (squadSlots[i, j].rol)
                {
                    case SquadConfiguration.SQUAD_ROL.Distance:
                        //wrestlerSpawned = Instantiate(Resources.Load("Prefabs/Wrestlers/BoogieWrestlerDistance"), position, Quaternion.identity, squadGO.transform) as GameObject;
                        wrestlerSpawned = Instantiate(Resources.Load("Prefabs/Wrestlers/Enemies/EBoogieWrestlerDistance"), position, Quaternion.identity, squadGO.transform) as GameObject;
                        //wrestlerSpawned = Instantiate(Resources.Load("Prefabs/Wrestlers/Allies/BoogieWrestlerDistance"), position, Quaternion.identity, squadGO.transform) as GameObject;
                        wrestlerSpawned.GetComponent<BoogieWrestler>().listPosition = counter;
                        break;
                    case SquadConfiguration.SQUAD_ROL.Giant:
                        //wrestlerSpawned = Instantiate(Resources.Load("Prefabs/Wrestlers/BoogieWrestlerGiant"), position, Quaternion.identity, squadGO.transform) as GameObject;
                        wrestlerSpawned = Instantiate(Resources.Load("Prefabs/Wrestlers/Enemies/EBoogieWrestlerGiant"), position, Quaternion.identity, squadGO.transform) as GameObject;
                        //wrestlerSpawned = Instantiate(Resources.Load("Prefabs/Wrestlers/Allies/BoogieWrestlerGiant"), position, Quaternion.identity, squadGO.transform) as GameObject;
                        wrestlerSpawned.GetComponent<BoogieWrestler>().listPosition = counter;
                        break;
                    case SquadConfiguration.SQUAD_ROL.Close:
                        //wrestlerSpawned = Instantiate(Resources.Load("Prefabs/Wrestlers/BoogieWrestlerClose"), position, Quaternion.identity, squadGO.transform) as GameObject;
                        wrestlerSpawned = Instantiate(Resources.Load("Prefabs/Wrestlers/Enemies/EBoogieWrestlerClose"), position, Quaternion.identity, squadGO.transform) as GameObject;
                        //wrestlerSpawned = Instantiate(Resources.Load("Prefabs/Wrestlers/Allies/BoogieWrestlerClose"), position, Quaternion.identity, squadGO.transform) as GameObject;
                        wrestlerSpawned.GetComponent<BoogieWrestler>().listPosition = counter;
                        break;
                    case SquadConfiguration.SQUAD_ROL.Commander:
                        commander.GetComponent<BoogieWrestlerCommander>().indexs = squadSlots[i, j].position;
                        commander.GetComponent<BoogieWrestler>().isLeaderPosition = true;
                        commander.GetComponent<BoogieWrestlerCommander>().listPosition = counter;
                        break;
                }
                counter++;
                if (wrestlerSpawned != null)
                {
                    //wrestlerSpawned.GetComponent<BoogieWrestler>().initialIndexs = new SquadConfiguration.Index(squadSlots[i, j].position.i, squadSlots[i, j].position.j);
                    wrestlerSpawned.GetComponent<BoogieWrestler>().indexs = new SquadConfiguration.Index(squadSlots[i, j].position.i, squadSlots[i, j].position.j);
                }
            }
        }
        currentFormationSelected = 0;
    }
}
