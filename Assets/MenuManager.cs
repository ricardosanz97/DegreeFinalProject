using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public MenuEntityMovement first;
    public Transform spawnFirst;
    public Transform destinyFirst;
    public MenuEntityMovement second;
    public Transform spawnSecond;
    public Transform destinySecond;
    public MenuDoorBehavior door;

    public GameObject creditsPanel;
    public GameObject menuPanel;
    public GameObject aboutPanel;

    private void Start()
    {
        this.GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Sounds/menu");
        this.GetComponent<AudioSource>().Play();
        SelectEntities();
    }

    public void SelectEntities()
    {
        GameObject firstEntity = Instantiate(Resources.Load<GameObject>("Prefabs/MenuEntities/Entities/" + Random.Range(1, 8)), spawnFirst.transform.position, spawnFirst.transform.rotation);
        first = firstEntity.GetComponent<MenuEntityMovement>();
        first.AssignPoint(spawnFirst, destinyFirst);

        GameObject secondEntity = Instantiate(Resources.Load<GameObject>("Prefabs/MenuEntities/Entities/" + Random.Range(1, 8)), spawnSecond.transform.position, spawnSecond.transform.rotation);
        second = secondEntity.GetComponent<MenuEntityMovement>();
        second.AssignPoint(spawnSecond, destinySecond);
    }

    public void RestartAnimation()
    {
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z - 60);
        door.RestartDoor();
        Destroy(first.gameObject);
        Destroy(second.gameObject);
        SelectEntities();
    }

    public void TriggerFirst()
    {
        first.Trigger();
    }

    public void TriggerSecond()
    {
        second.Trigger();
    }

    public void TriggerDoor()
    {
        door.Trigger();
    }

    public void OpenSandboxScene()
    {
        GameController.I.LoadScene((int)SCENES.Sandbox);
        //SceneManager.LoadScene((int)SCENES.Sandbox);
    }

    public void OpenCompleteDemoScene()
    {
        GameController.I.LoadScene((int)SCENES.Game);
        //SceneManager.LoadScene((int)SCENES.Game);
    }

    public void OpenLimitedDemoScene()
    {
        GameController.I.LoadScene((int)SCENES.LimitedGame);
        //SceneManager.LoadScene((int)SCENES.LimitedGame);
    }

    public void QuitButtonPressed()
    {
        Application.Quit();
    }

    public void AboutButtonPressed()
    {
        menuPanel.SetActive(false);
        creditsPanel.SetActive(false);
        aboutPanel.SetActive(true);
    }

    public void CreditsButtonPressed()
    {
        menuPanel.SetActive(false);
        aboutPanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void BackCreditsButtonPressed()
    {
        creditsPanel.SetActive(false);
        menuPanel.SetActive(true);
    }

    public void BackAboutButtonPresed()
    {
        aboutPanel.SetActive(false);
        menuPanel.SetActive(true);
    }
}
