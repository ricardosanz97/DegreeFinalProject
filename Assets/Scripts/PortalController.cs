using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour, ISaveable
{
    public ParticleSystem particles;
    public PortalController linkedPortal;
    public Transform teleportedPosition;
    public bool portalEnabled = false;
    public long id;
    private void Awake()
    {
        particles = GetComponentInChildren<ParticleSystem>();
        teleportedPosition = this.transform.Find("TeleportedPosition");
        id = GetHashCode();
        //EnablePortal();
    }

    private void OnEnable()
    {
        SaverManager.OnLoadData += Load;
        SaverManager.OnSaveData += Save;
    }

    public void Save()
    {
        SaverManager.I.saveData.Add("Portal" + id + "ParticlesSystemEnabled", particles.isPlaying);
        SaverManager.I.saveData.Add("Portal" + id + "PortalEnabled", portalEnabled);
    }

    public void Load()
    {
        portalEnabled = SaverManager.I.saveData["Portal" + id + "PortalEnabled"];
        Debug.Log("portal enabled = " + portalEnabled);
        if (portalEnabled)
        {
            EnablePortal();
        }
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

            if (LevelManager.I.limitedVersion)
            {
                LevelManager.I.ConversationWithDistancesFinished();
                return;
            }
            LevelManager.I.OnChangeStep(GAME_STEP.Step10);
        }
    }

}
