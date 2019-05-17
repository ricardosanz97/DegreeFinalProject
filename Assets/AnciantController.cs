using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnciantController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>())
        {
            if (!LevelManager.I.conversationWithAnciantFinished)
            {
                LevelManager.I.OnChangeStep(GAME_STEP.Step1);
            }
        }
    }
}
