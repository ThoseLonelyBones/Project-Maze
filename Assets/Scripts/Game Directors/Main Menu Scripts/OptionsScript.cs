using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class OptionsScript : MonoBehaviour
{
    public GameObject OptionsMenu;
    public GameObject GameplayScene;

    public Slider musicvolume, gamesfxvolume, scenesfxvolume;

    public Toggle autosave_toggle, datagathering_toggle;

    private Audio_Director audio_director;

    [SerializeField]
    private Writing_Director writing_director;

    private Scene     ActiveScene;

    private string slidername;

    public TMP_Text musicvolume_text, gamesfx_text, scenesfx_text, options_text, textspeed_button, textsize_button;


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

        musicvolume.value = audio_director.music_volume;
        musicvolume_text.text = (musicvolume.value * 100).ToString("0.00");

        gamesfxvolume.value = audio_director.game_sfx_volume;
        gamesfx_text.text = (gamesfxvolume.value * 100).ToString("0.00");

        scenesfxvolume.value = audio_director.scene_sfx_volume;
        scenesfx_text.text = (scenesfxvolume.value * 100).ToString("0.00");

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
        if(datacollection == "true")
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

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OptionsMenuOpen()
    {
        if(!OptionsMenu.activeSelf)
        {
            OptionsMenu.SetActive(true);
            GameplayScene.SetActive(false);
        }
        else
        {
            OptionsMenu.SetActive(false);
            GameplayScene.SetActive(true);
        }

        
    }

    public void VolumeSlider(float value)
    {
        Slider slider = (Slider)UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent(typeof(Slider));

        switch (slider)
        {
            case Slider _ when slider == musicvolume:
                //audio_director.music_volume = value;
                PlayerPrefs.SetFloat("music_volume", value);
                musicvolume_text.text = (value * 100).ToString("0.00");
                break;
            case Slider _ when slider == gamesfxvolume:
                //audio_director.game_sfx_volume = value;
                PlayerPrefs.SetFloat("game_sfx_volume", value);
                gamesfx_text.text = (value * 100).ToString("0.00");
                break;
            case Slider _ when slider == scenesfxvolume:
                //audio_director.scene_sfx_volume = value;
                PlayerPrefs.SetFloat("scene_sfx_volume", value);
                scenesfx_text.text = (value * 100).ToString("0.00");
                break;
            default:
                break;
        }
    }

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

    public void TextSize()
    {
        int textsize = PlayerPrefs.GetInt("textsize");
        switch(textsize)
        {
            case 48:
                PlayerPrefs.SetInt("textsize", 54);
                writing_director.UpdateSize();
                writing_director.TypingEffect(options_text, "Text size set to Large");
                textsize_button.text = "Text Size: LARGE";
                break;
            case 54:
                PlayerPrefs.SetInt("textsize", 48);
                writing_director.UpdateSize();
                writing_director.TypingEffect(options_text, "Text size set to Standard");
                textsize_button.text = "Text Size: Standard";
                break;
            default:
                break;
        }
    }

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

}
