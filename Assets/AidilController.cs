using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AidilController : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>() && LevelManager.I.currentStep == GAME_STEP.Step6)
        {
            if (!LevelManager.I.conversationWithAidilFinished)
            {
                LevelManager.I.OnChangeStep(GAME_STEP.Step7);
            }
        }
    }
}
