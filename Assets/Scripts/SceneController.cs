using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

class MyClass
{

    static string level1SceneName = "Level_1";
    static string gameOverSceneName = "Game_Over_Menu";
    static string mainMenuSceneName = "Main_Menu";


    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad()
    {
        MenuEvents.onStartSelected += SwitchToGameScene;
        MenuEvents.onGameEnded += SwitchToGameOver;
        MenuEvents.onReturnToMainMenu += SwitchToMainMenu;
        SceneManager.sceneLoaded += OnSceneLoadDo;

        // this is neccessary cause sceneLoaded doesn't fire for the first scene
        Debug.Log(SceneManager.GetActiveScene().name);
        if (SceneManager.GetActiveScene().name != gameOverSceneName && SceneManager.GetActiveScene().name != mainMenuSceneName)
        {
            GameEvents.GameStart();
            Debug.Log("Starting Game");
        }
    }


    private static void SwitchToGameScene()
    {
        SceneManager.LoadScene(level1SceneName);

    }

    private static void SwitchToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private static void SwitchToGameOver()
    {
        SceneManager.LoadScene(gameOverSceneName);

    }

    private static void OnSceneLoadDo(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name != mainMenuSceneName && scene.name != gameOverSceneName)
        {
            GameEvents.GameStart();
            Debug.Log("Starting Game");
        }
    }

}