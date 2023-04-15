using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Writing_Director : MonoBehaviour
{
    private TMP_Text Textbox;
    public string Text;
    private float time_to_write;
    private float script_timer;

    int textsize;
    float textspeed;

    private int char_pos;
    private Audio_Director audio_director;

    private bool invisibleCharacters;



    private void Awake()
    {
        Textbox = GameObject.Find("Text Display").GetComponent<TextMeshProUGUI>();

        GameObject audiodirector = GameObject.Find("AudioDirector");
        audio_director = audiodirector.GetComponent<Audio_Director>();
        if (audio_director == null)
        {
            Debug.Log("Audio Director not set!");
        }
        else
        {
            Debug.Log("Audio Director set!");
        }

        textsize = PlayerPrefs.GetInt("textsize");
        textspeed = PlayerPrefs.GetFloat("textspeed");

        if (textsize != null)
        {
            Textbox.fontSize = textsize;
        }

    }

    private void Start()
    {
    }

    public void TypingEffect(TMP_Text textbox, string newText)
    {
        textspeed = PlayerPrefs.GetFloat("textspeed");
        Textbox = textbox;
        Textbox.text = "";
        Textbox.fontSize = PlayerPrefs.GetInt("textsize");
        Text = newText;
        char_pos = 0;
        time_to_write = textspeed;
        StartCoroutine(Writing());
    }

    public void WritingStop()
    {
        Textbox.text = "";
        Text = "";
    }

    IEnumerator Writing()
    {
        bool play_typing_sound = true;

        while (char_pos < Text.Length && Textbox.text != Text)
        {
            if(play_typing_sound)
            {
                audio_director.PlayGameSFX(1);
                play_typing_sound = false;
            }

            Textbox.text += Text[char_pos].ToString();
            char_pos++;
            yield return new WaitForSeconds(time_to_write);                     // Need to make this work letter by letter (this writes more characters if the framerate allows it)
        }

        audio_director.StopGameSFX();
    }

    public void UpdateSize()
    {
        Textbox.fontSize = PlayerPrefs.GetInt("textsize");
    }
}
