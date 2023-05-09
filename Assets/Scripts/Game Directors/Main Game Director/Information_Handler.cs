using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static Encryption_Assistant;

// Information Handler is the script that mediates the EncryptionProcess that allows to save data in the game and the Other scripts. Here, data for the game is saved using SaveGame and SaveData, then subsequently recovered using LoadData.
public class Information_Handler : MonoBehaviour
{
    // the string that contains the game data to save into the game, private so it can't be accessed by other scripts
    private string game_data;

    // The name of the savefile and datafile. This is what they look like, using their unique extension ".save" and ".data"
    private string savefile = "HornOfAmmon_Savefile.save";
    private string datafile = "HornOfAmmon_Datafile.data";

    // The filepath required to reach, say, the save location of the game
    private string filepath;

    // Private, extermely important values! Secret_Key and IV are used to encrypt and decrypt data present in the game.
    private string secret_key;
    private string iv;

    private void Awake()
    {
        // Use Persistent Data Path. On most Windows machines, this should be .../appdata/LocalLow/LonelyBonesDev/Horn Of Ammon/
        filepath = Application.persistentDataPath;

        // If it doesn't exist, just create it.
        if (!Directory.Exists(filepath))
        {
            Directory.CreateDirectory(filepath);
        }

        // Find the config file that contains the secret key and iv. Given that it's a JSON, it takes a few steps to get it be read using JsonUtility
        string config_json = File.ReadAllText(Path.Combine(Application.dataPath, "config.json"));
        Debug.Log(config_json);
        var config = JsonUtility.FromJson<Encryption_Data>(config_json);
        secret_key = config.secret_key;
        iv = config.iv;

    }

    // This function prepares the game for the saving process, then begins DataSave and Encryption
    public void SaveGame(SOT_Scene scene, int index, int act_index, int play_index,  int exit_index, int timer, int attempts, int hook_index, int password_index, int chapter_progression_index)
    {
        // Savepath and Datapath contain the filename also (that's why they are different!
        string savepath = Path.Combine(filepath, savefile);
        Debug.Log(savepath);
        // This formatting may seem weird, but it's actually quite handy when cutting through it in the main gameplay loop. Also, it's inspired by Risk of Rain 2's player profile file.
        game_data = "<currentscene>" + scene + "</currentscene>" + "\n"                                      //SaveArray[0]
                  + "<index>" + index + "</index>" + "\n"                                                    //SaveArray[1]
                  + "<actindex>" + act_index + "</actindex>" + "\n"                                          //SaveArray[2]
                  + "<playindex>" + play_index + "</playindex>" + "\n"                                       //SaveArray[3]
                  + "<exitindex>" + exit_index + "</exitindex>" + "\n"                                       //SaveArray[4]
                  + "<attempttimer>" + timer + "</attempttimer>" + "\n"                                      //SaveArray[5]
                  + "<attemptnumber>" + attempts + "</attemptnumber>" + "\n"                                 //SaveArray[6]
                  + "<hookindex>" + hook_index + "</hookindex>" + "\n"                                       //SaveArray[7]
                  + "<passwordindex>" + password_index + "</passwordindex>" + "\n"                           //SaveArray[8]
                  + "<chapterprogression>" + chapter_progression_index + "</chapterprogression>" + "\n";     //SaveArray[9]
                
        //  Maybe save passwords

        Encryption_Assistant.Encrypt_GameData(game_data, savepath, secret_key, iv);
    }

    // Loads the game savefile from the game.
    public string LoadGame()
    {
        // Savepath and Datapath contain the filename also (that's why they are different!
        string savepath = Path.Combine(filepath, savefile);
        string load_data = Encryption_Assistant.Decrypt_GameData(savepath, secret_key, iv);

        return load_data;
    }

    // Loads the Save Data from the game.
    public void SaveData(string data)
    {
        // Savepath and Datapath contain the filename also (that's why they are different!
        string savepath = Path.Combine(filepath, datafile);

        Encryption_Assistant.Encrypt_GameData(data, savepath, secret_key, iv);
    }

    // Loads the data from the game.
    public string LoadData()
    {
        // Savepath and Datapath contain the filename also (that's why they are different!
        string savepath = Path.Combine(filepath, datafile);
        string load_data = Encryption_Assistant.Decrypt_GameData(savepath, secret_key, iv);

        return load_data;
    }
    
    // Just check if the first and last playerprefs have been set. If so, then say that settings are ready.
    public bool LoadSettings()
    {
        bool settings;

        if(PlayerPrefs.HasKey("music_volume") && PlayerPrefs.HasKey("textspeed"))
        {
            settings = true;
        }
        else
        {
            settings = false;
        }

        return settings;
    }


    // I... don't even think I'm using this anymore. This is a remain of the older implementation of Encryption. However... I'm scared to remove it in case I break the game.
    [System.Serializable]
    private class Encryption_Data
    {
        public string secret_key;
        public string iv;
    }
}
