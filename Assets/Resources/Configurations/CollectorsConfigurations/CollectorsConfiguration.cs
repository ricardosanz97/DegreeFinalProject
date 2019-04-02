using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New CollectorsConfig", menuName = "CollectorsConfiguration")]
public class CollectorsConfiguration : ScriptableObject
{
    #region speed
    public float minSpeed = 4f;
    public float maxSpeed = 7f;
    [Range(0f, 1f)]
    public float probabilityVariateSpeed = 0.4f;
    public float timeTryVariateSpeed = 2f;
    #endregion

    public float maxTimeToFindObjective = 10f;
    public float timeToCheckIfWorkWinished = 2f;

    #region collecting
    public float timeToCollect = 5f;
    public float timeToCollectAgain = 2f;
    public float timeToFollowMarkerAgain = 2f;

    public float minTimeChangeDirection = 2f;
    public float maxTimeChangeDirection = 4f;
    [Range(0f,1f)]
    public float probabilityChangeDirection = 0.8f;
    #endregion
}
