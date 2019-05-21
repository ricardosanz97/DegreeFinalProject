using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MenuEntityMovement : MonoBehaviour
{
    public Transform spawnPoint;
    public Transform destinyPoint;
    private NavMeshAgent _agent;
    private Animator _anim;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
    }

    public void AssignPoint(Transform spawn, Transform destiny)
    {
        this.spawnPoint = spawn;
        this.destinyPoint = destiny;
    }

    public void Trigger()
    {
        _anim.SetInteger("closeEnoughToAttack", 0);
        _agent.SetDestination(destinyPoint.position);
    }
}
