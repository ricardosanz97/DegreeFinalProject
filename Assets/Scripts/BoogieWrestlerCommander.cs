using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    [HideInInspector]public List<BoogieWrestler> squadWrestlers;
    [HideInInspector]public SquadConfiguration.Index leaderIndex;
    [HideInInspector]public SquadConfiguration.Index bodyIndex;
    public SquadConfiguration.Squad squadInfo;

    [HideInInspector]public List<SquadConfiguration.SQUAD_ROL> currentSquadList;

    [HideInInspector]public List<SquadConfiguration.Index> neededIndexs;

    public InteractableBody coveringBody;

    public SQUAD_STATE currentSquadState = SQUAD_STATE.CoveringPosition;

    public float wrestlersOffset = 1.8f;
    [HideInInspector]public bool selectingPosition = false;

    public void SetInitialPoint(Vector3 position)
    {
        initialPoint = position;
        _agent.SetDestination(initialPoint);
    }

    public void InteractableBodySelected(InteractableBody body)
    {
        coveringBody = body;

        SquadConfiguration.Index auxIndexs = new SquadConfiguration.Index(leaderIndex.i - bodyIndex.i, leaderIndex.j - bodyIndex.j);
       
        if (body.GetComponent<BoogieWrestler>())
        {
            body.GetComponent<BoogieWrestler>().lastPos = new SquadConfiguration.Index(body.GetComponent<BoogieWrestler>().indexs.i, body.GetComponent<BoogieWrestler>().indexs.j);
        }
        //this.ChangeIndexsRelativeToLeader();
        foreach (BoogieWrestler bw in squadWrestlers)
        {
            bw.leader = coveringBody.gameObject;
            bw.indexs.i += auxIndexs.i;
            bw.indexs.j += auxIndexs.j;
        }

        foreach (SquadConfiguration.Index i in neededIndexs)
        {
            i.i += auxIndexs.i;
            i.j += auxIndexs.j;
        }

        currentSquadState = SQUAD_STATE.CoveringBody;
        UIController.OnInteractableBodyPressed -= InteractableBodySelected;
        UIController.I.UIHideMouseSelector();
        UIController.I.selectingBodyToCover = false;
    }

    public void ChangeSquadFormation(SquadConfiguration.Squad newSquadInfo)
    {
        squadInfo = new SquadConfiguration.Squad(newSquadInfo.name, newSquadInfo.listFormation, newSquadInfo.squadCols, newSquadInfo.squadRows, newSquadInfo.customSquadConfiguration);
        currentSquadList = new List<SquadConfiguration.SQUAD_ROL>(squadInfo.listFormation);
        SquadConfiguration.SquadSlot[,] squadSlots = squadInfo.squad;
        leaderIndex = new SquadConfiguration.Index(newSquadInfo.leaderPosition.i, newSquadInfo.leaderPosition.j);
        List<BoogieWrestler> wrestlersLeft = new List<BoogieWrestler>(this.squadWrestlers);
        int counter = 0;
        for (int i = 0; i<squadInfo.squadRows; i++)
        {
            for (int j = 0; j<squadInfo.squadCols; j++)
            {
                BoogieWrestler bw = null;
                switch (squadSlots[i, j].rol)
                {
                    case SquadConfiguration.SQUAD_ROL.Distance:
                        bw = wrestlersLeft.Find((x) => x.wrestlerType == WRESTLER_TYPE.Distance);
                        break;
                    case SquadConfiguration.SQUAD_ROL.Close:
                        bw = wrestlersLeft.Find((x) => x.wrestlerType == WRESTLER_TYPE.Close);
                        break;
                    case SquadConfiguration.SQUAD_ROL.Giant:
                        bw = wrestlersLeft.Find((x) => x.wrestlerType == WRESTLER_TYPE.Giant);
                        break;
                    case SquadConfiguration.SQUAD_ROL.Commander:
                        bw = wrestlersLeft.Find((x) => x.wrestlerType == WRESTLER_TYPE.Commander);
                        bw.isLeaderPosition = true;
                        break;
                }
                if (bw != null)
                {
                    bw.indexs = new SquadConfiguration.Index(i, j);
                    bw.listPosition = counter;
                    wrestlersLeft.Remove(bw);
                    bw.Start();
                }
                counter++;
            }
        }
    }

    public void ResetDefaultFormation()
    {
        BoogieWrestler auxBw = null;
        if (coveringBody == null)
        {
            for (int i = 0; i < squadWrestlers.Count; i++)
            {
                BoogieWrestler bw = squadWrestlers[i];
                if (bw.indexs.i != bw.initialIndexs.i || bw.indexs.j != bw.initialIndexs.j)
                {
                    BoogieWrestler otherBw = squadWrestlers.Find((x) => x.indexs.i == bw.initialIndexs.i && x.indexs.j == bw.initialIndexs.j);
                    if (otherBw != null)
                    {
                        bw.ChangeSelectedWrestler(otherBw);
                    }
                    else
                    {
                        Debug.Log("Estamos aqui: " + bw.wrestlerType.ToString());
                        bw.indexs = new SquadConfiguration.Index(bw.initialIndexs.i, bw.initialIndexs.j);
                    }
                }
            }
        }
        else
        {
            //TODO: esto se podria cambiar por un ChangeFormation(commander.squadInfo);
            auxBw = squadWrestlers.Find((x) => x.isLeaderPosition);
            Debug.Log(auxBw.wrestlerType.ToString());
            this.UncoverBody();
            ResetDefaultFormation();
        }
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
        if (squadInfo.hasBody)
        {
            bodyIndex = new SquadConfiguration.Index(squadInfo.bodyPosition.i, squadInfo.bodyPosition.j);
        }
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
            SquadConfiguration.Index auxIndexs = new SquadConfiguration.Index(this.indexs.i, this.indexs.j);
            
            if (!commander.GetComponent<BoogieWrestler>().isLeaderPosition)
            {
                BoogieWrestler boogieOnLeaderPos = commander.squadWrestlers.Find((x) => x.isLeaderPosition);
                Debug.Log(boogieOnLeaderPos.wrestlerType.ToString());
                Debug.Log("pasa por aqui");
                this.commander.indexs = new SquadConfiguration.Index(boogieOnLeaderPos.indexs.i, boogieOnLeaderPos.indexs.j);
                boogieOnLeaderPos.indexs = new SquadConfiguration.Index(auxIndexs.i, auxIndexs.j);
                boogieOnLeaderPos.isLeaderPosition = false;
                commander.isLeaderPosition = true;
            }

            commander.AssignAsLeader();

            if (coveringBody != null)
            {
                if (coveringBody.GetComponent<BoogieWrestler>())
                {
                    return;
                }
                coveringBody = null;
            }
        }

        if (FindObjectOfType<xMarkerBehavior>() != null)
        {
            Destroy(FindObjectOfType<xMarkerBehavior>().gameObject);
        }
        this.currentSquadState = SQUAD_STATE.Moving;
        UIController.I.UIShowXMarker(clickPosition);
        randomPoint = clickPosition;
        leader.GetComponent<BoogieWrestler>()._agent.SetDestination(randomPoint);
    }

    public void UncoverBody()
    {
        SquadConfiguration.Index auxIndexs = new SquadConfiguration.Index(this.indexs.i, this.indexs.j);
        if (this.commander.coveringBody.GetComponent<BoogieWrestler>() != null)
        {
            BoogieWrestler bw = this.commander.coveringBody.GetComponent<BoogieWrestler>();
            bw.indexs = new SquadConfiguration.Index(bw.lastPos.i, bw.lastPos.j);
            bw.lastPos = new SquadConfiguration.Index(0, 0);
        }

        if (!commander.GetComponent<BoogieWrestler>().isLeaderPosition)
        {
            BoogieWrestler boogieOnLeaderPos = commander.squadWrestlers.Find((x) => x.isLeaderPosition);
            /*
            Debug.Log(boogieOnLeaderPos.wrestlerType.ToString());
            Debug.Log("pasa por aqui");
            this.commander.indexs = new SquadConfiguration.Index(boogieOnLeaderPos.indexs.i, boogieOnLeaderPos.indexs.j);
            boogieOnLeaderPos.indexs = new SquadConfiguration.Index(auxIndexs.i, auxIndexs.j);
            boogieOnLeaderPos.isLeaderPosition = false;
            commander.isLeaderPosition = true;
            */
            boogieOnLeaderPos.ChangeSelectedWrestler(this);
        }

        this.AssignAsLeader();
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

    public void RemoveWrestlerSquad(BoogieWrestler bw)
    {
        squadWrestlers.Remove(bw);
        closeWrestlers.Clear();
        distanceWrestlers.Clear();
        giantWrestlers.Clear();
        GetSquadInformation();
    }
}