using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class MultipathController : MonoBehaviour
{
    public int minPaths = 5;
    public int maxPaths = 9;
    public int numPaths;
    public PathBehavior[] Paths;
    public PathBehavior firstPath;
    public int currentPlayerIndex = -1;

    public Vector3 distanceTraveled;
    public Vector3 initialPosition;
    public string combination;
    private void Awake()
    {
        //numPaths = Random.Range(minPaths, maxPaths + 1);
        numPaths = this.transform.childCount;
        AssignPaths();
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
                pb.FirstCorridor = true;
                firstPath = pb;
                GetComponentInParent<MultipathObstacle>()._col = firstPath.begin.GetComponent<Collider>();
            }
        }

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
    }
}
