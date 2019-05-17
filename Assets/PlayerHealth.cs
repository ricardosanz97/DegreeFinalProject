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
        OnPlayerDead.Invoke();
        StartCoroutine(DieOnDelay());
       
        //lanzar un evento que sea player muerto
        //TODO: animator to die.
    }

    IEnumerator DieOnDelay()
    {
        yield return new WaitForSeconds(1f);
        _anim.SetInteger("state", 4);
    }
}
