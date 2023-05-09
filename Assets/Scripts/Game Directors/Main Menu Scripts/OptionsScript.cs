using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

// This is the script that governs the options present in the game. This options script contains all operations that can be done in the options menu
public class OptionsScript : MonoBehaviour
{
    // Gameobjects that govern the entirity of the Optionsmenu or the current active gameplay scene (either the main menu or the gameplay)
    public GameObject OptionsMenu;
    public GameObject GameplayScene;

    // The various elements of the Options menu! The volume sliders, the toggles, etc.
    public Slider musicvolume, gamesfxvolume, scenesfxvolume;

    public Toggle autosave_toggle, datagathering_toggle;

    // Audio and Writing director are implemented to allow for easier changes (and also to write information in the options menu)
    private Audio_Director audio_director;
    [SerializeField]
    private Writing_Director writing_director;

    // Check the current active scene to know which elements to swap in the game.
    private Scene     ActiveScene;

    private string slidername;

    // TMP text elements of the menu
    public TMP_Text musicvolume_text, gamesfx_text, scenesfx_text, options_text, textspeed_button, textsize_button;

    // Used to update the menu when it is closed and open
    private bool update_menu = false;


    // Initialization and Menu Update
    void Start()
    {
       
        OptionsMenu = GameObject.Find("OptionsMenu");
        OptionsMenu.SetActive(false);


        ActiveScene = SceneManager.GetActiveScene();
        if(ActiveScene.name == "Main Menu")
        {
            GameplayScene = GameObject.Find("GameMenu");
        }
        else
        {
            GameplayScene = GameObject.Find("GameplayScene");
        }


        GameObject audio_director_object = GameObject.Find("AudioDirector");
        audio_director = audio_director_object.GetComponent<Audio_Director>();

        UpdateMenu();

    }

    // Update is called once per frame
    void Update()
    {
        // Simply put, check if update menu changes. If it does, do UpdateMenu() once.
        if(update_menu)
        {
            UpdateMenu();
            update_menu = false;
        }
    }

    // When the menu is opened... or closed... it either activates itself and disables the rest of the game or does the opposite. This also updates the menu.
    public void OptionsMenuOpen()
    {
        if(!OptionsMenu.activeSelf)
        {
            OptionsMenu.SetActive(true);
            GameplayScene.SetActive(false);
            update_menu = true;
        }
        else
        {
            OptionsMenu.SetActive(false);
            GameplayScene.SetActive(true);
            update_menu = true;
        }

        
    }

    // Simple volume sliders. This bit of code manages to keep a single function that changes its target based on which slider is being used.
    public void VolumeSlider(float value)
    {
        Slider slider = (Slider)UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent(typeof(Slider));

        switch (slider)
        {
            case Slider _ when slider == musicvolume:
                //audio_director.music_volume = value;
                PlayerPrefs.SetFloat("music_volume", value);
                int display_value_1 = Mathf.FloorToInt((value * 100));
                musicvolume_text.text = display_value_1.ToString();
                break;
            case Slider _ when slider == gamesfxvolume:
                //audio_director.game_sfx_volume = value;
                PlayerPrefs.SetFloat("game_sfx_volume", value);
                int display_value_2 = Mathf.FloorToInt((value * 100));
                gamesfx_text.text = display_value_2.ToString();
                break;
            case Slider _ when slider == scenesfxvolume:
                //audio_director.scene_sfx_volume = value;
                PlayerPrefs.SetFloat("scene_sfx_volume", value);
                int display_value_3 = Mathf.FloorToInt((value * 100));
                scenesfx_text.text = display_value_3.ToString();
                break;
            default:
                break;
        }
    }

    // Every time you click the button it changes the game's speed to a different level of speed. Easy as that.
    public void TextSpeed()
    {
        float textspeed = PlayerPrefs.GetFloat("textspeed");
        switch(textspeed)
        {
            case 0.020f:
                PlayerPrefs.SetFloat("textspeed", 0.075f);
                writing_director.TypingEffect(options_text, "Text speed set to Slow");
                textspeed_button.text = "Text Speed: SLOW";
                break;
            case 0.045f:
                PlayerPrefs.SetFloat("textspeed", 0.020f);
                writing_director.TypingEffect(options_text, "Text speed set to Fast");
                textspeed_button.text = "Text Speed: FAST";
                break;
            case 0.075f:
                PlayerPrefs.SetFloat("textspeed", 0.045f);
                writing_director.TypingEffect(options_text, "Text speed set to Standard");
                textspeed_button.text = "Text Speed: STANDARD";
                break;
            default:
                PlayerPrefs.SetFloat("textspeed", 0.045f);
                writing_director.TypingEffect(options_text, "Text speed set to Standard");
                textspeed_button.text = "Text Speed: STANDARD";
                break;
        }
            
    }

    // Same thing as textspeed but for the text size.
    public void TextSize()
    {
        int textsize = PlayerPrefs.GetInt("textsize");
        switch(textsize)
        {
            case 46:
                PlayerPrefs.SetInt("textsize", 58);
                writing_director.UpdateSize();
                writing_director.TypingEffect(options_text, "Text size set to Large");
                textsize_button.text = "Text Size: LARGE";
                break;
            case 58:
                PlayerPrefs.SetInt("textsize", 46);
                writing_director.UpdateSize();
                writing_director.TypingEffect(options_text, "Text size set to Standard");
                textsize_button.text = "Text Size: Standard";
                break;
            default:
                PlayerPrefs.SetInt("textsize", 46);
                writing_director.UpdateSize();
                writing_director.TypingEffect(options_text, "Text size set to Standard");
                textsize_button.text = "Text Size: Standard";
                break;
        }
    }

    // Either the autosave is on, and then gets disabled or it is disabled and becomes enabled (this function is on toggle change)
    public void AutoSave()
    {
        string autosave = PlayerPrefs.GetString("autosave");
        if(autosave == "true")
        {
            PlayerPrefs.SetString("autosave", "false");
            writing_director.TypingEffect(options_text, "Autosave is Disabled");
        }
        else
        {
            PlayerPrefs.SetString("autosave", "true");
            writing_director.TypingEffect(options_text, "Autosave is Enabled");
        }
    }

    // Either data collection is on, and then gets disabled or it is disabled and becomes enabled (this function is on toggle change)
    public void DataCollection()
    {
        string datacollection = PlayerPrefs.GetString("datacollection");
        if (datacollection == "true")
        {
            PlayerPrefs.SetString("datacollection", "false");
            writing_director.TypingEffect(options_text, "Data Collection is Disabled");
        }
        else
        {
            PlayerPrefs.SetString("datacollection", "true");
            writing_director.TypingEffect(options_text, "Data Collection is Enabled");
        }
    }

    // Simply return to the main menu. Stop the music so it doesn't double up when you return there.
    public void ReturntoMainMenu()
    {
        audio_director.music_audio_source.Stop();
        SceneManager.LoadScene("Main Menu");
    }

    // Menu's update takes all the PLayerPrefs and then sets each element of the menu to their current playerpref value. This keeps the options as consistent as possible.
    public void UpdateMenu()
    {
        musicvolume.value = audio_director.music_volume;
        int display_value_1 = Mathf.FloorToInt((musicvolume.value * 100));
        musicvolume_text.text = display_value_1.ToString();

        gamesfxvolume.value = audio_director.game_sfx_volume;
        int display_value_2 = Mathf.FloorToInt((gamesfxvolume.value * 100));
        gamesfx_text.text = display_value_2.ToString();

        scenesfxvolume.value = audio_director.scene_sfx_volume;
        int display_value_3 = Mathf.FloorToInt((scenesfxvolume.value * 100));
        scenesfx_text.text = display_value_3.ToString();

        musicvolume.onValueChanged.AddListener(VolumeSlider);
        gamesfxvolume.onValueChanged.AddListener(VolumeSlider);
        scenesfxvolume.onValueChanged.AddListener(VolumeSlider);

        float textspeed = PlayerPrefs.GetFloat("textspeed");
        switch (textspeed)
        {
            case 0.020f:
                textspeed_button.text = "Text Speed: FAST";
                break;
            case 0.045f:
                textspeed_button.text = "Text Speed: STANDARD";
                break;
            case 0.075f:
                textspeed_button.text = "Text Speed: SLOW";
                break;
            default:
                textspeed_button.text = "Text Speed: STANDARD";
                break;
        }

        int textsize = PlayerPrefs.GetInt("textsize");
        switch (textspeed)
        {
            case 48:
                textsize_button.text = "Text Size: STANDARD";
                break;
            case 54:
                textsize_button.text = "Text Size: LARGE";
                break;
            default:
                textsize_button.text = "Text Size: STANDARD";
                break;
        }

        string datacollection = PlayerPrefs.GetString("datacollection");
        if (datacollection == "true")
        {
            datagathering_toggle.isOn = true;
        }
        else
        {
            datagathering_toggle.isOn = false;
        }

        string autosave = PlayerPrefs.GetString("autosave");
        if (autosave == "true")
        {
            autosave_toggle.isOn = true;
        }
        else
        {
            autosave_toggle.isOn = false;
        }
    }
}
