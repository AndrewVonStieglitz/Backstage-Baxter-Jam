using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] string level1SceneName;
    [SerializeField] string gameOverSceneName;
    [SerializeField] string mainMenuSceneName;
    // Start is called before the first frame update
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        MenuEvents.onStartSelected += SwitchToGameScene;
        MenuEvents.onGameEnded += SwitchToGameOver;
        MenuEvents.onReturnToMainMenu += SwitchToMainMenu;
        SceneManager.sceneLoaded += OnSceneLoadDo;
    }

    private void OnDisable()
    {
        MenuEvents.onStartSelected -= SwitchToGameScene;
        MenuEvents.onGameEnded -= SwitchToGameOver;
        MenuEvents.onReturnToMainMenu -= SwitchToMainMenu;
        SceneManager.sceneLoaded -= OnSceneLoadDo;
    }

    private void SwitchToGameScene()
    {
        SceneManager.LoadScene(level1SceneName);

    }

    private void SwitchToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void SwitchToGameOver()
    {
        SceneManager.LoadScene(gameOverSceneName);

    }

    private void OnSceneLoadDo(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == level1SceneName)
        {
            GameEvents.GameStart();
            Debug.Log("Starting Game");
        }
    }
}
