using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New CleanersConfig", menuName = "CleanersConfiguration")]
public class CleanersConfiguration : ScriptableObject
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

    #region cleaning
    public float timeToChargeAgain = 2f;
    public float timeToUnchargeAgain = 2f;
    #endregion

}
