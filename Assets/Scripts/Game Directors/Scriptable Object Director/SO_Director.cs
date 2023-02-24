using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/* 
 * The SO_Director (or Scriptable Object Director) is the script which governs the distribution and handling of information contained in Scriptable Object to other parts of the code.
 * The SO_Director is called by the GameDirector whenever information from a Scriptable Object needs to be put in the game, will it be from a Scenario or from a Dialogue.
 * While the SO_Director is able to communicate with all the Scriptable Objects, it doesn't do so in a specific order: instead, it is a collection of generalized functions that allow for game scalability
 * The SO_Director makes use of Scriptable Object to deconstruct, reformat and present the information contained in them in the clearest and most obvious way possible to the player, as well as allowing for:
 * 
 * Increased flexibility in the Game Director Script and for CONSIDERABLY less clutter (just compare the old Game Director script from the "The Guardsman's Stand" Demonstration to the new one!)
 * 
 * Ease of understanding of the code for any party involved, as well as providing IMMENSE scalabiity: the older code had a limited amount of actions, and each action was MANUALLY HARDCODED IN...
 * 
 *      [...] the all-caps of the previous sentence should let you know how bad it was. To be both the devil and the advocate, I had one week to learn Unity and create a fully-fledged Text-Adventure Demo
 *      [...] now, this code is MUCH more effective for scaling upwards as well as adding new scenarios or dialogue to the game. As long as I can add Scriptable Objects, I can do anything! ~ muahahahah ~
 *      
 * Creates a unique script that is called by whatever is needed, instead of having to load the Scriptable Object TWICE or more into memory. The Scriptable Object is called in here ONCE, and only the
 *      [...] current one. The ScenarioIndex variable in the GameDirector Script helps find which script to load.
 */



public class SO_Director : MonoBehaviour
{
    public MainGameplayLoop loop;
    public Writing_Director writing_director;
    /*
     * Progression Elements are loaded only when needed, only a maximum of two Scenes are loaded at once: SOT_Scene and SOT_Scenario.
     */
    public SOT_Scene current_scene;

    public string text_to_write;

    /*
     * Variables relative to the Dialogue-within-Scenarios handling, used by ReadFlag, case a.
     */
    public SOT_Scenario alternate_scenario = null;
    public int alternate_index;
    private int alternate_exit_index;
    private int alt_scenario_count = 0;


    /*
     * GameObject and Text still need to be accessed, even with the TextDirector script actually putting in the text. (Or... we don't. Not if we make a function that specifically writes to buttons, called ButtonWrite)
     */

    public GameObject   button1,
                        button2,
                        button3,
                        button4;

    public GameObject textDisplay;

    public TextMeshProUGUI   button1Text,
                             button2Text,
                             button3Text,
                             button4Text;

    public TextMeshProUGUI TextDisplay;

    public int exit_index = 0;
    private int[] response = { 0, 0, 0, 0 };

    /*  
     *  CallExits(int scene_index) is a function that returns an Int array of values, exit[], which contains (in order of buttons) the IDs that their respective button will create the text for.
     *  To check which buttons to activate and to check what text is required to be placed on each button, it takes the current_scenario's scenario_exits string[], which is formatted like this:
     *  
     *      [x1,x2,x3,x4][b1,b2,b3,b4]
     *  
     *  The first square bracket contains the ID value of the text each button is assigned to, as shown in the scene_exits variable in the SOT_Scene.cs script (line 62)
     *  These values are also subsequently converted from strings to integer and then passed as an int[] to the GameDirector script.
     *  The second square bracket contains the ID value of the text inside the respective button, as shown in the exit_text variable in the SOT_Scenario.cs script (line 56)
     *  These values are not returned to the GameDirector function. Instead, they are passed to the TextDirector script to be formatted correctly and then subsequently put inside each button.
     *  (... As of now the text simply appears in the text boxes with no formatting ...)
     * 
     */

    private void Awake()
    {
        Debug.Log("Awaking SO_Director");

        loop = GetComponent<MainGameplayLoop>();

        if (loop == null)
        {
            Debug.Log("Main Gameplay Loop is unreachable");
        }
        else
        {
            Debug.Log("Main Gameplay Loop is reachable");
        }

        writing_director = GetComponent<Writing_Director>();

        if(writing_director == null)
        {
            Debug.Log("Writing Director is off");
        }
        else
        {
            Debug.Log("Writing Director is on");
        }

        button1 = GameObject.Find("Button 1");
        button2 = GameObject.Find("Button 2");
        button3 = GameObject.Find("Button 3");
        button4 = GameObject.Find("Button 4");

        if(button1 == null || button2 == null || button3 == null || button4 == null)
        {
            Debug.Log("Buttons are not available?");
        }
        else
        {
            Debug.Log("Buttons are available!");
        }

        button1Text = button1.GetComponentInChildren<TextMeshProUGUI>();
        button2Text = button2.GetComponentInChildren<TextMeshProUGUI>();
        button3Text = button3.GetComponentInChildren<TextMeshProUGUI>();
        button4Text = button4.GetComponentInChildren<TextMeshProUGUI>();

        

        if (button1Text == null || button2Text == null || button3Text == null || button4Text == null)
        {
            Debug.Log("Buttons text is not available?");
        }
        else
        {
            Debug.Log("Button text is available!");
        }

        textDisplay = GameObject.Find("Text Display");
        TextDisplay = textDisplay.GetComponent<TextMeshProUGUI>();

        if(textDisplay != null)
        {
            if(TextDisplay == null)
            {
                Debug.Log("Object available, text unavailable?");
            }
            else
            {
                Debug.Log("Text Display is available!");
            }
        }
        else
        {
            Debug.Log("Object Unavailable?");               
        }
    }

    public int[] CallExits()
    {
        int[] exits_array = { 0, 0, 0, 0 };

        string[] current_scene_exits = current_scene.scene_exits;

        Debug.Log(current_scene_exits[0]);

        string exits_editing = current_scene_exits[exit_index];            // => exits_editing = [11,12,13,14][1,2,3,4]
        // Step 1 = Merge '][' into ','                                  
        exits_editing = exits_editing.Replace("][", ",");                   // => exits_editing = [11,12,13,14,1,2,3,4]
        // Step 2 = Delete "[" and "]"
        exits_editing = exits_editing.Replace("[", "");                     // => exits_editing = 11,12,13,14,1,2,3,4]
        exits_editing = exits_editing.Replace("]", "");                     // => exits_editing = 11,12,13,14,1,2,3,4
        // Step 3 = Split the String by "," -> The first 4 values are the value of exit_text (the button's text)
        //                                  -> The last 4 values  are the Button values (exits[], the return integer array)                                  
        string[] exits_formatted = exits_editing.Split(",");
        // Step 4 = exits_formatted is now usable in the next for loop! The example above now looks something like this:
        //
        //  exits_formatted[0] = 11; exits_formatted[3] = 14; exits_formatted[4] = 1; exits_formatted[7] = 4 
        // However this is still a string. So it still needs to be formatted a bit to actually work, as seen below using int.Parse
        Debug.Log(exits_formatted);

        for (int x = 0; x < 4; x++)
        {
           response[x] = int.Parse(exits_formatted[x]);
           exits_array[x] = int.Parse(exits_formatted[x + 4]);
            Debug.Log("Exit Array at Pos:" + x + "is: " + exits_array[x]);
           if(exits_array[x] > 0)
           {
                switch(x)
                {
                    case 0:
                        // Activate Button 1;
                        button1Text.text = "";                                                              // => Replace this with TextDirector's CleanseText Function when done!
                        button1.SetActive(true);
                        button1Text.text = current_scene.exit_text[int.Parse(exits_formatted[x])];      // => Replace this with TextDirector's Writing Function when done!
                        break;
                    case 1:
                        // Activate Button 2;
                        button2Text.text = "";                                                              // => Replace this with TextDirector's CleanseText Function when done!
                        button2.SetActive(true);
                        button2Text.text = current_scene.exit_text[int.Parse(exits_formatted[x])];      // => Replace this with TextDirector's Writing Function when done!
                        break;
                    case 2:
                        // Activate Button 3;
                        button3Text.text = "";                                                              // => Replace this with TextDirector's CleanseText Function when done!
                        button3.SetActive(true);
                        button3Text.text = current_scene.exit_text[int.Parse(exits_formatted[x])];      // => Replace this with TextDirector's Writing Function when done!
                        break;
                    case 3:
                        // Active Button 4;
                        button4Text.text = "";                                                               // => Replace this with TextDirector's CleanseText Function when done!
                        button4.SetActive(true);
                        button4Text.text = current_scene.exit_text[int.Parse(exits_formatted[x])];          // => Replace this with TextDirector's Writing Function when done!
                        break;
                    default:
                        Debug.Log("LonelyBones has made the advancement [How Did We Get Here?]");
                        break;
                }

           }
        }

        exit_index++;
        return exits_array;

    }

    /*
     * SceneFlags(int scene_inedex) is a function that allows the SO_Director to progress the current scene based on what the current scene is. This function is called when the current main_display has finished
     * writing the text of the scene by reference from the Main Gameplay Loop script. This function takes in the current index, and uses it to check the scene_flags[] array within a given Scene, giving out
     * different outputs based on the result and changing the progression of the game! This allows to for example have text that doesn't prompt an immediate response, or doesn't require certain text blocks to be
     * enormous slogs that require multiple textboxes to be read. It allows for flexibility, variety and complexity within the game without using a boilerplate format of Text -> Button 100% of the time.
     * 
     * This gives out the following results based on the following indexes:
     * 
     * a: alternate -> Load the dialogue based on dialogue-index, a local variable exclusive to Scenarios, whilst saving the current Scenario and Index. This is used later to recover the dialogue when this flag is called again -> if this flag is called in a dialogue, return to the saved scenario.
     * b: buttons -> Load CallExits[]
     * c: continue -> Load next Text[]
     * d: default -> default flag, does nothing. Don't use this!
     * r: return -> used exclusively as a way to tell the game that the current scene is over and to move over to the next one
     * 
     * Additional Flags:
     * 
     * f: fish -> used in conjunction with hook. Once called, the scene is reset to what the hook saved.
     * h: hook -> save the current index and when "fish" is called retrieve that scene. Used for Dead Ends.
     * 
     * (possible implementation) m: mood -> used to determine the Observer's mood. Possibly influences some dialogue choices.
     * 
     * s: sound -> used to scour for sound clips and add a sound effect to the clip. Search the BBC network for loads of soundclips.
     * t: timer -> used to decrease the attempt timer.
     * 
     * Other steps that need to be undertaken are done in the Main Gameplay Loop script
     */

    public char SceneFlags(int scene_index)
    {
        if (scene_index + 1 == current_scene.text.Length && alternate_scenario == null)
        {
            Debug.Log("Returning Flag R");
            exit_index = 0;
            return 'r';        
        }
        else
        {
            switch (current_scene.scene_flags[scene_index])
            {
                case 'a':
                    if (alternate_scenario == null && current_scene is SOT_Scenario)
                    {
                        try
                        {
                            alternate_scenario = (SOT_Scenario)current_scene;
                            alternate_index = scene_index + 1;
                            alternate_exit_index = exit_index;
                            exit_index = 0;
                            current_scene = alternate_scenario.scenario_dialogues[alt_scenario_count];
                            ChangeScenarioText(0);
                        }
                        catch (InvalidCastException)
                        {
                            Debug.Log("Well you got me: by all accounts, this doesn't make sense.");
                        }
                    }
                    else
                    {
                        if (current_scene = alternate_scenario.scenario_dialogues[alt_scenario_count])
                        {
                            current_scene = alternate_scenario;
                            scene_index = alternate_index;
                            exit_index = alternate_exit_index;
                            ChangeScenarioText(scene_index);

                            alternate_scenario = null;
                        }
                    }
                    
                    return 'a';
                case 'b':
                    // Call Exits is called on the main gameplay loop (needs the int[] return to work)
                    return 'b';
                case 'c':
                    // Change Scenario Text -> this prompts into a change of the index.
                    ChangeScenarioText(scene_index + 1);
                    return 'c';
                default:
                    // Return 'd', prompt error message on the d
                    return 'd';
            }
        }
    }

    public int ChangeScene(SOT_Scene newscene)
    {
        if(newscene != null)
        {
            current_scene = newscene;
        }
        else
        {
            Debug.Log("This is wrong...");
        }
        
        return 0;
    }

    // This gets out of bounds!
    public string ChangeScenarioText(int id)
    {
        TextDisplay.text = "";
        //TextDisplay.text = current_scene.text[id];                                                       // => Replace this with TextDirector's Writing Function when done!
        text_to_write = current_scene.text[id];
        writing_director.TypingEffect(TextDisplay, text_to_write, 0.045f);
        return TextDisplay.text;
    }

    // Dialogue Response

    public string DialogueResponse(string button_name)
    {
        int x = 4;

        switch(button_name)
        {
            case "Button 1":
                Debug.Log("Button 1 was pressed!");
                x = 0;
                break;
            case "Button 2":
                Debug.Log("Button 2 was pressed!");
                x = 1;
                break;
            case "Button 3":
                Debug.Log("Button 3 was pressed!");
                x = 2;
                break;
            case "Button 4":
                Debug.Log("Button 4 was pressed!");
                x = 3;
                break;
            default:
                Debug.Log("This shouldn't be happening?");
                break;
        }

        try
        {
            SOT_Dialogue dialogue = (SOT_Dialogue)current_scene;
            TextDisplay.text = "";
            if(x < 4)
            {
                //TextDisplay.text = dialogue.responses[response[x]];                                         // => Replace this with TextDirector's Writing Function when done!
                text_to_write = dialogue.responses[response[x]];
                writing_director.TypingEffect(TextDisplay, text_to_write, 0.045f);
                Debug.Log(dialogue.responses[response[x]]);
                return dialogue.responses[response[x]];
            }
            else
            {
                return "";
            }
        }
        catch (InvalidCastException)
        {
            Debug.Log("Well you got me: by all accounts, this doesn't make sense.");
            return "";
        }
        
    }

}
