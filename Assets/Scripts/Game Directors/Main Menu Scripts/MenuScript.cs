using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuScript : MonoBehaviour
{
    [SerializeField]
    public GameObject start, load, options, credits;

    [SerializeField]
    public Button startbtn, loadbtn, optionsbtn, creditsbtn;

    [SerializeField]
    private UI_Director ui_director;

    [SerializeField]
    private Information_Handler info_handler;


    public GameObject textDisplay;
    public TextMeshProUGUI TextDisplay;

    [SerializeField]
    private GameObject AudioDirector, VisualDirector, OptionsMenu;

    public GameObject MainMenu;

    private void Awake()
    {
        DontDestroyOnLoad(AudioDirector);
        DontDestroyOnLoad(VisualDirector);
        // Find a way to make this persist, probably copying it and putting it in Gameplay Script
    }

    // Start is called before the first frame update
    void Start()
    {
        ui_director = GetComponent<UI_Director>();
        info_handler = GetComponent<Information_Handler>();

        startbtn.onClick.AddListener(() => ClickStart());
        loadbtn.onClick.AddListener(() => ClickLoad());
        //Options's action is dependent on context and is directly assigned from the Editor.
        creditsbtn.onClick.AddListener(() => ClickCredits());

        Audio_Director audiodirector = AudioDirector.GetComponent<Audio_Director>();

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

        audiodirector.MainMenuMusic();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ClickStart()
    {
        PlayerPrefs.SetInt("load", 0);
        SceneManager.LoadScene("Gameplay Scene");     
    }

    void ClickLoad()
    {
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

    void ClickCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    void Settings(Audio_Director audiodirector)
    {
        audiodirector.music_volume = PlayerPrefs.GetFloat("music_volume");
        audiodirector.game_sfx_volume = PlayerPrefs.GetFloat("game_sfx_volume");
        audiodirector.scene_sfx_volume = PlayerPrefs.GetFloat("scene_sfx_volume");

    }
}
