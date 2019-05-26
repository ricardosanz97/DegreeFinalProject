using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AidilController : MonoBehaviour, ISaveable
{
    public void Load()
    {
        this.transform.position = SaverManager.I.saveData["AidilPosition"];
    }

    public void Save()
    {
        SaverManager.I.saveData.Add("AidilPosition", this.transform.position);
    }

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
