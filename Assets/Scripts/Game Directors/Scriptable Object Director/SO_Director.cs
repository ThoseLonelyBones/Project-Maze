using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;


/* 
 * The SO_Director (or Scriptable Object Director) is the script which governs the distribution and handling of information contained in Scriptable Object to other parts of the code.
 * The SO_Director is called by the GameDirector whenever information from a Scriptable Object needs to be put in the game, will it be from a Scenario or from a Dialogue.
 * While the SO_Director is able to communicate with all the Scriptable Objects, it doesn't do so in a specific order or by recovering data via hardcoded SO identifiers or functions: instead, it is a collection of generalized functions that allow for game scalability
 * The SO_Director makes use of Scriptable Object to deconstruct, reformat and present the information contained in them in the clearest and most obvious way possible to the player, as well as allowing for:
 * 
 *      Increased flexibility in the Gameplay_Director Script
 *      ONSIDERABLY less clutter (just compare the old Game Director script from the "The Guardsman's Stand" Demonstration to the new one and remember this game is INFINITELY bigger!)
 *      Ease of understanding of the code for any party involved, as well as providing IMMENSE scalabiity: the older code had a limited amount of actions, and each action was MANUALLY HARDCODED IN...
 *         "
 *          [...] the all-caps of the previous sentence should let you know how bad it was. To be both the devil and the advocate, I had one week to learn Unity and create a fully-fledged Text-Adventure Demo
 *          [...] now, this code is MUCH more effective for scaling upwards as well as adding new scenarios or dialogue to the game. As long as I can add Scriptable Objects, I can do anything! ~ OHOHOHohohoh <3 ~ "
 *                                                                                                                                                                                                                -LonelyBones
 *      
 * Creates a unique script that is called by whatever is needed, instead of having to load the Scriptable Object TWICE or more into memory. The Scriptable Object is called in here ONCE, and only the
 *      [...] current one. The ScenarioIndex variable in the GameDirector Script helps find which script to load.
 */



public class SO_Director : MonoBehaviour
{
    /*
     * Directors! These are called in here to use their functions when needed (such as the writing director for putting text on the screen)
     * 
     */
    public Gameplay_Director loop;
    public Writing_Director writing_director;
    /*
     * Progression Elements are loaded only when needed, only a maximum of two Scenes are loaded at once: current_scene and alternate_scenario (look further below for more info)
     */
    public SOT_Scene current_scene;
    /*
     * This is used as a control text in the gameplay_director as well as an import for the writing_director to know what to write in the main text box. I could have not made it a variable but
     * writing so_director.current_scene.text[index] strained my fingers after a while (aaaaaand it also didn't work with responses, figures!)
     */
    public string text_to_write;

    /*
     * Variables relative to the Dialogue-within-Scenarios handling, used by ReadFlag, case a.
     * There are a few very important things to save here, which are going to be used later on, such as:
     *  - The scenario we were in.                 (Actually, do we have to save this? Can't we just retrieve this again from the main Gameplay_Director? Look into it)
     *  - The index we were at.
     *  - The Exit Index we were at (very important!)
     *  - The current index of the scenario dialgoues (alt_scenario_count)
     */ 
    public SOT_Scenario alternate_scenario = null;
    public int alternate_index;
    private int alternate_exit_index;
    private int alt_scenario_count = 0;


    /*
     * GameObject and Text still need to be accessed, even with the TextDirector script actually putting in the text.
     * (Or... we don't. Not if we make a function that specifically writes to buttons, called ButtonWrite) <- Buttons are not written in, instead they spawn with a unique animation (look at VFX_Director for more info on ButtonSpawn)
     */

    public GameObject   button1,
                        button2,
                        button3,
                        button4;

    public GameObject textDisplay;

    /*
     * 
     * The text elements used to assign text to buttons and the text display! TextMeshProUGUI is actually super cool and allows you to do so many weird effects with text! It's great!
     * 
     */

    public TextMeshProUGUI   button1Text,
                             button2Text,
                             button3Text,
                             button4Text;

    public TextMeshProUGUI TextDisplay;

    // Two elements used in conjuction with the callexits and response handler functions 
    public int exit_index = 0;
    [SerializeField]
    public int hook_exit_index = 0;
    private int[] response = { 0, 0, 0, 0 };

    public int input_index = 0;


    // Awake is a good point to assing all elements where needed, to check if the various objects in the game and other scripts are reachable, etc.
    private void Awake()
    {
        Debug.Log("Scriptable Object Director Start-Up");

        loop = GetComponent<Gameplay_Director>();

        if (loop == null)
        {
            Debug.Log("SO_Director > Gameplay Director: X");
        }
        else
        {
            Debug.Log("SO_Director > Gameplay Director: V");
        }

        writing_director = GetComponent<Writing_Director>();

        if(writing_director == null)
        {
            Debug.Log("SO_Director > Writing Director: X");
        }
        else
        {
            Debug.Log("SO_Director > Writing Director: V");
        }

        button1 = GameObject.Find("Button 1");
        button2 = GameObject.Find("Button 2");
        button3 = GameObject.Find("Button 3");
        button4 = GameObject.Find("Button 4");

        if(button1 == null || button2 == null || button3 == null || button4 == null)
        {
            Debug.Log("SO_Director > Buttons: X");
        }
        else
        {
            Debug.Log("SO_Director > Buttons: V");
        }

        button1Text = button1.GetComponentInChildren<TextMeshProUGUI>();
        button2Text = button2.GetComponentInChildren<TextMeshProUGUI>();
        button3Text = button3.GetComponentInChildren<TextMeshProUGUI>();
        button4Text = button4.GetComponentInChildren<TextMeshProUGUI>();

        

        if (button1Text == null || button2Text == null || button3Text == null || button4Text == null)
        {
            Debug.Log("SO_Director > Button Text: X");
        }
        else
        {
            Debug.Log("SO_Director > Button Text: V");
        }

        textDisplay = GameObject.Find("Text Display");
        TextDisplay = textDisplay.GetComponent<TextMeshProUGUI>();

        if(textDisplay != null)
        {
            if(TextDisplay == null)
            {
                Debug.Log("SO_Director > Text Display Object: V, Text Display Text: X");
            }
            else
            {
                Debug.Log("SO_Director > Text Display Object: V, Text Display Text: V");
            }
        }
        else
        {
            Debug.Log("SO_Director > Text Display Object: X, Text Display Text: X");               
        }
    }

    /*  
    *  CallExits(int scene_index) is a function that returns an Int array of values, exit[], which contains (in order of buttons) the IDs that their respective button will create the text for.
    *  To check which buttons to activate and to check what text is required to be placed on each button, it takes the current_scenario's scenario_exits string[], which is formatted like this:
    *  
    *      [x1,x2,x3,x4][b1,b2,b3,b4]
    *  
    *  The first square bracket contains the ID value of the text inside the respective button, as shown in the exit_text variable in the SOT_Scenario.cs script (line 56)
    *  These values are not returned to the GameDirector function. Instead, they are passed to the TextDirector script to be formatted correctly and then subsequently put inside each button.
    *  The second square bracket contains the ID value of the text each button is assigned to, as shown in the scene_exits variable in the SOT_Scene.cs script (line 62)
    *  These values are also subsequently converted from strings to integer and then passed as an int[] to the GameDirector script.
    * 
    */

    public int[] CallExits()
    {
        int[] exits_array = { 0, 0, 0, 0 };

        string[] current_scene_exits = current_scene.scene_exits;

        Debug.Log(current_scene_exits[0]);

        string exits_editing = current_scene_exits[exit_index];            // => exits_editing = [11,12,13,14][1,2,3,4][0]
        // Step 1 = Merge '][' into ','                                  
        exits_editing = exits_editing.Replace("][", ",");                   // => exits_editing = [11,12,13,14,1,2,3,4,0]
        // Step 2 = Delete "[" and "]"
        exits_editing = exits_editing.Replace("[", "");                     // => exits_editing = 11,12,13,14,1,2,3,4,0]
        exits_editing = exits_editing.Replace("]", "");                     // => exits_editing = 11,12,13,14,1,2,3,4,0
        // Step 3 = Split the String by "," -> The first 4 values are the value of exit_text (the button's text)
        //                                  -> The last 4 values  are the Button values (exits[], the return integer array)
        //                                  -> The last value is the exit index addendum, which determines how much the exit array is supposed to increase whenever it is a value different than zero.
        //                                     If it is 0, then Exit_array increases by only 1.
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
                        button1Text.text = "";                                                              // => Replace this with VFX_Director's ButtonSpawn Function when done!
                        //button1.SetActive(true);
                        button1Text.text = current_scene.exit_text[int.Parse(exits_formatted[x])];      // => Replace this with VFX_Director's ButtonSpawn Function when done!
                        break;
                    case 1:
                        // Activate Button 2;
                        button2Text.text = "";                                                              // => Replace this with VFX_Director's ButtonSpawn  Function when done!
                        //button2.SetActive(true);
                        button2Text.text = current_scene.exit_text[int.Parse(exits_formatted[x])];      // => Replace this with VFX_Director's ButtonSpawn Function when done!
                        break;
                    case 2:
                        // Activate Button 3;
                        button3Text.text = "";                                                              // => Replace this with VFX_Director's ButtonSpawn  Function when done!
                        //button3.SetActive(true);
                        button3Text.text = current_scene.exit_text[int.Parse(exits_formatted[x])];      // => Replace this with VFX_Director's ButtonSpawn Function when done!
                        break;
                    case 3:
                        // Active Button 4;
                        button4Text.text = "";                                                               // => Replace this with VFX_Director's ButtonSpawn Function when done!
                        //button4.SetActive(true);
                        button4Text.text = current_scene.exit_text[int.Parse(exits_formatted[x])];          // => Replace this with VFX_Director's ButtonSpawn  Function when done!
                        break;
                    default:
                        Debug.Log("LonelyBones has made the advancement [How Did We Get Here?]");
                        break;
                }

           }
        }

        try
        {
            int exits_addendum = int.Parse(exits_formatted[8]);

            if (int.Parse(exits_formatted[8]) > 0)
            {
                exit_index += int.Parse(exits_formatted[8]);
            }
            else
            {
                exit_index++;
            }
        }
        catch(IndexOutOfRangeException)
        {
            Debug.Log("The array doesn't have an exits addendum declared, increase exit_index by 1");
            exit_index++;
        }


        
        
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
     * s: save -> used to save game progress (in testing).
     * t: timer -> used to decrease the attempt timer.
     * 
     * Additional Flags:
     * 
     * f: fish -> used in conjunction with hook. Once called, the scene is reset to what the hook saved.
     * h: hook -> save the current index and when "fish" is called retrieve that scene. Used for Dead Ends.
     * 
     * 
     * 
     * 
     * 
     * 
     * Other steps that need to be undertaken are done in the Main Gameplay Loop script
     */

    public string[] SceneFlags(int scene_index)
    {
        string[] return_array = { " " };
        bool returnconfirm = true;

        string[] section_array = SceneFlagSorter(scene_index);

        for (int x = 0; x < section_array.Length; x++)
        {

            if (scene_index + 1 == current_scene.text.Length && alternate_scenario == null && returnconfirm == true)
            {
                Debug.Log("Returning Flag R");
                Array.Resize(ref section_array, section_array.Length + 1);
                exit_index = 0;
                section_array[x + 1] = "r";
                returnconfirm = false;
            }
            else
            {
                switch (section_array[x])
                {
                    case "a":
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
                        break;
                    case "b":
                        // Call Exits is called on the main gameplay loop (needs the int[] return to work)
                        break;
                    case "c":
                        // Change Scenario Text -> this prompts into a change of the index.
                        break;
                    case "f":
                        exit_index = hook_exit_index;
                        // Return to saved 'h' index
                        break;
                    case "h":
                        hook_exit_index = exit_index;
                        // Return to this index once 'f' is called. If 'h' is called again, override the old one.
                        break;
                    case "i":
                        break;
                    case "q":
                        break;
                    case string m when Regex.IsMatch(m, "^m[0-9]{2}$"):             
                        break;
                    case string s when Regex.IsMatch(s, "^s[0-9]{2}$"):
                        break;
                    case "t":
                        break;
                    default:
                        // Return 'd', prompt error message on the d
                        break;
                }
            }

           
        }

        return section_array;
        
    }

    // Works as Intended, Perfect!
    private string[] SceneFlagSorter(int scene_index)
    {
        string[] sort_array = { " " };

        string scene_flags = current_scene.scene_flags[scene_index];
        int y = 0;

        for(int i = 0; i < scene_flags.Length; i++)
        {
            if (y > 0)
            {
                Array.Resize(ref sort_array, sort_array.Length + 1);
            }

            string workingstring = "";
            bool processComplete = false;

            while(!processComplete)
            {
                Debug.Log(i);
                string process_string = scene_flags.Substring(i, 1);
                foreach (char c in process_string)
                {

                   if(workingstring.Length >= 1)
                   {
                        if (Char.IsLetter(c))
                        {
                            sort_array[y] = workingstring;
                            y++;

                            processComplete = true;
                            i--;
                            break;
                        }
                        else if (Char.IsNumber(c))
                        {
                            workingstring += c.ToString();

                            if (i + 1 == scene_flags.Length)
                            {
                                sort_array[y] = workingstring;
                                y++;
                                processComplete = true;
                                break;
                            }
                            else
                            {
                                i++;
                            }
                            
                            
                        }
                   }
                   else
                   {
                        if (Char.IsLetter(c))
                        {
                            workingstring += c.ToString();

                            if(i + 1 == scene_flags.Length)
                            {
                                sort_array[y] = workingstring;
                                y++;
                                processComplete = true;
                                break;
                            }
                            else
                            {
                                i++;
                            }

                        }
                        else
                        {
                            Debug.Log("Error in formatting exit flags, make sure they are correct!");
                        }

                    }

   
                }
            }

        }

        for(int z = 0; z < sort_array.Length; z++)
        {
            Debug.Log("sort_array " + (scene_index + 1) + "'s value " + z + " is " + sort_array[z]);
        }
        return sort_array;
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
        //TextDisplay.text = current_scene.text[id];                                                       
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
                text_to_write = dialogue.responses[response[x]];
                writing_director.TypingEffect(TextDisplay, text_to_write, 0.045f);
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
