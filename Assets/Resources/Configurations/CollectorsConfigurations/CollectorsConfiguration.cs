using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New CollectorsConfig", menuName = "CollectorsConfiguration")]
public class CollectorsConfiguration : ScriptableObject
{
    [Header("Speed settings")]
    #region speed
    public float minSpeed = 4f;
    public float maxSpeed = 7f;
    [Range(0f, 1f)]
    public float probabilityVariateSpeed = 0.4f;
    public float timeTryVariateSpeed = 2f;
    #endregion

    [Header("Logic settings")]
    public float maxTimeToFindObjective = Mathf.Infinity;
    public float timeToCheckIfWorkWinished = 2f;

    [Header("Collecting settings")]
    #region collecting
    public float timeToCollect = 5f;
    public float timeToCollectAgain = 2f;
    public float timeToFollowMarkerAgain = 2f;

    public float minTimeChangeDirection = 2f;
    public float maxTimeChangeDirection = 4f;
    [Range(0f,1f)]
    public float probabilityChangeDirection = 0.8f;
    public float timeToDeposit = 2.5f;
    public float timeToReleaseMarker = 0.3f;

    public float markersLifePeriod = 10f;
    #endregion

}
