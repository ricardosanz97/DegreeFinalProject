using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public enum SQUAD_STATE
{
    Moving,
    CoveringPosition,
    CoveringPlayer,
    CoveringBody
}

public class BoogieWrestlerCommander : BoogieWrestler
{
    public List<BoogieWrestlerDistance> distanceWrestlers;
    public List<BoogieWrestlerClose> closeWrestlers;
    public List<BoogieWrestlerGiant> giantWrestlers;
    public List<BoogieWrestler> squadWrestlers;
    public SquadConfiguration.Index leaderIndex;

    public List<SquadConfiguration.SQUAD_ROL> currentSquadList;

    public List<SquadConfiguration.Index> neededIndexs;

    public InteractableBody coveringBody;

    public SQUAD_STATE currentSquadState = SQUAD_STATE.CoveringPosition;

    //public int formation = 0;

    public bool hasPlayer = false;

    public float distanceBetweenUs = 1.8f;
    public bool selectingPosition = false;

    public void SetInitialPoint(Vector3 position)
    {
        initialPoint = position;
        _agent.SetDestination(initialPoint);
    }

    public void InteractableBodySelected(InteractableBody body)
    {
        coveringBody = body;
        currentSquadState = SQUAD_STATE.CoveringBody;
        ChangeSquadFormation(SquadConfiguration.SQUAD_FORMATION.CoverBody);
        UIController.OnInteractableBodyPressed -= InteractableBodySelected;
        UIController.I.UIHideMouseSelector();
        UIController.I.selectingBodyToCover = false;
    }

    public override void WrestlerClicked(int clickButton)
    {
        base.WrestlerClicked(clickButton);
        if (clickButton == 1)
        {
            Debug.Log("hola soy " + gameObject.name);
            UISquadIndividualOptionsController.Create(
            () =>
            {
                //FollowPlayer();
            },
            () =>
            {
                //BreakFormation();
            }
            ,
            () =>
            {
                ChangePosition();
            },
            () =>
            {
                AssignAsLeader();
            }
            );
        }
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
        squadWrestlers = squadParent.gameObject.GetComponentsInChildren<BoogieWrestler>().ToList();
    }

    public void MoveToPosition(Vector3 clickPosition)
    {
        if (leader.GetComponent<BoogieWrestler>() == null)
        {
            leader = commander.gameObject;
            foreach (BoogieWrestler bw in squadWrestlers)
            {
                bw.leader = commander.gameObject;
            }
        }
        //UIController.OnMoveSquadPositionSelected -= MoveToPosition;
        if (FindObjectOfType<xMarkerBehavior>() != null)
        {
            Destroy(FindObjectOfType<xMarkerBehavior>().gameObject);
        }
        this.currentSquadState = SQUAD_STATE.Moving;
        UIController.I.UIShowXMarker(clickPosition);
        randomPoint = clickPosition;
        leader.GetComponent<BoogieWrestler>()._agent.SetDestination(randomPoint);
        Debug.Log("Clicking on " + clickPosition);
    }

    public override void Update()
    {
        base.Update();
        if (currentSquadState == SQUAD_STATE.Moving && Vector3.Distance(this.transform.position, randomPoint) <= 0.5f)
        {
            this.currentSquadState = SQUAD_STATE.CoveringPosition;
            if (FindObjectOfType <xMarkerBehavior>() != null)
            {
                Destroy(FindObjectOfType<xMarkerBehavior>().gameObject);
            }
        }
    }

    public void ChangeSquadFormation(SquadConfiguration.SQUAD_FORMATION formation)
    {
        SquadConfiguration.Squad newSquad = new SquadConfiguration.Squad(SquadConfiguration.SQUAD_LEVEL.First, formation);
        List<BoogieWrestler> wrestlers = new List<BoogieWrestler>();

        this.currentSquadList = newSquad.listFormation;

        int posI = newSquad.leaderPosition.i;
        int posJ = newSquad.leaderPosition.j;

        leaderIndex = new SquadConfiguration.Index(posI, posJ);
        this.hasPlayer = newSquad.hasPlayer;

        if (this.hasPlayer)
        {
            leader = FindObjectOfType<BoogiesSpawner>().gameObject;
        }
        else if (newSquad.hasBody)
        {
            leader = this.coveringBody.gameObject;
        }
        else
        {
            leader = this.gameObject;
        }

        ChangeIndexsRelativeToLeader();
        TakeInitialPosition();

        switch (formation)
        {
            case SquadConfiguration.SQUAD_FORMATION.Contention:
                //this.formation = 1;
                this.currentSquadState = SQUAD_STATE.CoveringPosition;
                break;
            case SquadConfiguration.SQUAD_FORMATION.Penetration:
                //this.formation = 2;
                this.currentSquadState = SQUAD_STATE.CoveringPosition;
                break;
            case SquadConfiguration.SQUAD_FORMATION.AroundPlayer:
                this.currentSquadState = SQUAD_STATE.CoveringPlayer;
                //this.formation = 3;
                break;
            case SquadConfiguration.SQUAD_FORMATION.CoverBody:
                this.currentSquadState = SQUAD_STATE.CoveringBody;
                //this.formation = 4;
                break;
        }

        SquadConfiguration.SquadSlot[,] squadSlots = newSquad.squad;
        int counter = 0;
        for (int i = 0; i<newSquad.squadRows; i++)
        {
            for (int j = 0; j<newSquad.squadCols; j++)
            {
                switch (squadSlots[i, j].rol)
                {
                    case SquadConfiguration.SQUAD_ROL.Distance:
                        for (int k = 0; k<distanceWrestlers.Count; k++)
                        {
                            if (!wrestlers.Contains(distanceWrestlers[k]))
                            {
                                wrestlers.Add(distanceWrestlers[k]);
                                distanceWrestlers[k].indexs = new SquadConfiguration.Index(squadSlots[i, j].position.i, squadSlots[i,j].position.j);
                                distanceWrestlers[k].listPosition = counter;
                                break;
                            }
                        }
                        break;
                    case SquadConfiguration.SQUAD_ROL.Close:
                        for (int k = 0; k<closeWrestlers.Count; k++)
                        {
                            if (!wrestlers.Contains(closeWrestlers[k]))
                            {
                                wrestlers.Add(closeWrestlers[k]);
                                closeWrestlers[k].indexs = new SquadConfiguration.Index(squadSlots[i, j].position.i, squadSlots[i, j].position.j);
                                closeWrestlers[k].listPosition = counter;
                                break;
                            }
                        }
                        break;
                    case SquadConfiguration.SQUAD_ROL.Giant:
                        for (int k = 0; k<giantWrestlers.Count; k++)
                        {
                            if (!wrestlers.Contains(giantWrestlers[k]))
                            {
                                wrestlers.Add(giantWrestlers[k]);
                                giantWrestlers[k].indexs = new SquadConfiguration.Index(squadSlots[i, j].position.i, squadSlots[i, j].position.j);
                                giantWrestlers[k].listPosition = counter;
                                break;
                            }
                        }
                        break;

                    case SquadConfiguration.SQUAD_ROL.Commander:
                        this.indexs = new SquadConfiguration.Index(squadSlots[i, j].position.i, squadSlots[i, j].position.j);
                        this.listPosition = counter;
                        Debug.Log(indexs.i + ", " + indexs.j);
                        break;
                }
                counter++;
            }
        }

        foreach (BoogieWrestler bw in wrestlers)
        {
            bw.commander = this;
            if (hasPlayer)
            {
                bw.leader = FindObjectOfType<BoogiesSpawner>().gameObject;
            }
            else if (newSquad.hasBody)
            {
                bw.leader = commander.coveringBody.gameObject;
            }
            else
            {
                bw.leader = this.gameObject;
            }
            bw.ChangeIndexsRelativeToLeader();
            bw.TakeInitialPosition();
        }
        this.ChangeIndexsRelativeToLeader();
        this.TakeInitialPosition();
    }

    public void RemoveWrestlerSquad(BoogieWrestler bw)
    {
        squadWrestlers.Remove(bw);
        closeWrestlers.Clear();
        distanceWrestlers.Clear();
        giantWrestlers.Clear();
        GetSquadInformation();
    }
}