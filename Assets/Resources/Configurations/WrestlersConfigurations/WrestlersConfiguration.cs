using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New WrestlersConfig", menuName = "WrestlersConfiguration")]
public class WrestlersConfiguration : ScriptableObject
{
    #region health
    public float commmanderHealth = 100;
    public float giantHealth = 150;
    public float closeHealth = 70;
    public float distanceHealth = 50;
    #endregion

    #region attackSpeed
    public float commanderAttackSpeed = 1.5f;
    public float giantAttackSpeed = 2f;
    public float closeAttackSpeed = 1f;
    public float distanceAttackSpeed = 1.2f;
    #endregion

    #region visionDistance
    public float commanderVisionDistance = 10f;
    public float giantVisonDistance = 4f;
    public float closeVisionDistance = 6f;
    public float distanceVisionDistance = 9f;
    #endregion

    #region attackRange
    public float commanderAttackRange = 1f;
    public float giantAttackRange = 1f;
    public float closeAttackRange = 1f;
    public float distanceAttackRange = 5f;
    #endregion

    #region attackDamage
    public float commanderAttackDamage = 5f;
    public float giantAttackDamage = 3f;
    public float closeAttackDamage = 4f;
    public float distanceAttackDamage = 3f;
    #endregion

    #region offset
    public float minOffset = 1.5f;
    public float maxOffset = 2f;
    [Range(0f, 1f)]
    public float probabilityVariateOffset = 0.5f;
    public float timeVariateOffset = 4f;
    #endregion

    #region speed
    public float distanceMinSpeed = 9f;
    public float distanceMaxSpeed = 14f;
    [Range(0f, 1f)]
    public float distanceProbabilityVariateSpeed = 0.75f;
    public float commanderMinSpeed = 10f;
    public float commanderMaxSpeed = 10f;
    [Range(0f, 1f)]
    public float comamnderProbabilityVariateSpeed = 0f;
    public float giantMinSpeed = 6f;
    public float giantMaxSpeed = 7f;
    [Range(0f, 1f)]
    public float giantProbabilityVariateSpeed = 0.2f;
    public float closeMinSpeed = 8f;
    public float closeMaxSpeed = 12f;
    [Range(0f, 1f)]
    public float closeProbabilityVariateSpeed = 0.6f;

    public float timeVariateSpeed = 3f;
    #endregion

    #region preferences
    public BoogieWrestler.WRESTLER_TYPE[] CommanderPreferences = {
        BoogieWrestler.WRESTLER_TYPE.Commander, 
        BoogieWrestler.WRESTLER_TYPE.Giant, 
        BoogieWrestler.WRESTLER_TYPE.Close, 
        BoogieWrestler.WRESTLER_TYPE.Distance 
    };

    public BoogieWrestler.WRESTLER_TYPE[] GiantPreferences = {
        BoogieWrestler.WRESTLER_TYPE.Giant,
        BoogieWrestler.WRESTLER_TYPE.Close,
        BoogieWrestler.WRESTLER_TYPE.Distance,
        BoogieWrestler.WRESTLER_TYPE.Commander
    };

    public BoogieWrestler.WRESTLER_TYPE[] ClosePreferences = {
        BoogieWrestler.WRESTLER_TYPE.Distance,
        BoogieWrestler.WRESTLER_TYPE.Close,
        BoogieWrestler.WRESTLER_TYPE.Distance,
        BoogieWrestler.WRESTLER_TYPE.Commander
    };

    public BoogieWrestler.WRESTLER_TYPE[] DistanceWrestlers =
    {
        BoogieWrestler.WRESTLER_TYPE.Close,
        BoogieWrestler.WRESTLER_TYPE.Giant,
        BoogieWrestler.WRESTLER_TYPE.Commander,
        BoogieWrestler.WRESTLER_TYPE.Distance
    };
    #endregion
}
