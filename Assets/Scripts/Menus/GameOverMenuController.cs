using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenuController : MonoBehaviour
{
    [SerializeField] Button restartButton;
    [SerializeField] Button menuButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        restartButton.onClick.AddListener(RestartGame);
        menuButton.onClick.AddListener(ReturnToMenu);
    }

    private void OnDisable()
    {
        restartButton.onClick.RemoveAllListeners();
        menuButton.onClick.RemoveAllListeners();
    }

    private void RestartGame()
    {
        MenuEvents.onStartSelected();
    }

    private void ReturnToMenu()
    {
        MenuEvents.ReturnToMainMenu();
    }
}
