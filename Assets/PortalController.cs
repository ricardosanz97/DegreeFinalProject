using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    public ParticleSystem particles;
    public PortalController linkedPortal;
    public Transform teleportedPosition;
    public bool portalEnabled = false;
    private void Awake()
    {
        particles = GetComponentInChildren<ParticleSystem>();
        teleportedPosition = this.transform.Find("TeleportedPosition");
        //EnablePortal();
    }

    public void EnablePortal()
    {
        linkedPortal.enabled = true;
        linkedPortal.particles.Play();
        portalEnabled = true;
        particles.Play();
    }

    public void DisablePortal()
    {
        linkedPortal.enabled = false;
        linkedPortal.particles.Stop();
        portalEnabled = false;
        particles.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerMovement>() && portalEnabled)
        {
            other.GetComponent<PlayerMovement>()._agent.enabled = false;
            other.GetComponent<PlayerMovement>().transform.position = linkedPortal.teleportedPosition.position;
            //other.GetComponent<PlayerMovement>()._agent.SetDestination(linkedPortal.teleportedPosition.position);
            other.GetComponent<PlayerMovement>().transform.rotation = linkedPortal.teleportedPosition.localRotation;
            DisablePortal();  
            other.GetComponent<PlayerMovement>()._agent.enabled = true;
            LevelManager.I.OnChangeStep(GAME_STEP.Step10);
        }
    }

}
