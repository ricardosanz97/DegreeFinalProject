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

    public List<SquadConfiguration.SQUAD_ROL> currentSquadList;

    public List<SquadConfiguration.Index> neededIndexs;

    public InteractableBody coveringBody;

    public float wrestlersOffset = 1.8f;
    [HideInInspector]public bool selectingPosition = false;

    private bool coroutineStarted = false;
    public float timeCoverAgain;

    public BoogieWrestlerCommander otherCommander = null;

    public override void SetConfiguration()
    {
        base.SetConfiguration();
        timeCoverAgain = Random.Range(minTimeCoverAgain, maxTimeCoverAgain);
        wrestlersOffset = Random.Range(minOffset, maxOffset);
    }

    public void SetInitialPoint(Vector3 position)
    {
        initialPoint = position;
        _agent.SetDestination(initialPoint);
    }

    public void InteractableBodySelected(InteractableBody body)
    {
        coveringBody = body;

        SquadConfiguration.Index auxIndexs = 
            new SquadConfiguration.Index(leaderIndex.i - bodyIndex.i, leaderIndex.j - bodyIndex.j);
       
        if (body.GetComponent<BoogieWrestler>())
        {
            body.GetComponent<BoogieWrestler>().lastPos = 
                new SquadConfiguration.Index(body.GetComponent<BoogieWrestler>().indexs.i, 
                                             body.GetComponent<BoogieWrestler>().indexs.j);
        }

        foreach (BoogieWrestler bw in squadWrestlers)
        {
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
        squadInfo = new SquadConfiguration.Squad(newSquadInfo.name, newSquadInfo.listFormation, 
            newSquadInfo.squadCols, newSquadInfo.squadRows, newSquadInfo.customSquadConfiguration);
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
            SquadConfiguration.SQUAD_ROL myRol = this.commander.currentSquadList[this.listPosition];
            SquadConfiguration.SQUAD_ROL hisRol = leader.GetComponent<BoogieWrestler>().commander.currentSquadList[leader.GetComponent<BoogieWrestler>().listPosition];
            this.commander.currentSquadList[this.listPosition] = hisRol;
            leader.GetComponent<BoogieWrestler>().commander.currentSquadList[leader.GetComponent<BoogieWrestler>().listPosition] = myRol;

            int auxListPos = this.listPosition;
            this.listPosition = leader.GetComponent<BoogieWrestler>().listPosition;
            leader.GetComponent<BoogieWrestler>().listPosition = auxListPos;

            for (int i = 0; i<squadWrestlers.Count; i++)
            {
                BoogieWrestler bw = squadWrestlers[i];
                SquadConfiguration.Index auxIndex = new SquadConfiguration.Index(bw.initialIndexs.i - commander.leaderIndex.i, bw.initialIndexs.j - commander.leaderIndex.j);
                BoogieWrestler otherBw = commander.squadWrestlers.Find((x) => x.indexs.i == auxIndex.i && x.indexs.j == auxIndex.j);
                if (otherBw != null && otherBw != bw)
                {
                    Debug.Log("cambiamos a " + bw.wrestlerType + " por " + otherBw.wrestlerType);
                    bw.ChangeSelectedWrestler(otherBw);
                }
                
            }

            for (int i = 0; i < squadWrestlers.Count; i++)
            {
                BoogieWrestler bw = squadWrestlers[i];
                bw.indexs = new SquadConfiguration.Index(bw.initialIndexs.i, bw.initialIndexs.j);
                bw.leader = this.gameObject;
                bw.ChangeIndexsRelativeToLeader();
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
        //this.transform.rotation = FindObjectOfType<BoogiesSpawner>().transform.rotation;
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
        if (this.currentState == STATE.OnSquadAttacking)
        {
            foreach (BoogieWrestler bw in squadWrestlers)
            {
                if (bw.targetSelected != null)
                {
                    if (bw.wrestlersAttackingMe.Contains(bw))
                    {
                        bw.wrestlersAttackingMe.Remove(bw);
                    }
                    bw.targetSelected = null;
                }

                bw._agent.stoppingDistance = 0f;
                bw.currentState = STATE.OnSquadMoving;
                //bw.StopCoroutines();
            }
        }
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
            if (bw.commander != null && bw.commander.coveringBody != null)
            {
                bw.currentState = STATE.OnSquadCoveringMoving;
            }
            else
            {
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
            if (this.GetComponentInParent<SquadTeam>().enemyWrestlers.Count > 0)
            {
                bw.currentState = STATE.OnSquadAttacking;
            }
            else
            {
                bw.currentState = STATE.OnSquadObserving;
            }
        }
    }

    public void INeedHelp(BoogieWrestler bw)
    {
        if (needHelpBoogies.Contains(bw))
        {
            return;
        }
        needHelpBoogies.Add(bw);
        if (!CheckCoveringEnabled)
        {
            if (GetLowestBoogie() == this) return;
            else StartCoroutine(CoverLowWrestler());
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
            if (currentState == STATE.OnSquadMoving) { yield return null; }
            BoogieWrestler needsMoreHelp = GetLowestBoogie();
            if (needsMoreHelp != null)
            {
                if (needsMoreHelp.wrestlersAttackingMe.Count > 0)
                {
                    foreach (BoogieWrestler bw in squadWrestlers)
                    {
                        if (bw.gameObject.activeInHierarchy)
                        {
                            if (bw.targetSelected != null)
                            {
                                if (bw.targetSelected.GetComponent<BoogieWrestler>() && bw.targetSelected.GetComponent<BoogieWrestler>().wrestlersAttackingMe.Contains(bw))
                                {
                                    bw.targetSelected.GetComponent<BoogieWrestler>().wrestlersAttackingMe.Remove(bw);
                                }
                                bw.targetSelected = null;
                            }

                            if (bw.helpingBoogie != null)
                            {
                                bw.helpingBoogie = null;
                            }

                            bw.EnemySelected(needsMoreHelp.wrestlersAttackingMe[Random.Range(0, needsMoreHelp.wrestlersAttackingMe.Count)]);
                            bw.helpingBoogie = needsMoreHelp;
                        }
                    }
                }    
            }
            yield return new WaitForSeconds(timeCoverAgain);
        }
    }

    public override void Update()
    {
        base.Update();

        if ((currentState == STATE.OnSquadMoving || currentState == STATE.OnSquadCoveringMoving) && Vector3.Distance(this.transform.position, randomPoint) <= 0.5f)
        {
            if (coveringBody != null)
            {
                //Debug.Log("change state to OnSquadCovering");
                foreach (BoogieWrestler bw in squadWrestlers)
                {
                    bw.currentState = STATE.OnSquadCovering;
                }
            }
            else
            {
                foreach (BoogieWrestler bw in squadWrestlers)
                {
                    if (bw.targetSelected == null)
                    {
                        bw.currentState = STATE.OnSquadObserving;
                    }
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

    public override void BackToPlayer()
    {
        foreach (BoogieWrestler bw in squadWrestlers)
        {
            if (bw.targetSelected != null)
            {
                if (bw.targetSelected.GetComponent<BoogieWrestler>().wrestlersAttackingMe.Contains(bw))
                {
                    bw.wrestlersAttackingMe.Remove(bw);
                }
                bw.targetSelected = null;
            }
            bw.currentState = STATE.BackToPlayer;
            bw._anim.SetInteger("closeEnoughToAttack", 0);
            bw.backToPlayer = true;
            bw._agent.stoppingDistance = 0f;
            bw._agent.SetDestination(FindObjectOfType<BoogiesSpawner>().transform.position);
        }
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
        while (true && !backToPlayer)
        {
            yield return new WaitForSeconds(0.5f);
            if (currentState == STATE.OnSquadAttacking)
            {
                if (this.visibleTargets.Count > 0)
                {
                    foreach (Transform t in commander.visibleTargets)
                    {
                        if (t != null)
                        {
                            if (t.GetComponent<BoogieWrestler>() && t.GetComponent<BoogieWrestler>().commander != null)
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
                }
            }
            yield return null;
        }
    }

    public override void Save()
    {
        base.Save();
        SaverManager.I.saveData[key + "coveringBody"] = this.coveringBody;
        SaverManager.I.saveData[key + "squadList"] = this.currentSquadList;
        SaverManager.I.saveData[key + "neededIndexs"] = this.neededIndexs;
    }

    public override void Load()
    {
        base.Load();
        coveringBody = SaverManager.I.saveData[key + "coveringBody"];
        currentSquadList = SaverManager.I.saveData[key + "squadList"];
        this.neededIndexs = SaverManager.I.saveData[key + "neededIndexs"];
    }
}