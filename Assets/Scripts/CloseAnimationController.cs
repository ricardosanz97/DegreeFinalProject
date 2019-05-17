using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseAnimationController : AnimationController
{
    public BoogieWrestlerClose bwc;
    Animator anim;
    public AnimationClip attack;
    public bool attacking = false;
    
    private void Awake()
    {
        bwc = GetComponent<BoogieWrestlerClose>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (attacking)
        {
            return;
        }

        if (bwc.currentState == STATE.OnSquadCovering)
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
                anim.SetInteger("closeEnoughToAttack", 1);
            }
        }
        else if ((bwc.currentState == STATE.OnSquadMoving || bwc.currentState == STATE.AloneMoving) && anim.GetInteger("closeEnoughToAttack") != 0)
        {
            anim.SetInteger("closeEnoughToAttack", 0);
        }
        else if ((bwc.currentState == STATE.OnSquadObserving || bwc.currentState == STATE.AloneObserving || bwc.currentState == STATE.OnSquadCoveringMoving) && (anim.GetInteger("closeEnoughToAttack") != -1 || anim.GetInteger("closeEnoughToAttack") != 0))
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
        else if (bwc.currentState == STATE.AloneAttacking || bwc.currentState == STATE.OnSquadAttacking)
        {
            if (bwc.closeEnoughToAttack)
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
        //return bwc._agent.velocity != Vector3.zero;
        return bwc._agent.velocity.magnitude > 4f;
    }

    private bool IsWalking()
    {
        //return bwc._agent.velocity != Vector3.zero;
        return bwc._agent.velocity.magnitude > 0.5f;
    }
}
