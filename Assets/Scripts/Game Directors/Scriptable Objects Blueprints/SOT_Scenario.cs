using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/*
 * "A ScriptableObject is a data container that you can use to save large amounts of data, indepentent of class instances. One of the main use cases for ScriptableObjects is to reduce your Project's memory usage
 *  by avoiding copies of values" (Unity Technologies, 2019). ScriptableObject are an easy way to implement "class"-like elements in the game, but they are memory efficent to avoid copies.
 *  Various parts of the game are implemented via ScriptableObjects. Rooms and Dialogue are managed on a case-by-case basis but operate on the same principle for ease of use.
 */

// Allows to create a ScriptableObject by Right-Clicking on the Menu and creating one by choosing the following path ("Scriptable Objects" ->  "Scene" ->  "Scenario")
[CreateAssetMenu (menuName = "Scriptable Objects/Scene/Scenario")]
// Class Inheritence changed from "MonoBehaviour" to "SOT_Progression", this is to actually create an Inhertied Scriptable Object instead of a standard class file or standard scriptable object
public class SOT_Scenario : SOT_Scene
{
    /*
     * More often than not, the wider "container" for a determined situation the player is in will be the scenario. Often you have to call Dialogue from a scenario, instead of completely changing the current
     * loaded Progression element the Scenarios may reference to their loaded dialogues when needed using this array of Dialogue Scriptable Objects.
     * 
     * However it's important to remember: NOT ALL DIALOGUES ARE BOUND TO SCENARIOS. Usually, Dialogues that are dependant of a scenario are preceded by the "Scenario_" prefix in their name.
     * 
     * A function to operate these dialogues can also be called from the SO_Director script used to retrive the necessary information when it is needed (to display the dialogue's buttons, for example)
     * 
     */
    [Tooltip("Dialogues related to a scenario / that are supposed to play during a scenario")]
    public SOT_Dialogue[] scenario_dialogues;
}
