using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerGiants : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>() && LevelManager.I.currentStep == GAME_STEP.Step4)
        {
            if (LevelManager.I.limitedVersion)
            {
                LevelManager.I.ConversationWithGiantsFinished();
                return;
            }
            LevelManager.I.OnChangeStep(GAME_STEP.Step5);
        }
    }
}
