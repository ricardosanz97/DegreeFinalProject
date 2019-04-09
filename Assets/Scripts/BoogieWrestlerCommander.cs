using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class BoogieWrestlerCommander : BoogieWrestler
{
    [HideInInspector]public List<BoogieWrestlerDistance> distanceWrestlers;
    [HideInInspector]public List<BoogieWrestlerClose> closeWrestlers;
    [HideInInspector]public List<BoogieWrestlerGiant> giantWrestlers;
    public List<BoogieWrestler> squadWrestlers;
    [HideInInspector]public SquadConfiguration.Index leaderIndex;
    [HideInInspector]public SquadConfiguration.Index bodyIndex;
    /*[HideInInspector]*/public SquadConfiguration.Squad squadInfo;

    public List<BoogieWrestler> needHelpBoogies = new List<BoogieWrestler>();
    private bool CheckCoveringEnabled = false;

    [HideInInspector]public List<SquadConfiguration.SQUAD_ROL> currentSquadList;

    [HideInInspector]public List<SquadConfiguration.Index> neededIndexs;

    public InteractableBody coveringBody;

    public float wrestlersOffset = 1.8f;
    [HideInInspector]public bool selectingPosition = false;

    private bool coroutineStarted = false;
    public float timeCoverAgain;

    public override void SetConfiguration()
    {
        base.SetConfiguration();
        timeCoverAgain = Random.Range(minTimeCoverAgain, maxTimeCoverAgain);
    }

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
        Debug.Log("change state to squadCovering");
        foreach (BoogieWrestler bw in squadWrestlers)
        {
            if (bw.wrestlerSelected != null)
            {
                bw.wrestlerSelected = null;
            }
            bw.visibleTargets.Clear();
            bw._agent.stoppingDistance = 0f;

            bw.currentState = STATE.OnSquadCovering;
            bw.leader = coveringBody.gameObject;
            bw.indexs.i += auxIndexs.i;
            bw.indexs.j += auxIndexs.j;
        }

        foreach (SquadConfiguration.Index i in neededIndexs)
        {
            i.i += auxIndexs.i;
            i.j += auxIndexs.j;
        }

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
                //TODO: no mostrar este botón (follow player)
            },
            () =>
            {
                //TODO: no mostrar este botón (break formation)
            }
            ,
            ChangePosition,
            AssignAsLeader
            );
        }
    }

    public override void Start()
    {
        this.transform.rotation = FindObjectOfType<BoogiesSpawner>().transform.rotation;
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
            Debug.Log("LEADER = COMMANDER");
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
        UIController.I.UIShowXMarker(clickPosition);
        randomPoint = clickPosition;
        foreach (BoogieWrestler bw in squadWrestlers)
        {
            if (bw.commander.coveringBody != null)
            {
                Debug.Log("ON SQUAD MOVING COVERING");
                bw.currentState = STATE.OnSquadCoveringMoving;
            }
            else
            {
                Debug.Log("ON SQUAD MOVING");
                bw.currentState = STATE.OnSquadMoving;
            }
          
            bw.MoveHere(randomPoint);
        }
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
            boogieOnLeaderPos.ChangeSelectedWrestler(this);
        }

        this.AssignAsLeader();

        foreach (BoogieWrestler bw in squadWrestlers)
        {
            bw.currentState = STATE.OnSquadObserving;
        }
    }

    public void INeedHelp(BoogieWrestler bw)
    {
        if (needHelpBoogies.Contains(bw))
        {
            return;
        }
        Debug.Log("lets add to the needed help list " + bw.wrestlerType);
        needHelpBoogies.Add(bw);
        if (!CheckCoveringEnabled)
        {
            StartCoroutine(CoverLowWrestler());
            CheckCoveringEnabled = true;
        }
        
    }

    private BoogieWrestler GetLowestBoogie()
    {
        float lowest = Mathf.Infinity;
        BoogieWrestler lowestWrestler = null;
        for (int i = 0; i<needHelpBoogies.Count; i++)
        {
            if ((needHelpBoogies[i].health/needHelpBoogies[i].initialHealth) < lowest)
            {
                lowest = needHelpBoogies[i].health/needHelpBoogies[i].initialHealth;
                lowestWrestler = needHelpBoogies[i];
            }
        }
        return lowestWrestler;
    }

    IEnumerator CoverLowWrestler()
    {
        while (true)
        {
            Debug.Log("Getting lowest boogie. ");
            BoogieWrestler needsMoreHelp = GetLowestBoogie();
            if (needsMoreHelp != null)
            {
                foreach (BoogieWrestler bw in squadWrestlers)
                {
                    if (bw.currentState != STATE.OnSquadCoveringMoving && bw.currentState != STATE.OnSquadCovering)
                    {
                        bw.currentState = STATE.OnSquadCovering;
                    }
                }
                Debug.Log("Lowest boogie is " + needsMoreHelp.wrestlerType.ToString());
                if (this.coveringBody != null)
                {
                    this.UncoverBody();
                }
               
                InteractableBodySelected(needsMoreHelp);
            }
            yield return new WaitForSeconds(timeCoverAgain);
        }
    }

    public override void Update()
    {
        base.Update();

        this.visibleTargets.RemoveAll((x) => x == null || x.GetComponent<BoogieWrestler>() == null);

        if ((currentState == STATE.OnSquadMoving || currentState == STATE.OnSquadCoveringMoving) && Vector3.Distance(this.transform.position, randomPoint) <= 0.5f)
        {
            if (coveringBody != null)
            {
                Debug.Log("change state to OnSquadCovering");
                foreach (BoogieWrestler bw in squadWrestlers)
                {
                    bw.currentState = STATE.OnSquadCovering;
                }
            }
            else
            {
                foreach (BoogieWrestler bw in squadWrestlers)
                {
                    bw.currentState = STATE.OnSquadObserving;
                }
            }
            if (FindObjectOfType <xMarkerBehavior>() != null)
            {
                Destroy(FindObjectOfType<xMarkerBehavior>().gameObject);
            }
        }

        if (!coroutineStarted && currentState == STATE.OnSquadAttacking)
        {
            StartCoroutine(CheckForNewEnemies());
            coroutineStarted = true;
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

    public BoogieWrestler GetHealthestWrestler()
    {
        float maxHealth = 0;
        BoogieWrestler healthest = null;
        for (int i = 0; i<squadWrestlers.Count; i++)
        {
            if (squadWrestlers[i].health > maxHealth)
            {
                maxHealth = squadWrestlers[i].health;
                healthest = squadWrestlers[i];
            }
        }
        return healthest;
    }

    IEnumerator CheckForNewEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (currentState == STATE.OnSquadAttacking)
            {
                if (this.visibleTargets.Count > 0)
                {
                    foreach (Transform t in commander.visibleTargets)
                    {
                        foreach (BoogieWrestler bw in t.GetComponent<BoogieWrestler>().commander.squadWrestlers)
                        {
                            if (!this.GetComponentInParent<SquadTeam>().enemyWrestlers.Contains(bw))
                            {
                                this.GetComponentInParent<SquadTeam>().enemyWrestlers.Add(bw);
                            }
                        }
                    }
                }
            }
            yield return null;
        }
    }
}