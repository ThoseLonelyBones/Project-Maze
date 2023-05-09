using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// MenuScript governs the MainMenu's behaviour and functions.
public class MenuScript : MonoBehaviour
{
    // Gameobjects related to button. This is used to make them disappear
    [SerializeField]
    public GameObject start, load, options, credits;
    
    // The four buttons present in the main title screen. This is used to give them functions
    [SerializeField]
    public Button startbtn, loadbtn, optionsbtn, creditsbtn;

    // UI Director is included because of its ability to make canvas elements appear and disappear
    [SerializeField]
    private UI_Director ui_director;

    // Information Handler, used in retriving and protecting secure data, such as savefiles and datafiles
    [SerializeField]
    private Information_Handler info_handler;

    // GameObject and TMP element for the text display, moved downwards compared to its position in the main gameplay loop.
    public GameObject textDisplay;
    public TextMeshProUGUI TextDisplay;

    // Additional Objects that have been added. These objects are actually saved and carried between scenes using the "DontDestroyOnLoad"
    [SerializeField]
    private GameObject AudioDirector, VisualDirector, OptionsMenu;

    // A way to remove the entire MainMenu when hopping into the options
    public GameObject MainMenu;

    private void Awake()
    {
        // AudioDirector and VisualDirector are not cloned again in the new Scene. Instead, they are carried over to save memory and mantain their use.
        DontDestroyOnLoad(AudioDirector);
        DontDestroyOnLoad(VisualDirector);
        
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialization

        ui_director = GetComponent<UI_Director>();
        info_handler = GetComponent<Information_Handler>();

        startbtn.onClick.AddListener(() => ClickStart());
        loadbtn.onClick.AddListener(() => ClickLoad());
        //Options's action is dependent on context and is directly assigned from the Editor.
        creditsbtn.onClick.AddListener(() => ClickCredits());

        Audio_Director audiodirector = AudioDirector.GetComponent<Audio_Director>();

        // If the game can't find user, settings, here are the default settings
        if(!info_handler.LoadSettings())
        {
            PlayerPrefs.SetFloat("music_volume", 1f);
            PlayerPrefs.SetFloat("game_sfx_volume", 1f);
            PlayerPrefs.SetFloat("scene_sfx_volume", 1f);
            PlayerPrefs.SetString("autosave", "true");
            PlayerPrefs.SetString("datacollection", "false");
            PlayerPrefs.SetInt("textsize", 46);
            PlayerPrefs.SetFloat("textspeed", 0.045f);
        }

        Settings(audiodirector);
        PlayerPrefs.SetInt("credits_scene", 0);

        // This function is the bane of me. It somehow makes the game double-up the music if you visit the Credits scene at least once. I reccomend quitting out the game when that happens.
        audiodirector.MainMenuMusic();

        Debug.Log(info_handler.LoadData());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Clicking Start loads you into the gameplay scene with no savefile loaded.
    void ClickStart()
    {
        PlayerPrefs.SetInt("load", 0);
        SceneManager.LoadScene("Gameplay Scene");     
    }

    // This checks whether or not you have a savefile available. If you don't, it throws an error. If you do, it instead gives a player preference of load = 1 to make sure the game assigns your data correctly on the main gameplays screen.
    void ClickLoad()
    {
        // This loads the game data and assigns it to a string. This is used exclusively for Datafiles.
        string playersavedata = info_handler.LoadGame();
        if(playersavedata == null)
        {
            TextDisplay.text = "The game incurred in an issue while loading your savefile. Make sure one is present or available!";
        }
        else
        {
            PlayerPrefs.SetString("savedata", playersavedata);
            PlayerPrefs.SetInt("load", 1);
            SceneManager.LoadScene("Gameplay Scene");
        }
       
    }
    
    // Sends you to the Credits Scene
    void ClickCredits()
    {
        SceneManager.LoadScene("Credits");
    }
    
    // Gets Audio_Settings working. This is used when audio is assigned at the start.
    void Settings(Audio_Director audiodirector)
    {
        audiodirector.music_volume = PlayerPrefs.GetFloat("music_volume");
        audiodirector.game_sfx_volume = PlayerPrefs.GetFloat("game_sfx_volume");
        audiodirector.scene_sfx_volume = PlayerPrefs.GetFloat("scene_sfx_volume");

    }
}
