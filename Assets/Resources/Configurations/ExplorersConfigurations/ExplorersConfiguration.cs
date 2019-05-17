using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New ExplorersConfig", menuName = "ExplorersConfiguration")]
public class ExplorersConfiguration : ScriptableObject
{
    [Header("Speed Settings")]
    #region speed
    public float minSpeed = 4f;
    public float maxSpeed = 7f;
    [Range(0f, 1f)]
    public float probabilityVariateSpeed = 0.4f;
    public float timeTryVariateSpeed = 2f;
    #endregion

    [Header("Logic Settings")]
    public float maxTimeToFindObjective = 10f;
    public float timeToCheckIfWorkWinished = 2f;

    #region exploring
    [Header("Exploring Settings")]
    public float timeToCarryAgain = 5f;
    [Range(0f, 1f)]
    public float probabilityFollowClue = 1f;
    #endregion
}
