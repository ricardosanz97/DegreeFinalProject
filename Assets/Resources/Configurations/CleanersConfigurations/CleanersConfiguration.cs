using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New CleanersConfig", menuName = "CleanersConfiguration")]
public class CleanersConfiguration : ScriptableObject
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
    public float maxTimeToFindObjective = Mathf.Infinity;
    public float timeToCheckIfWorkWinished = 2f;
    [Header("Cleaning Settings")]
    #region cleaning
    public float timeToChargeAgain = 2f;
    public float timeToUnchargeAgain = 2f;
    #endregion

}
