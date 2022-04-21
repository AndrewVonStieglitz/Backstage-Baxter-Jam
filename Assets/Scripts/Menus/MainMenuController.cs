using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] Button quitButton;
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
        startButton.onClick.AddListener(StartClicked);
        quitButton.onClick.AddListener(QuitClicked);
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
    }


    private void StartClicked()
    {
        MenuEvents.StartSelected();
    }

    private void QuitClicked()
    {
        MenuEvents.GameQuit();
        Application.Quit();
    }
}
