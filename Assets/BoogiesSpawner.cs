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

    public bool selectingObjective = false;
    //public Obstacle obstacleSelected;
    //public Obstacle lastObstacleSelected;
    public GameObject boogieCleaner;
    public GameObject boogieWrestler;
    public GameObject boogieCollector;
    public GameObject boogieExplorer;

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

    private void Update()
    {
        currentAmount = (cleanersAmount + wrestlersAmount + collectorsAmount + explorersAmount) + leftBoogiesAmount;
        HandleInputSelect();
    }

    private void HandleInputSelect()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            selectingObjective = !selectingObjective;
            FindObjectOfType<CircleMouseBehavior>().GetComponentInChildren<SpriteRenderer>().enabled = selectingObjective;
        }
    }

    public void SpawnAllBoogiesSelected(Vector3 clickPoint)
    {
        if (CleanersAmount > 0) { StartCoroutine(SpawnCleanerBoogies(clickPoint)); }
        if (WrestlersAmount > 0) { StartCoroutine(SpawnWrestlersBoogies(clickPoint)); }
        if (CollectorsAmount > 0) { StartCoroutine(SpawnCollectorsBoogies(clickPoint)); }
        if (ExplorersAmount > 0) { StartCoroutine(SpawnExplorersBoogies(clickPoint)); }
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

    private IEnumerator SpawnWrestlersBoogies(Vector3 clickPoint)
    {
        int spawnNumber = wrestlersAmount;
        for (int i = 0; i<spawnNumber; i++)
        {
            yield return new WaitForSeconds(0.1f);
            SpawnBoogie(BoogieType.Wrestler, clickPoint);
            wrestlersAmount--; 
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
            case BoogieType.Wrestler:
                boogieToSpawn = boogieWrestler;
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
}
