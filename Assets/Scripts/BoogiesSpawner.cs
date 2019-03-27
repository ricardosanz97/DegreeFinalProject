using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class BoogiesSpawner : MonoBehaviour
{
    public int currentAmount = 30;
    public int leftBoogiesAmount = 0;
    public int currentElixir = 10;
    public int wrestlersAmount = 5;
    public int cleanersAmount = 10;
    public int collectorsAmount = 5;
    public int explorersAmount = 10;

    public float radiusCircle;

    public bool canSpawnBoogies = false;
    public bool canSpawnSquad = false;

    public GameObject boogieCleaner;
    public GameObject boogieWrestler;
    public GameObject boogieCollector;
    public GameObject boogieExplorer;

    public GameObject squadWrestlers;

    public int currentFormationSelected = 0;
    public BoogieWrestlerCommander commanderSquadSelected = null;

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

    public static int CurrentElixir
    {
        get { return FindObjectOfType<BoogiesSpawner>().currentElixir; }
        set { FindObjectOfType<BoogiesSpawner>().currentElixir = value; }
    }

    public static int CleanersAmount
    {
        get { return FindObjectOfType<BoogiesSpawner>().cleanersAmount; }
        set { FindObjectOfType<BoogiesSpawner>().cleanersAmount = value; }
    }

    public static int WrestlersAmount
    {
        get { return FindObjectOfType<BoogiesSpawner>().wrestlersAmount; }
        set { FindObjectOfType<BoogiesSpawner>().wrestlersAmount = value; }
    }

    public static int CollectorsAmount
    {
        get { return FindObjectOfType<BoogiesSpawner>().collectorsAmount; }
        set { FindObjectOfType<BoogiesSpawner>().collectorsAmount = value; }
    }

    public static int ExplorersAmount
    {
        get { return FindObjectOfType<BoogiesSpawner>().explorersAmount; }
        set { FindObjectOfType<BoogiesSpawner>().explorersAmount = value; }
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
        currentAmount = (cleanersAmount + wrestlersAmount + collectorsAmount + explorersAmount) + leftBoogiesAmount;
    }

    public static void SpawnBoogiesEnabled()
    {
        UIController.OnGroundClicked += FindObjectOfType<BoogiesSpawner>().SpawnAllBoogiesSelected;
    }

    public static void SpawnSquadEnabled()
    {
        UIController.OnGroundClicked += UIController.I.OnSpawnSquadPositionSelected;
    }

    public static void SpawnBoogiesDisabled()
    {
        UIController.OnGroundClicked -= FindObjectOfType<BoogiesSpawner>().SpawnAllBoogiesSelected;
    }

    public static void SpawnSquadDisabled()
    {
        UIController.OnGroundClicked -= UIController.I.OnSpawnSquadPositionSelected;
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
        }
        yield return null;
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

    public void SpawnSquad(Vector3 position, SquadConfiguration.Squad squadConfig)
    {
        GameObject commander = Instantiate(Resources.Load("Prefabs/Wrestlers/BoogieWrestlerCommander"), position, Quaternion.identity) as GameObject;
        GameObject squadGO = new GameObject("Squad");
        squadGO.transform.position = position;
        squadGO.layer = LayerMask.NameToLayer("Squad");
        commander.transform.SetParent(squadGO.transform);

        int posI = squadConfig.leaderPosition.i;
        int posJ = squadConfig.leaderPosition.j;

        commander.GetComponent<BoogieWrestlerCommander>().squadInfo = squadConfig;
        commander.GetComponent<BoogieWrestlerCommander>().currentSquadList = new List<SquadConfiguration.SQUAD_ROL>(squadConfig.listFormation);

        commander.GetComponent<BoogieWrestlerCommander>().leaderIndex = new SquadConfiguration.Index(posI,posJ);

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
                        wrestlerSpawned = Instantiate(Resources.Load("Prefabs/Wrestlers/BoogieWrestlerDistance"), position, Quaternion.identity, squadGO.transform) as GameObject;
                        wrestlerSpawned.GetComponent<BoogieWrestler>().listPosition = counter;
                        break;
                    case SquadConfiguration.SQUAD_ROL.Giant:
                        wrestlerSpawned = Instantiate(Resources.Load("Prefabs/Wrestlers/BoogieWrestlerGiant"), position, Quaternion.identity, squadGO.transform) as GameObject;
                        wrestlerSpawned.GetComponent<BoogieWrestler>().listPosition = counter;
                        break;
                    case SquadConfiguration.SQUAD_ROL.Close:
                        wrestlerSpawned = Instantiate(Resources.Load("Prefabs/Wrestlers/BoogieWrestlerClose"), position, Quaternion.identity, squadGO.transform) as GameObject;
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
