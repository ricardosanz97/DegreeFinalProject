using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnciantController : MonoBehaviour, ISaveable
{
    Animator _anim;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }
    public void Load()
    {
        this.transform.position = SaverManager.I.saveData["AncientPosition"];
        _anim.SetInteger("state", SaverManager.I.saveData["AncientAnimState"]);
    }

    public void Save()
    {
        SaverManager.I.saveData.Add("AncientPosition", this.transform.position);
        SaverManager.I.saveData.Add("AncientAnimState", _anim.GetInteger("state"));
    }

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
