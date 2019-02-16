using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerBehaviour : MonoBehaviour
{
    public BoogieCollector markerCreator;
    public GameObject previousMarker;
    public GameObject nextMarker;
    public float lifePeriod = 10f;

    private void Start()
    {
        StartCoroutine(OnLifePeriodElapsed());
    }

    IEnumerator OnLifePeriodElapsed()
    {
        yield return new WaitForSeconds(lifePeriod);
        Destroy(this.gameObject);
    }
}
