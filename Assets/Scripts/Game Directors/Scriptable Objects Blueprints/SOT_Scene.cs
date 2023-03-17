using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * "A ScriptableObject is a data container that you can use to save large amounts of data, indepentent of class instances. One of the main use cases for ScriptableObjects is to reduce your Project's memory usage
 *  by avoiding copies of values" (Unity Technologies, 2019). ScriptableObject are an easy way to implement "class"-like elements in the game, but they are memory efficent to avoid copies.
 *  Various parts of the game are implemented via ScriptableObjects. Rooms and Dialogue are managed on a case-by-case basis but operate on the same principle for ease of use.
 */

public abstract class SOT_Scene : ScriptableObject
{
    // Element ID, used for recognition within game scritps. Each progression element has a different id that is checked in case the scenario needs to change, for example.
    [Tooltip("Element ID")]
    public int id;

    /* 
     * [TextArea] creates a bigger text box in the Unity Editor. It's useful when adding or removing large quantities of text!
     * 
     * List of strings called text: here are all the writing options that are inserted in the MAIN textbox of the game. 
     *  
     *  All text relative to a room and its puzzles is kept in the "ScenarioTree.txt" document within the "Assets/Scripts/Scriptable Objects/Scenes/Progression Trees" folder.
     *  All text relative to a dialogue and its responses is kept in the "DialogueTree.txt" document within the "Assets/Scripts/Scriptable Objects/Scenes/Progression Trees" folder.
     */
    [TextArea]
    [Tooltip("Scene Text— remember formatting rules!")]
    public string[] text;

    /*
     * [TextArea] creates a bigger text box in the Unity Editor. It's useful when adding or removing large quantities of text!
     * 
     * List of strings called exit_text: here are all the options to show the text to be put on the various choice buttons when they are called into action by the main game loop.
     * This list of strings contains both the text options for the various buttons.
     * Generally speaking, the text follows this protocol.
     * 
     */
    [TextArea]
    [Tooltip("Button Text")]
    public string[] exit_text;

    /*
     * [TextArea] creates a bigger text box in the Unity Editor. It's useful when adding or removing large quantities of text!
     * A string array used to determine the outcome of choice buttons. Each element of the array contains a group of "exits", each "exit" has a value (stored in groups of 4) the number of which corresponds to the respective text in the text array (SOT_Progression.cs -> line 22.
     * There are four choice buttons, so four choices at maximum are available per scenario, which explains the storage method. Navigated through using a global index value. This is also used to determine whether a button is active or not (if an exit value reads 0) it means
     * that button should be disabled.
     * 
     * IE: Assume Progression is a Scenario. After the Introduction to a room, the navigation script retrives the current room's exits' line0. Here, there are the values [1, 2, 3, 0]. This means that pressing button0 takes value 1,
     * button1 produces value 2, button2 produces value 3 and button3 should not be available in this scenario, hence why it produces value 0. After a button is pressed, index changes if the room or situation causes it so.
     * 
     * 
     * 
     *              "I wanted to have this as a List of Lists or an Array of Arrays...
     *              but it doesn't appear this actually works. I have to put it as a list
     *              of Strings and then retrieve the results later using the "CallExits" Function..."
     *                                          
     *                                                                                  - LonelyBones
     *
     *
     */
    [TextArea]
    [Tooltip("Exit Text used to determine the outcome of choice buttons, button text AND button flags")]
    public string[] scene_exits;

    /*
     * A string array called scene_tree, this is used to determined how the scene needs to proceed. Called at the end of every text displayed in the main text display that isn't a response, the scene tree
     * tells the game what the next manouver should be to proceed in the scene., based on the character read from this variable. The index to navigate this array is the exact same as the index to navigate the
     * text array (aka index), which is used to determine which text is displayed AND what to do after that text is displayed
     * 
     * Scene flags uses certain characters to determine how progression is handled for different components. Here are the following:
     * 
     * a, or "alternate" Used in Scenarios, this is a clue for the game to save the current index and then start the dialogue within a scenario. This flag is also used in Scenario-exclusive Dialogues to return to the Scenario using the saved index.
     * b, or "buttons": Call for the game to start Choice Buttons, using index to call the function CallExits()
     * c, or "continue": The index increases by one, and the next text is displayed. This WAITS for the Response to be finished first in the case of Dialogue
     * r, or "return": used exclusively as a way to tell the game that the current scene is over and to move over to the next one
     * 
     * Additional Flags:
     * 
     * f, or "fish": used in conjunction with hook. Once called, the scene is reset to what the hook saved.
     * h, or "hook":save the current index and when "fish" is called retrieve that scene. Used for Dead Ends.
     * 
     * s, or "sound": used to scour for sound clips and add a sound effect to the clip. Search the BBC network for loads of soundclips.
     * t, or "timer": used to decrease the attempt timer.
     * 
     * Other steps that need to be undertaken are done in the Main Gameplay Loop script
     */
    [Tooltip("This is used to determine how the scene needs to proceed, if it needs to be interrupted, if certain dialogues need to be skipped or anything. \n This is the flag index: \n a: alternate (use exclusively in Scenarios w/ Dialogues or Dialogues-within-scenarios) \n" +
             "b: buttons \n c: continue \n d: default (used for interruption or displaying error messages) \n f: fish (used in conjunction with hook, fish is the last text before returning to the hook text \n h: hook (used in conjunction with fish, hook saves the current room and is called back with fish \n" +
             "s: sound (calls the sound director for more specific sounds) \n t: timer (decrease attempt timer)")]
    public string[] scene_flags;

}
