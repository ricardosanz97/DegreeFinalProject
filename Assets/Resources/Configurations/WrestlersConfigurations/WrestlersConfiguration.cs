using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New WrestlersConfig", menuName = "WrestlersConfiguration")]
public class WrestlersConfiguration : ScriptableObject
{
    #region health
    public float commanderHealth = 100;
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
    public float commanderVisionDistance = 8f;
    public float giantVisionDistance = 4f;
    public float closeVisionDistance = 6f;
    public float distanceVisionDistance = 10f;
    #endregion

    #region fieldOfView
    public float commanderFieldView = 60f;
    public float giantFieldView = 90f;
    public float closeFieldView = 55f;
    public float distanceFieldView = 45f;
    #endregion

    #region attackRange
    public float commanderAttackRange = 1.5f;
    public float giantAttackRange = 1.5f;
    public float closeAttackRange = 1.5f;
    public float distanceAttackRange = 7f;
    #endregion

    #region attackDamage
    public float commanderAttackDamage = 5f;
    public float giantAttackDamage = 3f;
    public float closeAttackDamage = 4f;
    public float distanceAttackDamage = 3f;
    #endregion

    #region timeFindTargets
    public float closeTimeFindTargets = 0.5f;
    public float distanceTimeFindTargets = 0.5f;
    public float giantTimeFindTargets = 1.2f;
    public float commanderTimeFindTargets = 1f;
    #endregion

    #region offset
    public float minOffset = 1.5f;
    public float maxOffset = 2f;
    [Range(0f, 1f)]
    public float probabilityVariateOffset = 0.5f;
    public float timeVariateOffset = 4f;
    #endregion

    #region changeObjective
    public float closeMinTimeChange = 5f;
    public float closeMaxTimeChange = 10f;
    public float distanceMinTimeChange = 5f;
    public float distanceMaxTimeChange = 10f;
    public float giantMinTimeChange = 2f;
    public float giantMaxTimeChange = 5f;
    public float commanderMinTimeChange = 4f;
    public float commanderMaxTimeChange = 9f;
    [Range(0f,1f)]
    public float closeProbChange = 0.5f;
    [Range(0f, 1f)]
    public float distanceProbChange = 0.5f;
    [Range(0f, 1f)]
    public float giantProbChange = 0.75f;
    [Range(0f, 1f)]
    public float commanderProbChange = 0.6f;
    #endregion

    #region speed
    public float distanceMinSpeed = 9f;
    public float distanceMaxSpeed = 14f;
    [Range(0f, 1f)]
    public float distanceProbabilityVariateSpeed = 0.75f;
    public float distanceTimeVariateSpeed = 2f;
    public float commanderMinSpeed = 10f;
    public float commanderMaxSpeed = 10f;
    [Range(0f, 1f)]
    public float commanderProbabilityVariateSpeed = 0f;
    public float commanderTimeVariateSpeed = 3.5f;
    public float giantMinSpeed = 6f;
    public float giantMaxSpeed = 7f;
    [Range(0f, 1f)]
    public float giantProbabilityVariateSpeed = 0.2f;
    public float giantTimeVariateSpeed = 4f;
    public float closeMinSpeed = 8f;
    public float closeMaxSpeed = 12f;
    [Range(0f, 1f)]
    public float closeProbabilityVariateSpeed = 0.6f;
    public float closeTimeVariateSpeed = 2.5f;

    public float timeVariateSpeed = 3f;
    #endregion

    #region preferences
    public float closeProbabilityPreferences = 0.85f;
    public float giantProbabilityPreferences = 0.7f;
    public float commanderProbabilityPreferences = 0.8f;
    public float distanceProbabilityPreferences = 0.6f;
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

    public BoogieWrestler.WRESTLER_TYPE[] DistancePreferences =
    {
        BoogieWrestler.WRESTLER_TYPE.Close,
        BoogieWrestler.WRESTLER_TYPE.Giant,
        BoogieWrestler.WRESTLER_TYPE.Commander,
        BoogieWrestler.WRESTLER_TYPE.Distance
    };
    #endregion
}
