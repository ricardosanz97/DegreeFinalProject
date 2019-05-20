using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
public enum GAME_STEP
{
    Step0,//start game
    Step1,//we start talking anciant
    Step2,//we finish talking anciant
    Step3,//we kill the commander
    Step4,//we clean the debris
    Step5, //we get the explorers
    Step6,//we start talking aidil
    Step7,//we finish talking aidil
    Step8,//we enable the teleport
    Step9,//we get the collectors
    Step10,//we enable teleport
    Step11,//we use teleport -> battle
    Step12,//we defeat the anciant. Battle ended.
    Step13,//level completed
    Step14,
    Step15
}
public class LevelManager : Singleton<LevelManager>
{
    public DebrisObstacle debrisObstacle;
    public ElixirObstacle elixirObstacle;
    public MultipathObstacle multipathObstacle;
    [HideInInspector]public BoogieWrestlerCommander allieCommander;
    [HideInInspector]public BoogieWrestlerCommander initialEnemyCommander;

    public Transform spawnCommanderSquadPosition;
    public Transform spawnFirstWrestlers;
    public Transform spawnFinalEnemyWrestlers;

    public BoogieWrestlerDistance[] distances;
    public BoogieWrestlerGiant[] giants;
    public Transform[] distancePositions;
    public Transform[] giantPositions;

    public GameObject doorAncient;
    public GameObject anciantConversation;

    public GameObject doorCollectors1;
    public GameObject doorCollectors2;

    public GameObject aidilConversation;
    public GameObject giantsConversation;
    public GameObject distancesConversation;
    public GameObject lodwigConversation;

    public Transform explorersSpawnPoint;
    public Transform collectorsSpawnPoint;
    public Transform cleanersSpawnPoint;

    public bool conversationWithAnciantFinished;
    public bool conversationWithAidilFinished;
    public bool conversationWithGiantsFinished;
    public bool conversationWithDistancesFinished;

    public GAME_STEP currentStep;
    public bool cleanersDoorOpened = false;
    public bool collectorsDoorOpened = false;

    public PortalController portalCollectors;
    public PortalController portalInitialRoom;

    public Image fadeBlackImage;
    public GameObject ancient;
    public Transform lodwigPosition;

    private void Start()
    {
        DoorCollectors[] d = FindObjectsOfType<DoorCollectors>();
        doorCollectors1 = d[0].gameObject;
        doorCollectors2 = d[1].gameObject;
        OnChangeStep(GAME_STEP.Step0);
    }

    private void Update()
    {
        if (debrisObstacle.completed && currentStep == GAME_STEP.Step3)
        {
            //FindObjectOfType<PlayerMovement>()._agent.areaMask = NavMesh.AllAreas;
            OnChangeStep(GAME_STEP.Step4);
        }

        if (elixirObstacle.completed && currentStep == GAME_STEP.Step8)
        {
            OnChangeStep(GAME_STEP.Step9);
        }

        if (initialEnemyCommander != null && initialEnemyCommander.currentState == STATE.OnSquadAttacking)
        {
            //ACTIVAR MUSICA EPICA DE BATALLA
        }

        if (currentStep == GAME_STEP.Step11)
        {
            BoogieWrestler[] wrestlers = FindObjectsOfType<BoogieWrestler>();
            bool enemyAlives = false;
            foreach (BoogieWrestler bw in wrestlers)
            {
                if (bw.team == TEAM.B)
                {
                    enemyAlives = true;
                }
            }

            if (!enemyAlives)
            {
                OnChangeStep(GAME_STEP.Step12);
            }
        }
    }

    public IEnumerator PositionateDistances()
    {
        for (int i = 0; i < distances.Length; i++)
        {
            while (distances[i].commander == null)
            {
                yield return null;
            }
            distances[i].BreakFormation();
            distances[i]._agent.enabled = false;
            distances[i].transform.position = distancePositions[i].position; distances[i].transform.rotation = distancePositions[i].rotation;
            distances[i]._agent.enabled = true;
        }
    }

    public IEnumerator PositionateGiants()
    {
        for (int i = 0; i < giants.Length; i++)
        {
            while (giants[i].commander == null)
            {
                yield return null;
            }
            giants[i].BreakFormation();
            giants[i]._agent.enabled = false;
            giants[i].transform.position = giantPositions[i].position; giants[i].transform.rotation = giantPositions[i].rotation;
            giants[i]._agent.enabled = true;
        }
    }

    public void OpenAncientDoor()
    {
        for (int i = 0; i<allieCommander.squadWrestlers.Count; i++)
        {
            if (allieCommander.squadWrestlers[i].wrestlerType != BoogieWrestler.WRESTLER_TYPE.Distance && allieCommander.squadWrestlers[i].wrestlerType != BoogieWrestler.WRESTLER_TYPE.Giant)
            {
                allieCommander.squadWrestlers[i]._agent.areaMask = NavMesh.AllAreas; //except the non-walkable
            }
        }

        int mask = 0;
        mask += 1 << NavMesh.GetAreaFromName("Walkable");
        mask += 1 << NavMesh.GetAreaFromName("InsideJailWrestlers");
        mask += 1 << NavMesh.GetAreaFromName("FrontJail");
        FindObjectOfType<PlayerMovement>()._agent.areaMask = mask;

        doorAncient.transform.DOLocalMoveY(6f, 2f);
        Debug.Log("door opened");
    }

    public void InitialCommanderDead()
    {
        Debug.Log("initial commander dead");
        FindObjectOfType<LevelManager>().OnChangeStep(GAME_STEP.Step3);
    }

    public void ConversationAncientFinished()
    {
        Debug.Log("conversation ancient finished!");
        OnChangeStep(GAME_STEP.Step2);
    }

    public void OpenCleanersDoors()
    {
        GameObject door = FindObjectOfType<DoorCleaners>().gameObject;
        door.transform.DOLocalMoveY(6f, 2.5f).OnComplete(() => {
            cleanersDoorOpened = true;
            FindObjectOfType<PlayerMovement>()._agent.areaMask += 1 << NavMesh.GetAreaFromName("InsideJailCleaners");
        });
    }

    public void OpenCollectorsDoor()
    {
        DoorCollectors[] collectorDoors = FindObjectsOfType<DoorCollectors>();
        doorCollectors1 = collectorDoors[0].gameObject;
        doorCollectors2 = collectorDoors[1].gameObject;
        doorCollectors1.transform.DOLocalMoveY(6f, 2.5f);
        doorCollectors2.transform.DOLocalMoveY(6f, 2.5f).OnComplete(()=> {
            collectorsDoorOpened = true;
            FindObjectOfType<PlayerMovement>()._agent.areaMask += 1 << NavMesh.GetAreaFromName("InsideJailCollectors");
        });
    }

    public void ConversationWithAidilFinished()
    {
        OnChangeStep(GAME_STEP.Step8);
    }
    
    public void ConversationWithGiantsFinished()
    {
        OnChangeStep(GAME_STEP.Step6);
    }

    public void ConversationWithDistancesFinished()
    {
        OnChangeStep(GAME_STEP.Step11);
    }

    IEnumerator SpawnAllieWrestlers()
    {
        Squad newSquad = AssetDatabase.LoadAssetAtPath("Assets/Resources/Squads/Allies/Allies.asset", typeof(ScriptableObject)) as Squad;
        SquadConfiguration.Squad squadConfig = new SquadConfiguration.Squad(newSquad.squadName, newSquad.squadRol, newSquad.numRows, newSquad.numCols, newSquad.customConfiguration);
        FindObjectOfType<SquadConfiguration>().currentSquadSelected = new SquadConfiguration.Squad(newSquad.squadName, newSquad.squadRol, newSquad.numRows, newSquad.numCols, newSquad.customConfiguration);
        FindObjectOfType<BoogiesSpawner>().SpawnAllieSquad(spawnFirstWrestlers.position, spawnFirstWrestlers.rotation, FindObjectOfType<SquadConfiguration>().currentSquadSelected);

        allieCommander = FindObjectOfType<BoogieWrestlerCommander>();
        distances = allieCommander.transform.parent.GetComponentsInChildren<BoogieWrestlerDistance>();
        giants = allieCommander.transform.parent.GetComponentsInChildren<BoogieWrestlerGiant>();

        foreach (BoogieWrestlerDistance bwd in distances)
        {
            bwd.transform.GetChild(0).gameObject.SetActive(false);
        }
        foreach (BoogieWrestlerGiant bwg in giants)
        {
            bwg.transform.GetChild(0).gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(2f);

        for (int i = 0; i < giants.Length; i++)
        {
            giants[i].transform.GetChild(0).gameObject.SetActive(true);
            giants[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < distances.Length; i++)
        {
            distances[i].transform.GetChild(0).gameObject.SetActive(true);
            distances[i].gameObject.SetActive(false);
        }
    }

    private void SpawnFinalEnemyWrestlers()
    {
        Squad newSquad = AssetDatabase.LoadAssetAtPath("Assets/Resources/Squads/Enemies/Finalenemy.asset", typeof(ScriptableObject)) as Squad;
        SquadConfiguration.Squad squadConfig2 = new SquadConfiguration.Squad(newSquad.squadName, newSquad.squadRol, newSquad.numRows, newSquad.numCols, newSquad.customConfiguration);
        FindObjectOfType<SquadConfiguration>().currentSquadSelected = new SquadConfiguration.Squad(newSquad.squadName, newSquad.squadRol, newSquad.numRows, newSquad.numCols, newSquad.customConfiguration);
        FindObjectOfType<BoogiesSpawner>().SpawnEnemySquad(spawnFinalEnemyWrestlers.position, spawnFinalEnemyWrestlers.rotation, FindObjectOfType<SquadConfiguration>().currentSquadSelected);
    }

    IEnumerator LodwigDies()
    {
        yield return new WaitForSeconds(2f);
        lodwigConversation.SetActive(true);
        ancient.GetComponent<Animator>().SetInteger("state", 2);
    }

    public void ConversationLodwigFinished()
    {
        OnChangeStep(GAME_STEP.Step13);
    }

    public void OnChangeStep(GAME_STEP step)
    {
        currentStep = step;
        Debug.Log("now we are on " + currentStep.ToString());
        switch (step)
        {
            case GAME_STEP.Step0:
                //spawn first wrestlers
                StartCoroutine(SpawnAllieWrestlers());

                //spawn commander
                Squad newSquad2 = AssetDatabase.LoadAssetAtPath("Assets/Resources/Squads/Enemies/Commander.asset", typeof(ScriptableObject)) as Squad;
                SquadConfiguration.Squad squadConfig2 = new SquadConfiguration.Squad(newSquad2.squadName, newSquad2.squadRol, newSquad2.numRows, newSquad2.numCols, newSquad2.customConfiguration);
                FindObjectOfType<SquadConfiguration>().currentSquadSelected = new SquadConfiguration.Squad(newSquad2.squadName, newSquad2.squadRol, newSquad2.numRows, newSquad2.numCols, newSquad2.customConfiguration);
                FindObjectOfType<BoogiesSpawner>().SpawnEnemySquad(spawnCommanderSquadPosition.position, spawnCommanderSquadPosition.rotation, FindObjectOfType<SquadConfiguration>().currentSquadSelected);
                BoogieWrestlerCommander[] commanders = FindObjectsOfType<BoogieWrestlerCommander>();
                foreach (BoogieWrestlerCommander c in commanders)
                {
                    if (c.team == TEAM.B)
                    {
                        initialEnemyCommander = c;
                        break;
                    }
                }

                multipathObstacle.GetComponentInChildren<MultipathController>().CloseAllDoors();

                FindObjectOfType<BoogiesSpawner>().ForceSpawnBoogies(BoogieType.Cleaner, cleanersSpawnPoint.position, BoogiesSpawner.CleanersSpawned);
                FindObjectOfType<BoogiesSpawner>().ForceSpawnBoogies(BoogieType.Explorer, explorersSpawnPoint.position, BoogiesSpawner.ExplorersSpawned);
                FindObjectOfType<BoogiesSpawner>().ForceSpawnBoogies(BoogieType.Collector, collectorsSpawnPoint.position, BoogiesSpawner.CollectorsSpawned);
                break;
            case GAME_STEP.Step1:
                anciantConversation.SetActive(true);
                FindObjectOfType<PlayerMovement>().enabled = false;
                FindObjectOfType<BoogiesSpawner>().GetComponent<Animator>().SetInteger("state", 0);
                break;
            case GAME_STEP.Step2:
                anciantConversation.SetActive(false);
                conversationWithAnciantFinished = true;
                FindObjectOfType<PlayerMovement>().enabled = true;
                break;
            case GAME_STEP.Step3:
                FindObjectOfType<TriggerOpenDoorCleaners>().GetComponent<SphereCollider>().enabled = true;
                allieCommander.wrestlersOffset = 2f;
                //STOP EPIC BATTLE MUSIC
                //SE ACTIVA LA UI PARA PODER DEVOLVER LOS CLEANERS
                break;
            case GAME_STEP.Step4:
                FindObjectOfType<PlayerMovement>()._agent.areaMask += 1 << NavMesh.GetAreaFromName("DebrisObstacle");
                for (int i = 0; i < giants.Length; i++)
                {
                    giants[i].gameObject.SetActive(true);
                }
                StartCoroutine(PositionateGiants());
                break;
            case GAME_STEP.Step5:
                //start conversation with giants
                giantsConversation.SetActive(true);
                FindObjectOfType<PlayerMovement>().enabled = false;
                FindObjectOfType<BoogiesSpawner>().GetComponent<Animator>().SetInteger("state", 0);
                break;
            case GAME_STEP.Step6:
                giantsConversation.SetActive(false);
                conversationWithGiantsFinished = true;
                FindObjectOfType<PlayerMovement>().enabled = true;
                //stop conversation with giants
                break;
            case GAME_STEP.Step7:
                aidilConversation.SetActive(true);
                FindObjectOfType<PlayerMovement>().enabled = false;
                FindObjectOfType<BoogiesSpawner>().GetComponent<Animator>().SetInteger("state", 0);
                break;
            case GAME_STEP.Step8:
                aidilConversation.SetActive(false);
                FindObjectOfType<PlayerMovement>().enabled = true;
                OpenCollectorsDoor();
                break;
            case GAME_STEP.Step9:
                portalCollectors.EnablePortal();
                for (int i = 0; i<distances.Length; i++)
                {
                    distances[i].gameObject.SetActive(true);
                }
                StartCoroutine(PositionateDistances());
                SpawnFinalEnemyWrestlers();
                ancient.isStatic = false;
                ancient.transform.position = lodwigPosition.position;
                ancient.transform.rotation = lodwigPosition.rotation;
                ancient.GetComponent<Animator>().SetInteger("state", 1);
                break;
            case GAME_STEP.Step10:
                distancesConversation.SetActive(true);
                FindObjectOfType<PlayerMovement>().enabled = false;
                FindObjectOfType<PlayerMovement>().GetComponent<Animator>().SetInteger("state", 0);
                break;
            case GAME_STEP.Step11:
                distancesConversation.SetActive(false);
                FindObjectOfType<PlayerMovement>().enabled = true;
                break;
            case GAME_STEP.Step12:
                StartCoroutine(LodwigDies());
                break;
            case GAME_STEP.Step13:
                lodwigConversation.SetActive(false);
                fadeBlackImage.DOFade(1f, 3f).OnComplete(() =>
                {
                    fadeBlackImage.GetComponentInChildren<TextMeshPro>().DOFade(1f, 3f).OnComplete(()=> 
                    {
                        //StartCoroutine(GameController.I.EndGame());
                    });
                });
                break;
            case GAME_STEP.Step14:
                //open final door and end game.
                break;
            case GAME_STEP.Step15:
                break;
        }

    }


}
