using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //How long the intermission between songs lasts for
    [SerializeField] private float intermissionTime;
    public float IntermissionTime { get => intermissionTime; }

    public song currentSong { get => currentSong; private set { currentSong = value; } }

    static public float timer { get => timer; private set { timer = value; } }

    public enum GameState { menu, intermission, playing }

    static public GameState currentGameState { get => currentGameState; private set { currentGameState = value; } }

    static public float happiness { get => happiness; private set { happiness = Mathf.Clamp(value, 0, 100); } }

    // Update is called once per frame
    void Update()
    {
        //Decrementing timer
        if (currentGameState == GameState.intermission)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                //After a delay, start playing the song
                GameEvents.StartSong();
            }
        }

        if (currentGameState == GameState.playing)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                //When song duration is up, call time up
                GameEvents.TimeUp();
            }
        }
    }

    //Called when game scene is loaded. Initialize level
    private void OnGameStart()
    {
        currentGameState = GameState.intermission;
        timer = intermissionTime;
        GameEvents.StartAlbum();
        Debug.Log("Game Started");
    }

    //When RecipeLoader annoucnes the next song
    private void OnReadySong(song song)
    {
        currentSong = song;
        foreach (recipe recipe in currentSong.componentRecipes)
        {
            UIEvents.DisplayRecipe(recipe);
        }
    }

    //When the song starts playing
    private void OnStartSong()
    {
        currentGameState = GameState.playing;
        timer = currentSong.duration;
    }

    //When player wins/fails
    private static void OnGameOver()
    {
        currentGameState = GameState.menu;
        MenuEvents.onGameEnded();
    }

    //When time is up for a song
    private void OnTimeUp()
    {
        //Queue up next song and start intermission timer.
        GameEvents.NextSong();
        GameEvents.EndSong();
        timer = IntermissionTime;
        currentGameState = GameState.intermission;
    }

    //When every song for level is finished
    private void OnAlbumEnded()
    {
        //Todo: transition to game over
        Debug.LogWarning("Album ended currently not implemented");
    }

    private void OnCableConnected()
    {

    }

    private void OnEnable()
    {
        GameEvents.onGameStart += OnGameStart;
        GameEvents.onGameOver += OnGameOver;
        GameEvents.onTimeUp += OnTimeUp;
        GameEvents.onEndAlbum += OnAlbumEnded;
        GameEvents.onReadySong += OnReadySong;
        GameEvents.onStartSong += OnStartSong;
    }

    private void OnDisable()
    {
        GameEvents.onGameStart -= OnGameStart;
        GameEvents.onGameOver -= OnGameOver;
        GameEvents.onTimeUp -= OnTimeUp;
        GameEvents.onEndAlbum -= OnAlbumEnded;
        GameEvents.onReadySong -= OnReadySong;
        GameEvents.onStartSong -= OnStartSong;
    }


}
