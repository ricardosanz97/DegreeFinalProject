using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class BoogieWrestlerCommander : BoogieWrestler
{
    public List<BoogieWrestlerDistance> distanceWrestlers;
    public List<BoogieWrestlerClose> closeWrestlers;
    public List<BoogieWrestlerGiant> giantWrestlers;
    public SquadConfiguration.Index leaderIndex;

    public bool hasPlayer = false;

    public float distanceBetweenUs = 1.8f;
    public bool selectingPosition = false;

    public void SetInitialPoint(Vector3 position)
    {
        initialPoint = position;
        _agent.SetDestination(initialPoint);
    }

    public override void WrestlerClicked(int clickButton)
    {
        Debug.Log("hola soy " + gameObject.name);
        base.WrestlerClicked(clickButton);
    }

    public override void Start()
    {
        Debug.Log("LEADER INDEXS = " + leaderIndex.i + ", " + leaderIndex.j);
        Debug.Log("MY INDEXS = " + indexs.i + ", " + indexs.j);
        base.Start();
        GetSquadInformation();
    }

    private void GetSquadInformation()
    {
        Transform squadParent = this.transform.parent;
        distanceWrestlers = squadParent.gameObject.GetComponentsInChildren<BoogieWrestlerDistance>().ToList();
        closeWrestlers = squadParent.gameObject.GetComponentsInChildren<BoogieWrestlerClose>().ToList();
        giantWrestlers = squadParent.gameObject.GetComponentsInChildren<BoogieWrestlerGiant>().ToList();
    }

    public void MoveToPosition(Vector3 clickPosition)
    {
        Debug.Log("Clicking on " + clickPosition);
    }
}
