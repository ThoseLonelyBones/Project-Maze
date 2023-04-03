using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static Encryption_Assistant;

public class Information_Handler : MonoBehaviour
{
    private string game_data;

    private string savefile = "HornOfAmmon_Savefile.save";
    private string datafile = "HornOfAmmon_Datafile.data";

    private string filepath;

    private string secret_key;
    private string iv;

    // Call Application.persistentDataPath in here
    private void Awake()
    {
        filepath = Application.persistentDataPath;

        if (!Directory.Exists(filepath))
        {
            Directory.CreateDirectory(filepath);
        }

        string config_json = File.ReadAllText(Path.Combine(Application.dataPath, "config.json"));
        Debug.Log(config_json);
        var config = JsonUtility.FromJson<Encryption_Data>(config_json);

        secret_key = config.secret_key;
        iv = config.iv;

        Debug.Log("This is me reading the JSON file:" + secret_key);
        Debug.Log("This is me reading the JSON file, again:" + iv);
    }

    public void SaveGame(SOT_Scene scene, int index, int act_index, int play_index,  int exit_index, int timer, int attempts)
    {
        string savepath = Path.Combine(filepath, savefile);

        Debug.Log(savepath);

        game_data = "<currentscene>" + scene + "</currentscene>" + "\n"
                  + "<index>" + index + "</index>" + "\n"
                  + "<actindex>" + act_index + "</actindex>" + "\n"
                  + "<playindex>" + play_index + "</playindex" + "\n"
                  + "<exitindex" + exit_index + "</exitindex>" + "\n"
                  + "<attempttimer>" + timer + "</attempttimer>" + "\n"
                  + "<attemptnumber>" + attempts + "</attemptnumber>" + "\n";

        Encryption_Assistant.Encrypt_GameData(game_data, savepath, secret_key, iv);
    }

    public string LoadGame()
    {
        string savepath = Path.Combine(filepath, savefile);
        string load_data = Encryption_Assistant.Decrypt_GameData(savepath, secret_key, iv);

        return load_data;
    }

    public void SaveData(/*Stuff saved with the 'q' flag (questionarre) will be put in here as an imported array, then edited inside this function.*/)
    {
        string savepath = Path.Combine(filepath, datafile);


    }


    [System.Serializable]
    private class Encryption_Data
    {
        public string secret_key;
        public string iv;
    }
}
