using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cables;

public class GameManager : MonoBehaviour
{

    [SerializeField] private HUDHappinessDisplay happinessHUD;
    [SerializeField] private TMPro.TextMeshProUGUI cassetteText;

    //How long the intermission between songs lasts for
    [SerializeField] private float intermissionTime;
    public float IntermissionTime { get => intermissionTime; }

    public song currentSong { get; set; }

    static public float timer { get; set; }

    public enum GameState { menu, intermission, playing }
    
    static public GameState currentGameState { get; set; }

    [SerializeField] private float startingHappiness;
    [SerializeField] private float minHappinessRate;
    [SerializeField] private float maxHappinessRate;
    static public float happinessRate { get; set; }
    public bool isStarted { get; set; }

    static private float m_happiness;
    static public float happiness { get => m_happiness; set { m_happiness = Mathf.Clamp(value, 0, 100); }}

    IDictionary<InstrumentSO, recipe> recipeDictionary;
    IDictionary<CableController, InstrumentSO> connectionsDictionary;

    List<recipe> completedRecipes = new List<recipe>();

    private AudioSource dummyDrumAS;

    // Update is called once per frame
    void Update()
    {
        //Decrementing timer
        if (currentGameState == GameState.intermission)
        {
            timer -= Time.deltaTime;

            float minutes = Mathf.Floor(timer / 60f);
            float seconds = Mathf.Ceil(timer % 60f);
            cassetteText.text = "Intermission" + "  " + minutes + ":" + seconds.ToString("00");

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
                //When song duration is up, call time up. Timer counts up instead of down to act as timer for song
                GameEvents.TimeUp();
                return;
            }


            //Interpolate happiness rate of change depending on current happiness
            float t = 1 - happiness / 100;
            float interpolatedHappinessRate;
            if (happinessRate < 0) 
            {
                interpolatedHappinessRate = Mathf.Lerp(happinessRate, 0, t);
            }
            else 
            {
                interpolatedHappinessRate = Mathf.Lerp(.2f, happinessRate, t);
            }
            happiness += interpolatedHappinessRate * Time.deltaTime;
            happinessHUD.SetHappiness(happiness);
            

            float minutes = Mathf.Floor(timer / 60f);
            float seconds = Mathf.Ceil(timer % 60f);
            cassetteText.text = currentSong.songName + "  " + minutes + ":" + seconds.ToString("00");


            // if (happiness <= 0)
            // {
            //     //When happiness reaches 0, we lose
            //     GameEvents.GameOver();
            //     return;
            // }
        }
    }

    private void Awake()
    {
        Debug.Log("HIII WHAT CAME FIRST?");
        recipeDictionary = new Dictionary<InstrumentSO, recipe>();
    }

    //Called when game scene is loaded. Initialize level
    private void OnGameStart()
    {
        currentGameState = GameState.intermission;
        timer = intermissionTime;
        happiness = startingHappiness;
        happinessRate = minHappinessRate;
        happinessHUD.SetHappiness(startingHappiness);

        float minutes = Mathf.Floor(intermissionTime / 60f);
        float seconds = Mathf.Ceil(intermissionTime % 60f);
        cassetteText.text = "Intermission" + "  " + minutes + ":" + seconds.ToString("00");

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
        isStarted = true;
        if (dummyDrumAS != null)
        {
            dummyDrumAS.clip = currentSong.drumTrack;
            dummyDrumAS.Play();
        }
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
        if (dummyDrumAS != null)
            dummyDrumAS.Stop();
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
        if(instrument == null)
        {
            print("no instrument");
            return;
        }
        if (recipeDictionary.TryGetValue(instrument, out recipe recipe))
        {
            //If matching recipe found with correct instrument
            int totalPluggables = recipe.midAffectors.Length + 2;
            if (cable.pluggablesList.Count == totalPluggables)
            {//First checks if size of list matches
                Debug.Log("Got here 1");
                if (cable.pluggablesList[0] == recipe.amp && cable.pluggablesList[totalPluggables - 1] == recipe.speaker) //Checks if amp and speaker are correct
                {
                    Debug.Log("Got here 2");
                    List<PluggablesSO> pluggables = new List<PluggablesSO>(cable.pluggablesList); //Makes duplicate of lists so operations can be done on it without affecting original
                    foreach(MidAffectorSuper midAffector in recipe.midAffectors)
                    {
                        Debug.Log("Got here 3");
                        if (!pluggables.Contains(midAffector))
                        {
                            Debug.Log("Got here 4");
                            GameEvents.RecipeBroken(recipe); //If does not contain, it must be a broken recipe
                            return;
                        }
                        else
                        {
                            //removes item if it does contain so it can't represent duplicates
                            Debug.Log("Got here 5");
                            pluggables.Remove(midAffector);
                        }
                    }
                    //If it hasn't been stopped, recipe must be complete.
                    print("GM Calling recipe complete");
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

    //When we complete a recipe
    private void OnRecipeCompleted(recipe recipe)
    {
        // a recipe has been completed
        print("GM OnRecipeCompleted function internal");
        completedRecipes.Add(recipe);
        happinessRate = EvaluateHappinessRate();
    }

    //When we break a recipe
    private void OnRecipeBroken(recipe recipe)
    {
        if (completedRecipes.Exists(x => x.instrument == recipe.instrument)) {
            // someone just unplugged a complete recipe
            happinessRate = EvaluateHappinessRate();
        }
    }

    private float EvaluateHappinessRate()
    {

        float difference = maxHappinessRate - minHappinessRate;
        float multiplier = completedRecipes.Count / recipeDictionary.Count;
        float increment = difference * multiplier;
        float finalRate = minHappinessRate + increment;
        Debug.Log(finalRate);
        return finalRate;

        // switch (recipesCompleted)
        // {
        //     case 5:
        //         return 10;
        //     case 4:
        //         return 6;
        //     case 3:
        //         return 3;
        //     case 2:
        //         return 1;
        //     case 1:
        //         return 0;
        //     case 0:
        //         return -1;
        //     case -1:
        //         Debug.LogError("We have completed a negative number of recipes, that shouldn't have happened!");
        //         return 0;
        //     default:
        //         return 0;
        // }
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
        GameEvents.onRecipeCompleted += OnRecipeCompleted;
        GameEvents.onRecipeBroken += OnRecipeBroken;
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

    private void Start()
    {
        try
        {
            dummyDrumAS = transform.GetChild(0).GetComponent<AudioSource>();
        }
        catch
        {
            print("GM Could not locate dummy drummer player");
        }
    }
}
