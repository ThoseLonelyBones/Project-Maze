using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// WritingDirector is the funciton that governs the typing effect in game, as well as handling the correct management and thoroughput of tags in the game.
public class Writing_Director : MonoBehaviour
{
    // Textbox refers to any place you need to write text in.
    private TMP_Text Textbox;
    // Text is any text that needs to be written
    public string Text;
    // TTW, or time_to_write is the time spent between each character
    private float time_to_write;
    // I'm afraid to remove variables in the belif that the game will f*****g explode (pardon my French) if I do so.
    // I wouldn't trust VisualStudio with a 5-line script, and very much less so with the entire game.
    private float script_timer;
    
    // These two values are taken from PlayerPrefs and then used to display text
    int textsize;
    float textspeed;

    // Char Pos is used in the writing function
    private int char_pos;
    private Audio_Director audio_director;

    // This was a stupid attempt at getting tags to be erased by the game as they were being read. I don't need this anymore but I'm afraid I'll break stuff if I remove it.
    private bool invisibleCharacters;



    private void Awake()
    {   // Initialization

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

        // If there is a set textsize, set it to that textsize
        textsize = PlayerPrefs.GetInt("textsize");
        textspeed = PlayerPrefs.GetFloat("textspeed");

        if (textsize != 0)
        {
            Textbox.fontSize = textsize;
        }

    }

    private void Start()
    {
    }

    // TypingEffect gives a few settings before hand, such as specifying the box to write in, the text to start with, taking the text speed and size and then begins a Coroutine of Writing.
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

    // It's... pretty self explanatory. It stops the writing.
    public void WritingStop()
    {
        Textbox.text = "";
        Text = "";
    }

    // The Coroutine ran by Writing(), it types text on screen and waits for time to write between each character. Has an internal control metric of Text and skips tags via the tag_effect_bool. This also plays the typing sound.
    IEnumerator Writing()
    {
        bool play_typing_sound = true;
        bool tag_effect = false;
        string tag = "";

        // Conditions for the writing loop
        while (char_pos < Text.Length && Textbox.text != Text)
        {
            // If the sound ain't playing, play it. Otherwise, shut it.
            if(play_typing_sound)
            {
                audio_director.PlayGameSFX(1);
                play_typing_sound = false;
            }

            // If you spot the beginning of a tag, don't wait any time to write the characters just immediatly write all of them. In TMP, tags are deleted from the textbox and their effects applied once you write them out. As long as your PC isn't a
            // potato, this thing should work 100% of the time in deleting tags before they are seen. This makes it seem like the text you write is actually already having the intended effects.
            if(Text[char_pos] == '<' || tag_effect)
            {
                tag_effect = true;
                Textbox.text += Text[char_pos].ToString();
                tag += Text[char_pos].ToString();

                if(Text[char_pos] == '>')
                {
                    tag_effect = false;
                }
            }
            else
            {
                // This is just the regular writing. If there is no tag... write the characters normally (waiting for TTW each time)
                Textbox.text += Text[char_pos].ToString();
                yield return new WaitForSeconds(time_to_write);                     
            }

            char_pos++;
        }

        audio_director.StopGameSFX();
    }

    // Used to update the font size of the textbox on the next writing occasion
    public void UpdateSize()
    {
        Textbox.fontSize = PlayerPrefs.GetInt("textsize");
    }

}
