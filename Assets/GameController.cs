using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SCENES
{
    Loader = 0,
    Menu = 1,
    Game = 2,
    LimitedGame = 3,
    Sandbox = 4
}

public class GameController : Singleton<GameController>
{
    private void Awake()
    {
        Debug.Log("GameController created!");
    }
    public void LoadScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }
}
