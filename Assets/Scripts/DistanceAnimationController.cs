using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceAnimationController : AnimationController
{
    public BoogieWrestlerDistance bwd;
    Animator anim;
    public AnimationClip attack;
    public bool attacking = false;

    private void Awake()
    {
        bwd = GetComponent<BoogieWrestlerDistance>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (attacking)
        {
            return;
        }

        if (bwd.currentState == STATE.AloneFollowingPlayer && (anim.GetInteger("closeEnoughToAttack") != 0 || anim.GetInteger("closeEnoughToAttack") != -1))
        {
            if (IsWalking() || IsMoving())
            {
                anim.SetInteger("closeEnoughToAttack", 0);
            }
            else
            {
                anim.SetInteger("closeEnoughToAttack", -1);
            }
        }

        if (bwd.currentState == STATE.OnSquadCovering)
        {
            
            if (IsMoving()) //taking squad position
            {
                anim.SetInteger("closeEnoughToAttack", 0);
            }
            else if (IsWalking())
            {
                anim.SetInteger("closeEnoughToAttack", 2);
            }
            else
            {
                anim.SetInteger("closeEnoughToAttack", -1);
            }
        }

        else if ((bwd.currentState == STATE.OnSquadMoving || bwd.currentState == STATE.AloneMoving || bwd.currentState == STATE.OnSquadCoveringMoving) && anim.GetInteger("closeEnoughToAttack") != 0)
        {
            anim.SetInteger("closeEnoughToAttack", 0);
        }
        else if ((bwd.currentState == STATE.OnSquadObserving || bwd.currentState == STATE.AloneObserving) && (anim.GetInteger("closeEnoughToAttack") != -1 || anim.GetInteger("closeEnoughToAttack") != 0))
        {
            if (IsMoving())
            {
                anim.SetInteger("closeEnoughToAttack", 0);
            }
            else if (IsWalking()) //taking squad position
            {
                anim.SetInteger("closeEnoughToAttack", 2);
            }
            else
            {
                anim.SetInteger("closeEnoughToAttack", -1);
            }
        }
        else if (bwd.currentState == STATE.AloneAttacking || bwd.currentState == STATE.OnSquadAttacking)
        {
            if (bwd.targetSelected != null && bwd.CloseEnough(bwd.targetSelected.transform.position))
            {
                if (IsMoving() && anim.GetInteger("closeEnoughToAttack") != 0)
                {
                    anim.SetInteger("closeEnoughToAttack", 0);
                }
                else if (IsWalking() && anim.GetInteger("closeEnoughToAttack") != 2)
                {
                    anim.SetInteger("closeEnoughToAttack", 2);
                }
                else if (anim.GetInteger("closeEnoughToAttack") != 1)
                {
                    anim.SetInteger("closeEnoughToAttack", 1);
                }
            }
            else
            {
                if (IsMoving() && anim.GetInteger("closeEnoughToAttack") != 0)
                {
                    anim.SetInteger("closeEnoughToAttack", 0);
                }
                /*
                else if (IsWalking() && anim.GetInteger("closeEnoughToAttack") != 2)
                {
                    anim.SetInteger("closeEnoughToAttack", 2);
                }
                */
            }
        }
    }

    public override void Attack()
    {
        StartCoroutine(AttackAnimation());
    }

    private IEnumerator AttackAnimation()
    {
        attacking = true;
        anim.SetInteger("closeEnoughToAttack", 3);
        yield return new WaitForSeconds(attack.length);
        attacking = false;
        yield return null;
    }

    private bool IsMoving()
    {
        //return bwd._agent.velocity != Vector3.zero;
        return bwd._agent.velocity.magnitude > 4f;
    }

    private bool IsWalking()
    {
        //return bwd._agent.velocity != Vector3.zero;
        return bwd._agent.velocity.magnitude > 0.5f;
    }
}
