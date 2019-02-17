using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoogieWrestlerCommander : BoogieWrestler
{
    public List<BoogieWrestlerDistance> distanceWrestlers;
    public List<BoogieWrestlerClose> closeWrestlers;
    public List<BoogieWrestlerGiant> giantWrestlers;
    public float distanceBetweenUs = 1.8f;

    public void SetInitialPoint(Vector3 position)
    {
        initialPoint = position;
        _agent.SetDestination(initialPoint);
    }

    public override void Start()
    {
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
}
