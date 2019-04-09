using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public float viewRadius;
    [Range(0,360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    public BoogieWrestler bw;

    void Awake()
    {
        bw = GetComponent<BoogieWrestler>();
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        bw.visibleTargets.Clear();
        if (bw == null || bw.currentState == STATE.OnSquadCovering)
        {
            return;
        }
        
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        for (int i = 0; i<targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float distToTarget = Vector3.Distance(this.transform.position, target.position);
                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, obstacleMask) &&
                    target.GetComponent<BoogieWrestler>() != null &&
                    target.GetComponent<BoogieWrestler>().team != bw.team)
                {
                    if (!bw.visibleTargets.Contains(target))
                    {
                        bw.visibleTargets.Add(target);
                    }
                    if (!bw.commander.visibleTargets.Contains(target))
                    {
                        bw.commander.visibleTargets.Add(target);
                    }
                }
            }
        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public void StartVision(float delay)
    {
        StartCoroutine(FindTargetsWithDelay(delay));
    }
}
