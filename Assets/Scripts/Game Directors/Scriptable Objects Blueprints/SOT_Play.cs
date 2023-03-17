using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * "A ScriptableObject is a data container that you can use to save large amounts of data, indepentent of class instances. One of the main use cases for ScriptableObjects is to reduce your Project's memory usage
 *  by avoiding copies of values" (Unity Technologies, 2019). ScriptableObject are an easy way to implement "class"-like elements in the game, but they are memory efficent to avoid copies.
 *  Various parts of the game are implemented via ScriptableObjects. Rooms and Dialogue are managed on a case-by-case basis but operate on the same principle for ease of use.
 */

/*
 * To make the game FULLY modular, there needs an efficent and direct way for the game to pull acts into the game world. The Play Scriptable Object contains ALL acts that make up the game, in the order of loading.
 * This allows to add or remove acts from the Unity Editor, as opposed to hardcoding them in text.
 * 
 */

[CreateAssetMenu(menuName = "Scriptable Objects/Scriptable Object Container/Play")]
public class SOT_Play : ScriptableObject
{

    /*
     *  Where all playable sections of the game are compiled and put together. If you want to add an act, you can move it in here!
     *  Acts are used as a quick way to divide certain gameplay sections of the game (such as prologue, puzzle1, puzzle2, etc.) and are placed in order of appearance in here.
     */
    [Tooltip("All Acts of the game, compiled!")]
    public SOT_Act[] all_acts;
}
