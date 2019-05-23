using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadTeam : MonoBehaviour
{
    public TEAM team;
    public List<BoogieWrestler> enemyWrestlers = new List<BoogieWrestler>();
    public long uniqueId;

    private void OnEnable()
    {
        uniqueId = GetHashCode() * Random.Range(1, 9);
        while (SaverManager.I.uniqueIds.Contains(uniqueId))
        {
            uniqueId = GetHashCode() * (int)Time.unscaledTime * Random.Range(1, 9);
        }
        Debug.Log("uniqueId = " + uniqueId);
    }

    private void Update()
    {
        enemyWrestlers.RemoveAll((x) => x == null);
        if (this.transform.childCount == 0)
        {
            Destroy(this.gameObject);
        }
    }
}
