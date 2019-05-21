using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SCENES
{
    Loader = 0,
    Menu = 1,
    Game = 2,
    Sandbox = 3
}

public class GameController : Singleton<GameController>
{
    public void LoadScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }
}
