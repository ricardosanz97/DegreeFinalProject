using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerBehaviour : MonoBehaviour
{
    public BoogieCollector markerCreator;
    public GameObject previousMarker;
    public GameObject nextMarker;
    public float lifePeriod;
    public ElixirObstacleStone.TYPE type;
    public bool valid;

    private void Start()
    {
        StartCoroutine(OnLifePeriodElapsed());
        lifePeriod = markerCreator.markerLifePeriod;
    }

    IEnumerator OnLifePeriodElapsed()
    {
        Debug.Log("life period = " + lifePeriod);
        yield return new WaitForSeconds(lifePeriod);
        Destroy(this.gameObject);
    }
}
