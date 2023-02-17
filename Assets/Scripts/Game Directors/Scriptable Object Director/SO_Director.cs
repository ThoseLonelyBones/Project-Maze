using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

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
    /*
     * Progression Elements are loaded only when needed
     */
    public SOT_Scene    current_scene;

    public SOT_Scenario alternate_scenario = null;
    public int          alternate_index;
    private int         alt_scenario_count = 0;


    /*
     * GameObject and Text still need to be accessed, even with the TextDirector script actually putting in the text. 
     */

    public GameObject   button1 = GameObject.Find("Button 1"), 
                        button2 = GameObject.Find("Button 2"), 
                        button3 = GameObject.Find("Button 3"), 
                        button4 = GameObject.Find("Button 4");

    public Text         button1Text,
                        button2Text,
                        button3Text,
                        button4Text;

    public Text         TextDisplay;

    public int[]        exits_array = { 0, 0, 0, 0 };


    /*  
     *  CallExits(int scene_index) is a function that returns an Int array of values, exit[], which contains (in order of buttons) the IDs that their respective button will create the text for.
     *  To check which buttons to activate and to check what text is required to be placed on each button, it takes the current_scenario's scenario_exits string[], which is formatted like this:
     *  
     *      [x1,x2,x3,x4][b1,b2,b3,b4]
     *  
     *  The first square bracket contains the ID value of the text each button is assigned to, as shown in the scenario_exits variable in the SOT_Scenario.cs script (line 78)
     *  These values are also subsequently converted from strings to integer and then passed as an int[] to the GameDirector script.
     *  The second square bracket contains the ID value of the text inside the respective button, as shown in the exit_text variable in the SOT_Scenario.cs script (line 56)
     *  These values are not returned to the GameDirector function. Instead, they are passed to the TextDirector script to be formatted correctly and then subsequently put inside each button.
     *  (... As of now the text simply appears in the text boxes with no formatting ...)
     * 
     */



    void CallExits(int scene_index)
    {
        exits_array = { 0, 0, 0, 0 };
        string[] current_scene_exits = current_scene.scene_exits;

        string exits_editing = current_scene_exits[scene_index];            // => exits_editing = [11,12,13,14][1,2,3,4]
        // Step 1 = Merge '][' into ','                                  
        exits_editing = exits_editing.Replace("][", ",");                   // => exits_editing = [11,12,13,14,1,2,3,4]
        // Step 2 = Delete "[" and "]"
        exits_editing = exits_editing.Replace("[", "");                     // => exits_editing = 11,12,13,14,1,2,3,4]
        exits_editing = exits_editing.Replace("]", "");                     // => exits_editing = 11,12,13,14,1,2,3,4
        // Step 3 = Split the String by "," -> The first 4 values are the Button values (exits[], the return integer array)
        //                                  -> The last 4 values are the value of exit_text (the button's text)                                    
        string[] exits_formatted = exits_editing.Split(",");
        // Step 4 = exits_formatted is now usable in the next for loop! The example above now looks something like this:
        //
        //  exits_formatted[0] = 11; exits_formatted[3] = 14; exits_formatted[4] = 1; exits_formatted[7] = 4 
        // However this is still a string. So it still needs to be formatted a bit to actually work, as seen below using int.Parse


        for (int x = 0; x < 4; x++)
        {
           exits_array[x] = int.Parse(exits_formatted[x]);
           if(exits_array[x] > 0)
           {
                switch(x)
                {
                    case 0:
                        // Activate Button 1;
                        button1Text.text = "";                                                              // => Replace this with TextDirector's CleanseText Function when done!
                        button1.SetActive(true);
                        button1Text.text = current_scene.exit_text[int.Parse(exits_formatted[x + 4])];      // => Replace this with TextDirector's Writing Function when done!
                        break;
                    case 1:
                        // Activate Button 2;
                        button2Text.text = "";                                                              // => Replace this with TextDirector's CleanseText Function when done!
                        button2.SetActive(true);
                        button2Text.text = current_scene.exit_text[int.Parse(exits_formatted[x + 4])];      // => Replace this with TextDirector's Writing Function when done!
                        break;
                    case 2:
                        // Activate Button 3;
                        button3Text.text = "";                                                              // => Replace this with TextDirector's CleanseText Function when done!
                        button3.SetActive(true);
                        button3Text.text = current_scene.exit_text[int.Parse(exits_formatted[x + 4])];      // => Replace this with TextDirector's Writing Function when done!
                        break;
                    case 3:
                        // Active Button 4;
                        button4Text.text = "";                                                               // => Replace this with TextDirector's CleanseText Function when done!
                        button4.SetActive(true);
                        button4Text.text = current_scene.exit_text[int.Parse(exits_formatted[x + 4])];      // => Replace this with TextDirector's Writing Function when done!
                        break;
                    default:
                        Debug.Log("LonelyBones has made the advancement [How Did We Get Here?]");
                        break;
                }

           }
        }

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
     * 
     * Other steps that need to be undertaken are done in the Main Gameplay Loop script
     */

    char SceneFlags(int scene_index)
    {      

        switch(current_scene.scene_flags[scene_index])
        {
            case 'a':
                if(alternate_scenario == null && current_scene is SOT_Scenario)
                {
                    try
                    {
                        alternate_scenario = (SOT_Scenario)current_scene;

                        alternate_index = scene_index + 1;                                               // Do I really need this? Check main gameplay loop. <= Yeah, you do.
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
                        ChangeScenarioText(alternate_index);

                        alternate_scenario = null;
                        alternate_index = 0;
                    }
                }
                return 'a';
            case 'b':
                // Call Exits
                CallExits(scene_index);
                return 'b';
            case 'c':
                // Change Scenario Text
                ChangeScenarioText(scene_index + 1);
                return 'c';
            default:
                // Return 'd', prompt error message on the d
                return 'd';
        }

    }

    int[] ReturnExits()
    {
        return exits_array;
    }

    void ChangeScenarioText(int id)
    {
        TextDisplay.text = "";
        TextDisplay.text = current_scene.text[id];                                                          // => Replace this with TextDirector's Writing Function when done!
    }

}
