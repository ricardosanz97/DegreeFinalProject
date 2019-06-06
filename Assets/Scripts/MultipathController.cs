using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class MultipathController : MonoBehaviour, ISaveable
{
    public int minPaths = 5;
    public int maxPaths = 9;
    public int numPaths;
    public PathBehavior[] Paths;
    public PathBehavior firstPath;
    public int currentPlayerIndex = -1;

    public Vector3 distanceTraveled;
    public Vector3 initialPosition;
    public ClueBehavior[] clues;
    public string combination;

    public Transform leftDoorClosed;
    public Transform rightDoorClosed;
    public Transform leftDoorOpened;
    public Transform rightDoorOpened;

    public bool playerCompleteMultipath;
    private void Awake()
    {
        //numPaths = Random.Range(minPaths, maxPaths + 1);
        numPaths = this.transform.childCount;
        AssignPaths();
    }

    private void Start()
    {
        if (FindObjectOfType<LevelManager>() != null && LevelManager.I.limitedVersion)
        {
            for (int i = 0; i < 2; i++)
            {
                int clue = int.Parse(combination[i].ToString());
                GameObject clueGO = Instantiate(Resources.Load<GameObject>("Prefabs/Clues/" + clue + "Clue"),new Vector3(100,100,100), Quaternion.identity);
                Vector3 position = new Vector3(Paths[0].begin.transform.position.x, 0.4f + (i * clueGO.GetComponent<Collider>().bounds.size.y), Paths[0].begin.transform.position.z);
                clueGO.transform.position = position;
                clues[i] = clueGO.GetComponent<ClueBehavior>();
                clueGO.GetComponent<ClueBehavior>().pathIndex = i;
                clueGO.GetComponent<ClueBehavior>().corridorIndex = clue;
                clueGO.GetComponent<ClueBehavior>().placed = true;
            }
        }
    }

    private void OnEnable()
    {
        SaverManager.OnSaveData += Save;
        SaverManager.OnLoadData += Load;
    }

    private void AssignPaths()
    {
        Paths = new PathBehavior[numPaths];
        for (int i = 0; i<numPaths; i++)
        {
            PathBehavior pb = this.transform.GetChild(i).GetComponentInChildren<PathBehavior>();
            pb.PathIndex = i;
            Paths[i] = pb;
            Paths[i].AssignCorridorIndexs();
            if (i == 0)
            {
                pb.FirstPath = true;
                firstPath = pb;
                GetComponentInParent<MultipathObstacle>()._col = firstPath.begin.GetComponent<Collider>();
            }
        }
        clues = new ClueBehavior[Paths.Length];

        for (int i = 0; i<Paths.Length - 1; i++)
        {
            Paths[i].nextInitCorridor = Paths[i + 1].transform.parent.GetComponentInChildren<CorridorBegin>().gameObject;
        }

        for (int i = 1; i<Paths.Length; i++)
        {
            Paths[i].LastCorridorCorrectIndex = Paths[i - 1].CorridorCorrectIndex;
        }

        StringBuilder sb = new StringBuilder();
        for (int i = 0; i<Paths.Length; i++)
        {
            sb.Append(Paths[i].CorridorCorrectIndex.ToString());
        }
        combination = sb.ToString();

        //Debug.Log("limitedVersion = " + LevelManager.I.limitedVersion);
    }

    public void CloseAllDoors()
    {
        /*
        Vector3 rightClosedPosition = new Vector3(0.23f, -0 - 189, 0.316f);
        Vector3 rightClosedRotation = Vector3.zero;

        Vector3 leftClosedPosition = new Vector3(0.303f, -0.544f, 0.002f);
        Vector3 leftClosedRotation = new Vector3(0, -29.15f, 0);

        Vector3 rightOpenedPosition = new Vector3(0.23f, -0.189f, 0.316f);
        Vector3 rightOpenedRotation = new Vector3(0, 100.326f, 0);

        Vector3 leftOpenedPosition = new Vector3(0.169f, -0.54f, 0.018f);
        Vector3 leftOpenedRotation = new Vector3(0, -133.76f, 0);
        */
        for (int i = 0; i<this.transform.childCount; i++)
        {
            for (int j = 0; j<this.transform.GetChild(i).Find("Corridors").childCount; j++)
            {
                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelBegin").transform.Find("right").transform.localPosition = rightDoorClosed.position;
                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelBegin").transform.Find("right").transform.localRotation = rightDoorClosed.rotation;

                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelBegin").transform.Find("right").transform.localPosition = rightDoorClosed.position;
                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelBegin").transform.Find("right").transform.rotation = rightDoorClosed.rotation;

                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelBegin").transform.Find("left").transform.localPosition = leftDoorClosed.position;
                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelBegin").transform.Find("left").transform.rotation = leftDoorClosed.rotation;

                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelBegin").transform.Find("left").transform.localPosition = leftDoorClosed.position;
                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelBegin").transform.Find("left").transform.rotation = leftDoorClosed.rotation;

                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelEnd").transform.Find("right").transform.localPosition = rightDoorClosed.position;
                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelEnd").transform.Find("right").transform.rotation = rightDoorClosed.rotation;

                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelEnd").transform.Find("right").transform.localPosition = rightDoorClosed.position;
                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelEnd").transform.Find("right").transform.rotation = rightDoorClosed.rotation;

                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelEnd").transform.Find("left").transform.localPosition = leftDoorClosed.position;
                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelEnd").transform.Find("left").transform.rotation = leftDoorClosed.rotation;

                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelEnd").transform.Find("left").transform.localPosition = leftDoorClosed.position;
                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelEnd").transform.Find("left").transform.rotation = leftDoorClosed.rotation;
            }
        }
    }

    public void OpenAllBeginDoors()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            for (int j = 0; j < this.transform.GetChild(i).Find("Corridors").childCount; j++)
            {
                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelBegin").transform.Find("pCube4").transform.localPosition = rightDoorOpened.position;
                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelBegin").transform.Find("pCube4").transform.localRotation = rightDoorOpened.rotation;

                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelBegin").transform.Find("pCube4").transform.localPosition = rightDoorOpened.position;
                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelBegin").transform.Find("pCube4").transform.rotation = rightDoorOpened.rotation;

                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelBegin").transform.Find("polySurface4").transform.localPosition = leftDoorOpened.position;
                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelBegin").transform.Find("polySurface4").transform.rotation = leftDoorOpened.rotation;

                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelBegin").transform.Find("polySurface4").transform.localPosition = leftDoorOpened.position;
                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelBegin").transform.Find("polySurface4").transform.rotation = leftDoorOpened.rotation;
            }
        }
    }

    public void OpenAllEndDoors()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            for (int j = 0; j < this.transform.GetChild(i).Find("Corridors").childCount; j++)
            {
                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelEnd").transform.Find("pCube4").transform.localPosition = rightDoorOpened.position;
                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelEnd").transform.Find("pCube4").transform.localRotation = rightDoorOpened.rotation;

                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelEnd").transform.Find("pCube4").transform.localPosition = rightDoorOpened.position;
                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelEnd").transform.Find("pCube4").transform.rotation = rightDoorOpened.rotation;

                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelEnd").transform.Find("polySurface4").transform.localPosition = leftDoorOpened.position;
                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelEnd").transform.Find("polySurface4").transform.rotation = leftDoorOpened.rotation;

                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelEnd").transform.Find("polySurface4").transform.localPosition = leftDoorOpened.position;
                this.transform.GetChild(i).Find("Corridors").GetChild(j).Find("DoorModelEnd").transform.Find("polySurface4").transform.rotation = leftDoorOpened.rotation;
            }
        }
    }

    public void Save()
    {
        string key = "MultipathObstacle" + GetComponentInParent<MultipathObstacle>().uniqueId + "Clue";
        int counter = 0;
        for (int i = 0; i<clues.Length; i++)
        {
            if (clues[i] != null)
            {
                string key2 = key + i;
                SaverManager.I.saveData.Add(key2 + "Position", clues[i].transform.position);
                SaverManager.I.saveData.Add(key2 + "Rotation", clues[i].transform.rotation);
                SaverManager.I.saveData.Add(key2 + "CorridorIndex", clues[i].corridorIndex);
                SaverManager.I.saveData.Add(key2 + "PathIndex", clues[i].pathIndex);
                SaverManager.I.saveData.Add(key2 + "PlacedBy", clues[i].placedBy);
                SaverManager.I.saveData.Add(key2 + "Placed", clues[i].placed);
                counter++;
            }
        }
        SaverManager.I.saveData.Add(key + "CluesAmount", counter);
        SaverManager.I.saveData.Add(key + "Clues", clues);
    }

    public void Load()
    {
        string key = "MultipathObstacle" + GetComponentInParent<MultipathObstacle>().uniqueId + "Clue";

        int counter = SaverManager.I.saveData[key + "CluesAmount"];

        int numListClues = clues.Length;
        for (int i = 0; i<numListClues; i++)
        {
            if (i >= counter)
            {
                if (clues[i] != null)
                {
                    Destroy(clues[i].gameObject);
                    clues[i] = null;
                }
            }
        }
        clues = SaverManager.I.saveData[key + "Clues"];
        for (int i = 0; i<clues.Length; i++)
        {
            try
            {
                if (clues[i] != null)
                {
                    clues[i].transform.position = SaverManager.I.saveData[key + i + "Position"];
                    clues[i].transform.rotation = SaverManager.I.saveData[key + i + "Rotation"];
                    clues[i].corridorIndex = SaverManager.I.saveData[key + i + "CorridorIndex"];
                    clues[i].pathIndex = SaverManager.I.saveData[key + i + "PathIndex"];
                    clues[i].placedBy = SaverManager.I.saveData[key + i + "PlacedBy"];
                    clues[i].placed = SaverManager.I.saveData[key + i + "Placed"];
                }
            }

            catch
            {
                Destroy(clues[i].gameObject);
            }
        }

    }
}
