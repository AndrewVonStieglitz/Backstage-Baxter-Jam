using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] static private float maxTime;
    static public float MaxTime { get => maxTime; }

    static private float timer;
    static public float Timer { get => timer; }

    public enum GameState { menu, playing }

    static private GameState currentGameState = GameState.menu;
    static public GameState CurrentGameState { get => currentGameState; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentGameState == GameState.playing)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                GameEvents.TimeUp();
            }
        }
    }

    private void OnEnable()
    {
        GameEvents.onGameStart += OnGameStart;
        GameEvents.onGameOver += OnGameOver;
        GameEvents.onTimeUp += OnTimeUp;
    }

    private void OnDisable()
    {
        GameEvents.onGameStart -= OnGameStart;
        GameEvents.onGameOver -= OnGameOver;
        GameEvents.onTimeUp -= OnTimeUp;
    }

    private void OnGameStart()
    {
        currentGameState = GameState.playing;
        timer = maxTime;
    }

    private void OnGameOver()
    {
        currentGameState = GameState.menu;
    }

    private void OnTimeUp()
    {
        GameEvents.GameOver();
    }


}
