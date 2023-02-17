using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/*
 * "A ScriptableObject is a data container that you can use to save large amounts of data, indepentent of class instances. One of the main use cases for ScriptableObjects is to reduce your Project's memory usage
 *  by avoiding copies of values" (Unity Technologies, 2019). ScriptableObject are an easy way to implement "class"-like elements in the game, but they are memory efficent to avoid copies.
 *  Various parts of the game are implemented via ScriptableObjects. Rooms and Dialogue are managed on a case-by-case basis but operate on the same principle for ease of use.
 */

/*
 *  This unique scriptable object is known as an Act. An Act contains an array of Scenarios and Dialogues that make up what is known as an act: a sequence of scenarios or events that the player must go through
 *  in order to play the game. For example, certain situations require multiple dialogues in a row, other require a mix of dialogue and scenarios, and others just scenarios. Occasionally, this can take a toll
 *  while creating the game: especially when it becomes a confusing mess of callbacks and so forth. Acts make it easier to group up what is required in the game and to use it when needed.
 * 
 */

[CreateAssetMenu(menuName = "Scriptable Objects/Scriptable Object Container/Act")]
public class SOT_Act : ScriptableObject
{
    // Act ID, used for recognition within game scritps. Each Act has a different ID that is checked in case the scenario needs to change, for example.
    [Tooltip("Act ID")]
    public int actid;

    [Tooltip("All Scenarios and Dialogues that make an Act")]
    public SOT_Scene[] act_elements;
}
