using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadTeam : MonoBehaviour
{
    public TEAM team;
    public List<BoogieWrestler> enemyWrestlers = new List<BoogieWrestler>();

    private void Update()
    {
        enemyWrestlers.RemoveAll((x) => x == null);
    }
}
