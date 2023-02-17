using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/*
 * "A ScriptableObject is a data container that you can use to save large amounts of data, indepentent of class instances. One of the main use cases for ScriptableObjects is to reduce your Project's memory usage
 *  by avoiding copies of values" (Unity Technologies, 2019). ScriptableObject are an easy way to implement "class"-like elements in the game, but they are memory efficent to avoid copies.
 *  Various parts of the game are implemented via ScriptableObjects. Rooms and Dialogue are managed on a case-by-case basis but operate on the same principle for ease of use.
 */

// Allows to create a ScriptableObject by Right-Clicking on the Menu and creating one by choosing the following path ("Scriptable Objects" -> "Scene" -> "Dialogue")
[CreateAssetMenu(menuName = "Scriptable Objects/Scene/Dialogue")]
// Class Inheritence changed from "MonoBehaviour" to "SOT_Progression", this is to actually create an Inhertied Scriptable Object instead of a standard class file or standard scriptable object
public class SOT_Dialogue : SOT_Scene
{
    /*
     * Dialogues, unlike Scenarios, are made of a response sent to the recipient of the dialogue. In turn, this makes dialogues more fun and interactive for the user, as well as gives a distinct "voice" to the MC
     * 
     *                      "I can't believe I just explained what a conversation is..."
     *                                          
     *                                                                                  - LonelyBones
     */
    [Tooltip("Dialogue Responses")]
    [TextArea]
    public string[] responses;


}
