using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cables;

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

    IDictionary<InstrumentSO, recipe> recipeDictionary;
    IDictionary<CableController, InstrumentSO> connectionsDictionary;

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
            timer += Time.deltaTime;
            if (timer >= currentSong.duration)
            {
                //When song duration is up, call time up. Timer counts up instead of down to act as timer for song
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
        UIEvents.ClearRecipes();
        currentSong = song;
        foreach (recipe recipe in currentSong.componentRecipes)
        {
            recipeDictionary.Add(recipe.instrument, recipe);
            UIEvents.DisplayRecipe(recipe);
        }
    }

    //When the song starts playing
    private void OnStartSong()
    {
        currentGameState = GameState.playing;
        timer = 0;
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
        Invoke("EndGame", 5);
    }

    private void OnCableConnected(CableController cable, PlugCable plug)
    {
        InstrumentSO instrument = cable.instrument;
        if (recipeDictionary.TryGetValue(instrument, out recipe recipe))
        {
            //If matching recipe found with correct instrument
            int totalPluggables = recipe.midAffectors.Length + 2;
            if (cable.pluggablesList.Count == totalPluggables)
            {//First checks if size of list matches
                if (cable.pluggablesList[0] == recipe.amp && cable.pluggablesList[totalPluggables - 1] == recipe.speaker) //Checks if amp and speaker are correct
                {
                    List<PluggablesSO> pluggables = new List<PluggablesSO>(cable.pluggablesList); //Makes duplicate of lists so operations can be done on it without affecting original
                    foreach(MidAffectorSuper midAffector in recipe.midAffectors)
                    {
                        if (!pluggables.Contains(midAffector))
                        {
                            GameEvents.RecipeBroken(recipe); //If does not contain, it must be a broken recipe
                            return;
                        }
                        else
                        {
                            //removes item if it does contain so it can't represent duplicates
                            pluggables.Remove(midAffector);
                        }
                    }
                    //If it hasn't been stopped, recipe must be complete.
                    GameEvents.RecipeCompleted(recipe);
                    return;
                }
            }
            //If no conditions are met, recipe must be broken
            GameEvents.RecipeBroken(recipe);
            
        }
        else
        {
            Debug.LogError("No recipe found matching instrument");
        }
        //cable.pluggablesList;
        //plug.pluggable

    }

    private void EndGame()
    {
        GameEvents.GameOver();
    }

    private void OnEnable()
    {
        GameEvents.onGameStart += OnGameStart;
        GameEvents.onGameOver += OnGameOver;
        GameEvents.onTimeUp += OnTimeUp;
        GameEvents.onEndAlbum += OnAlbumEnded;
        GameEvents.onReadySong += OnReadySong;
        GameEvents.onStartSong += OnStartSong;
        GameEvents.onCableConnectPlug += OnCableConnected;
        GameEvents.onCableDisconnectPlug += OnCableConnected;
    }

    private void OnDisable()
    {
        GameEvents.onGameStart -= OnGameStart;
        GameEvents.onGameOver -= OnGameOver;
        GameEvents.onTimeUp -= OnTimeUp;
        GameEvents.onEndAlbum -= OnAlbumEnded;
        GameEvents.onReadySong -= OnReadySong;
        GameEvents.onStartSong -= OnStartSong;
        GameEvents.onCableConnectPlug -= OnCableConnected;
        GameEvents.onCableDisconnectPlug -= OnCableConnected;
    }


}
