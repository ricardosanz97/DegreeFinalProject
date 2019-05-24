using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{
    private Animator _animator;
    public NavMeshAgent _agent;
    public float speedMovement = 2f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (UIController.I.writing)
        {
            return;
        }
        if (!GetComponent<PlayerHealth>().alive || !LevelManager.I.GameStarted) { return; }
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        if (vertical > 0)
        {
            HandlePositiveMovement(horizontal, vertical);
        }
        else if (vertical < 0)
        {
            //HandleNegativeMovement(horizontal, vertical);
        }
        else
        {
            if (_animator.GetInteger("state") != 0)
            {
                _animator.SetInteger("state", 0);
            }
        }
        
    }

    public void LookAtY(Vector3 position)
    {
        transform.LookAt(new Vector3(position.x, transform.position.y, position.z));
    }

    public void HandlePositiveMovement(float horizontal, float vertical)
    {
        if (horizontal != 0 || vertical != 0)
        {
            if (_animator.GetInteger("state") != 1 && !_agent.isStopped)
            {
                _animator.SetInteger("state", 1);
            }
            Vector3 input = new Vector3(horizontal, 0, vertical);
            if (input.magnitude > 1) input = input.normalized;

            Vector3 angles = Camera.main.transform.rotation.eulerAngles;
            angles.x = 0;
            Quaternion rotation = Quaternion.Euler(angles);

            Vector3 direction = rotation * input;

            Debug.DrawLine(transform.position, transform.position + direction, Color.green, 0, false);

            _agent.velocity = direction * speedMovement;

            LookAtY(transform.position + direction);
        }
    }

    private void HandleNegativeMovement(float horizontal, float vertical)
    {
        throw new NotImplementedException();
    }

}

