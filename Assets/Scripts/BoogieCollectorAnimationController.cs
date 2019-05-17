using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoogieCollectorAnimationController : MonoBehaviour
{
    private BoogieCollector _bc;
    public AnimationClip collectElixirAnimation;
    public AnimationClip depositElixirAnimation;

    private void Awake()
    {
        _bc = GetComponent<BoogieCollector>();
    }

    private void Update()
    {
        /*
        if (_bc._agent.velocity.magnitude <= 0.1f)
        {
            if (_bc.currentState == BoogieCollector.CURRENT_STATE.FollowingMarkersToCollectorMachine || _bc.currentState == BoogieCollector.CURRENT_STATE.GoingToCollectorMachine)
            {
                if (_bc._anim.GetInteger("collecting") != -2)
                {
                    _bc._anim.SetInteger("collecting", -2);
                }
            }
            else if (_bc.currentState == BoogieCollector.CURRENT_STATE.FollowingMarkersToElixirStone || _bc.Wandering())
            {
                if (_bc._anim.GetInteger("collecting") != -1)
                {
                    _bc._anim.SetInteger("collecting", -1);
                }
            }
        }
        else
        {
            if (_bc.Wandering() || _bc.currentState == BoogieCollector.CURRENT_STATE.FollowingMarkersToElixirStone)
            {
                if (_bc._anim.GetInteger("collecting") != 0)
                {
                    _bc._anim.SetInteger("collecting", 0);
                }
            }
            else if (_bc.currentState == BoogieCollector.CURRENT_STATE.GoingToCollectorMachine || _bc.currentState == BoogieCollector.CURRENT_STATE.FollowingMarkersToCollectorMachine)
            {
                if (_bc._anim.GetInteger("collecting") != 2)
                {
                    _bc._anim.SetInteger("collecting", 2);
                }
            }
        }
        */
    }
}
