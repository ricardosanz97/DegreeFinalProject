using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoogiesSpawner : MonoBehaviour
{
    public int currentAmount = 30;
    public int leftBoogiesAmount = 0;
    public int currentElixir = 10;
    public int wrestlersAmount = 5;
    public int cleanersAmount = 10;
    public int collectorsAmount = 5;
    public int explorersAmount = 10;

    public bool selectingAlliesObjective = false;
    public bool selectingSquadWrestlerSpawn = false;

    public GameObject boogieCleaner;
    public GameObject boogieWrestler;
    public GameObject boogieCollector;
    public GameObject boogieExplorer;

    public GameObject squadWrestlers;

    public bool boogiesCleanersKnowCollectorMachinePosition = true;

    public float radiusSpawn = 2f;

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

    public bool positionAccepted = false;
    public bool selectorEnabled = false;

    private void Update()
    {
        currentAmount = (cleanersAmount + wrestlersAmount + collectorsAmount + explorersAmount) + leftBoogiesAmount;
        HandleInputSelect();
    }

    private void HandleInputSelect()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            HandleAlliesSelect();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            HandleWrestlersSelect();
        }
    }

    public void HandleWrestlersSelect()
    {
        selectingSquadWrestlerSpawn = !selectingSquadWrestlerSpawn;
        selectorEnabled = selectingSquadWrestlerSpawn;
        FindObjectOfType<SelectorMouseBehavior>().GetComponentInChildren<SpriteRenderer>().enabled = selectingSquadWrestlerSpawn;
    }

    public void HandleAlliesSelect()
    {
        selectingAlliesObjective = !selectingAlliesObjective;
        FindObjectOfType<CircleMouseAlliesBehavior>().GetComponentInChildren<SpriteRenderer>().enabled = selectingAlliesObjective;
    }

    public void SpawnAllBoogiesSelected(Vector3 clickPoint)
    {
        if (CleanersAmount > 0) { StartCoroutine(SpawnCleanerBoogies(clickPoint)); }
        if (CollectorsAmount > 0) { StartCoroutine(SpawnCollectorsBoogies(clickPoint)); }
        if (ExplorersAmount > 0) { StartCoroutine(SpawnExplorersBoogies(clickPoint)); }
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

    public void CreateSquad(int level)
    {
        Vector3 position = FindObjectOfType<SelectorMouseBehavior>().clickPosition;
        //FindObjectOfType<SelectorMouseBehavior>().GetComponentInChildren<SpriteRenderer>().enabled = false;
        switch (level)
        {
            case 1:
                SquadConfiguration.Squad squadFirst = new SquadConfiguration.Squad(SquadConfiguration.SQUAD_LEVEL.First);
                SquadConfiguration.SquadSlot[,] squad1 = squadFirst.squad;
                SpawnSquad(position, squad1);
                break;
            case 2:
                SquadConfiguration.Squad squadSecond = new SquadConfiguration.Squad(SquadConfiguration.SQUAD_LEVEL.Second);
                SquadConfiguration.SquadSlot[,] squad2 = squadSecond.squad;
                SpawnSquad(position, squad2);
                break;
            case 3:
                SquadConfiguration.Squad squadThird = new SquadConfiguration.Squad(SquadConfiguration.SQUAD_LEVEL.Third);
                SquadConfiguration.SquadSlot[,] squad3 = squadThird.squad;
                SpawnSquad(position, squad3);
                break;
        }
    }

    private void SpawnSquad(Vector3 position, SquadConfiguration.SquadSlot[,] squad)
    {
        GameObject squadGO = new GameObject("squad");
        for (int i = 0; i<Mathf.Sqrt(squad.Length); i++)
        {
            for (int j = 0; j<Mathf.Sqrt(squad.Length); j++)
            {
                GameObject wrestlerSpawned = null;
                switch (squad[i,j].rol)
                {
                    case SquadConfiguration.SQUAD_ROL.Behind:
                        wrestlerSpawned = Instantiate(Resources.Load("Prefabs/Wrestlers/BoogieWrestlerDistance"), position, Quaternion.identity, squadGO.transform) as GameObject;
                        break;
                    case SquadConfiguration.SQUAD_ROL.Commander:
                        wrestlerSpawned = Instantiate(Resources.Load("Prefabs/Wrestlers/BoogieWrestlerCommander"), position, Quaternion.identity, squadGO.transform) as GameObject;
                        break;
                    case SquadConfiguration.SQUAD_ROL.CornerBehind:
                        wrestlerSpawned = Instantiate(Resources.Load("Prefabs/Wrestlers/BoogieWrestlerDistance"), position, Quaternion.identity, squadGO.transform) as GameObject;
                        break;
                    case SquadConfiguration.SQUAD_ROL.CornerFront:
                        wrestlerSpawned = Instantiate(Resources.Load("Prefabs/Wrestlers/BoogieWrestlerGiant"), position, Quaternion.identity, squadGO.transform) as GameObject;
                        break;
                    case SquadConfiguration.SQUAD_ROL.Front:
                        wrestlerSpawned = Instantiate(Resources.Load("Prefabs/Wrestlers/BoogieWrestlerClose"), position, Quaternion.identity, squadGO.transform) as GameObject;
                        break;
                    case SquadConfiguration.SQUAD_ROL.Lateral:
                        wrestlerSpawned = Instantiate(Resources.Load("Prefabs/Wrestlers/BoogieWrestlerClose"), position, Quaternion.identity, squadGO.transform) as GameObject;
                        break;
                }
                wrestlerSpawned.GetComponent<BoogieWrestler>().indexs = squad[i, j].position;
            }
        }
    }
}
