using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerHealth : AttackTarget
{
    public bool alive = true;
    public TEAM team = TEAM.A;

    private Animator _anim;

    public static event Action OnPlayerDead;

    private void Awake()
    {
        initialHealth = health;
        _anim = GetComponent<Animator>();
    }

    public override void Die()
    {
        alive = false;
        if (LevelManager.I.aidilConversation.activeInHierarchy)
        {
            LevelManager.I.aidilConversation.SetActive(false);
        }
        if (LevelManager.I.giantsConversation.activeInHierarchy)
        {
            LevelManager.I.giantsConversation.SetActive(false);
        }
        if (LevelManager.I.anciantConversation.activeInHierarchy)
        {
            LevelManager.I.anciantConversation.SetActive(false);
        }
        if (LevelManager.I.distancesConversation.activeInHierarchy)
        {
            LevelManager.I.distancesConversation.SetActive(false);
        }
        OnPlayerDead.Invoke();
        StartCoroutine(DieOnDelay());
    }

    IEnumerator DieOnDelay()
    {
        yield return new WaitForSeconds(1f);
        _anim.SetInteger("state", 4);
        yield return new WaitForSeconds(3.5f);
        UIController.I.PlayerIsDead();
    }
}
