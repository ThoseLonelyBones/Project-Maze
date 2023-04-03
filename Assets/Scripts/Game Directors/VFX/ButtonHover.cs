using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonHover : MonoBehaviour
{

    public Writing_Director writing_director;
    public TextMeshProUGUI TextDisplay;

    // Start is called before the first frame update
    void Start()
    {
        writing_director = GetComponent<Writing_Director>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void onButtonHover(string button_name)
    {
        string text_to_write = "";

        switch(button_name)
        {
            case "start_game":
                text_to_write = "Begin your adventure...";
                break;
            case "load_game":
                text_to_write = "Resume your adventure from the last save point.";
                break;
            case "options":
                text_to_write = "Change some settings and options of the game.";
                break;
            case "credits":
                text_to_write = "See who made this project possible!";
                break;
            case "default":
                text_to_write = "This required a lot of patience, now it requires an act of God. (Max0r, 2022)";
                break;
        }

        writing_director.TypingEffect(TextDisplay, text_to_write, 0.025f);

        //TextDisplay.text = text_to_write;
    }

    public void onButtonLeaveHover()
    {
        writing_director.WritingStop();
        //TextDisplay.text = "";
    }
}
