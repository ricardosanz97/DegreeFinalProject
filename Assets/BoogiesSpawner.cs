using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoogiesSpawner : MonoBehaviour
{
    public int currentAmount = 30;
    public int cleanersAmount = 1;
    public int wrestlersAmount = 0;
    public int collectorsAmount = 0;
    public int mudThrowersAmount = 0;
    public int explorersAmount = 0;

    public bool selectingObjective = false;
    public Obstacle obstacleSelected;
    public Obstacle lastObstacleSelected;
    public GameObject boogieCleaner;
    public GameObject boogieWrestler;
    public GameObject boogieCollector;
    public GameObject boogieMudThrower;
    public GameObject boogieExplorer;

    public float radiusSpawn = 2f;

    private void Update()
    {
        HandleInputSelect();
        if (obstacleSelected)
        {
            HandleObstacleType(obstacleSelected.type);
            lastObstacleSelected = obstacleSelected;
            obstacleSelected = null;
        }
    }

    private void HandleInputSelect()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            selectingObjective = !selectingObjective;
        }
    }

    private void HandleObstacleType(ObstacleType type)
    {
        switch (type)
        {
            case ObstacleType.Debris:
                StartCoroutine(SpawnCleanerBoogies());
                break;
            case ObstacleType.Enemy:
                StartCoroutine(SpawnWrestlersBoogies());
                break;
        }
    }

    private IEnumerator SpawnCleanerBoogies()
    {
        for (int i = 0; i<cleanersAmount; i++)
        {
            yield return new WaitForSeconds(0.1f);
            SpawnBoogie(BoogieType.Cleaner);
        }
        yield return null;
    }

    private IEnumerator SpawnWrestlersBoogies()
    {
        for (int i = 0; i<wrestlersAmount; i++)
        {
            yield return new WaitForSeconds(0.1f);
            SpawnBoogie(BoogieType.Wrestler);
        }
        yield return null;
    }

    private void SpawnBoogie(BoogieType type)
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
            case BoogieType.MudThrower:
                boogieToSpawn = boogieMudThrower;
                break;
        }
        GameObject b = Instantiate(boogieToSpawn, position, this.transform.rotation);
        b.GetComponent<Boogie>().SetObjective(lastObstacleSelected);
    }
}
