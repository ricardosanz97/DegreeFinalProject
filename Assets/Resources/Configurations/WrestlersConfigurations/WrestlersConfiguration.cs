using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[CreateAssetMenu(fileName = "New WrestlersConfig", menuName = "WrestlersConfiguration")]
public class WrestlersConfiguration : ScriptableObject
{
    [System.Serializable]
    public class MyOwnEvent : UnityEvent { };
    [Header("Health Settings")]
    #region health
    public int commanderHealth = 250;
    public int giantHealth = 200;
    public int closeHealth = 150;
    public int distanceHealth = 120;
    #endregion
    [Header("Attack speed Settings")]
    #region attackSpeed
    public float commanderAttackSpeed = 3f;
    public float giantAttackSpeed = 4f;
    public float closeAttackSpeed = 2.5f;
    public float distanceAttackSpeed = 4f;
    #endregion
    [Header("Vision distance Settings")]
    #region visionDistance
    public float commanderVisionDistance = 8f;
    public float giantVisionDistance = 4f;
    public float closeVisionDistance = 6f;
    public float distanceVisionDistance = 10f;
    #endregion
    [Header("Field of view Settings")]
    #region fieldOfView
    public float commanderFieldView = 60f;
    public float giantFieldView = 90f;
    public float closeFieldView = 55f;
    public float distanceFieldView = 45f;
    #endregion
    [Header("Attack range Settings")]
    #region attackRange
    public float distanceAttackRange = 7f;
    #endregion
    [Header("Attack damage Settings")]
    #region attackDamage
    public int commanderAttackDamage = 5;
    public int giantAttackDamage = 3;
    public int closeAttackDamage = 4;
    public int distanceAttackDamage = 3;
    #endregion
    [Header("Time find targets Settings")]
    #region timeFindTargets
    public float closeTimeFindTargets = 0.5f;
    public float distanceTimeFindTargets = 0.5f;
    public float giantTimeFindTargets = 1.2f;
    public float commanderTimeFindTargets = 1f;
    #endregion
    [Header("Offset between them Settings")]
    #region offset
    public float minOffset = 1.5f;
    public float maxOffset = 2f;
    [Range(0f, 1f)]
    public float probabilityVariateOffset = 0.5f;
    public float timeVariateOffset = 4f;
    #endregion
    [Header("Changing objective Settings")]
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
    [Header("Speed Settings")]
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
    [Header("Probability preferences Settings")]
    #region probabilityPreferences
    [Range(0f, 1f)]
    public float closeProbabilityPreferences = 0.85f;
    [Range(0f, 1f)]
    public float giantProbabilityPreferences = 0.7f;
    [Range(0f, 1f)]
    public float commanderProbabilityPreferences = 0.8f;
    [Range(0f, 1f)]
    public float distanceProbabilityPreferences = 0.6f;
    #endregion
    [Header("Preferences Settings")]
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
    [Header("In-battle Settings")]
    #region emergencyParameters
    public float percentHpCoverClose = 0.6f;
    public float percentHpCoverDistance = 0.6f;
    public float percentHpCoverCommander = 0.5f;
    public float percentHpCoverGiant = 0.4f;
    public float minTimeCoverAgain = 6f;
    public float maxTimeCoverAgain = 12f;
    #endregion
    [Header("Callbacks when die Settings")]
    #region onDie
    public OnDieEvent commanderOnDieCallback = new OnDieEvent();
    public OnDieEvent giantOnDieCallback = new OnDieEvent();
    public UnityEvent distanceOnDieCallback;
    public UnityEvent closeOnDieCallback;
    #endregion
}
