using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/*
 * The bad, the good and the ABSOLUTELY MASSIVE GAMEPLAY DIRECTOR!
 * 
 *  Gameplay_Director is the main gameplay script of the game. In here, the flags of each Textblock are further checked from SO_Director and specific actions are taken based on the player's choices. This script governs
 *  button presses, saving through the save button and the save flag and handles the passcode input. Additionally, it is the main connection junction between most other scripts. Gameplay Director calls all utility scripts to itself
 *  and uses this fact to pass information between various other scripts. Additionally it handles navigation through the scenes of an act and the acts of a play.
 * 
 */

public class Gameplay_Director : MonoBehaviour
{

    /*
     * Including the other important scripts, such as:
     * ScriptableObject Director
     * Timer Director
     * ScriptableObjectTemplate Scene (which is the current scene that's playing)
     * UIDirector, which governs aspects such as button placement, etc.
     * InformationHandler, which handles information stored and saved in the game.
     * InputHandler, which handles passcode inputs and checks related to that.
     * AudioDirector, which handles all audio related aspects of the game.
     */

    private SO_Director so_director;

    public Timer_Director timer_director;

    [SerializeField]
    private UI_Director ui_director;

    [SerializeField]
    private Information_Handler info_handler;

    [SerializeField]
    private Input_Handler input;

    [SerializeField]
    private Audio_Director audio_director;

    /*
     * The two ScriptableObjects used in the game, Play (which contains a collection of acts which are a collection of scenes (abstract parent of dialogues and scenarios) and the current scene.
     */

    public SOT_Play sot_play;
    public SOT_Scene scene;

    /*
     * 
     * The choice buttons as gameobjects, buttons and TMPs for text editing. It allows to activate / deactivate them when needed, as well as change their text and give them their functions
     * 
     */

    public static GameObject       button1,
                                   button2,
                                   button3,
                                   button4;

    public Button                  button_1,
                                   button_2,
                                   button_3,
                                   button_4;


    public TextMeshProUGUI         button1Text,
                                   button2Text,
                                   button3Text,
                                   button4Text;

    /*
     * 
     * The text display, which is where the bulk of the game's text is displayed, both in GameObject and TMP form
     * 
     */

    public GameObject textDisplay;
    public TextMeshProUGUI TextDisplay;

    /*
     *  The SaveGame Button
     */
    public Button save_button;


    /*
     * Button Exits, used to determine to which index the user should be sent to once they click on this specific button. For more information, search for the ReadFlag's "b" flag.
     * 
     */
    public int              button1Exit,
                            button2Exit,
                            button3Exit,
                            button4Exit;

    /*
     * Passcode Field and Fake Passcode Field, used to give the appearance of a Password Field that gets filled by the user. More on this in Input Handler and UI Director
     * 
     */

    public GameObject passcode_field;
    public GameObject fake_passcode_field;

    /*
     * Control Booleans:
     * progressText = required to make the game go to the next index / terminate current scene. If its off then it means the game wants to keep you on the current scene.
     * alternateScene = used in conjunction with the flag "a" of the ReadFlags function. For more info, read about flag "a"
     * isResponse = used during dialogues, isResponse triggers after a button response, showing text but not progressing the game by reading the flags of the current index.
     */

    public bool progressText = true;
    public bool alternateScene = false;
    public bool isResponse = false;
    public bool fixFlagR = true;


    /*
     * Control Strings:
     * button_response: used to determine which button was pressed in the ButtonClick function
     * text_check: used to verify whether or not the text being written in at the moment is correct
     */
    private string button_response;
    public string text_check;


    /*
     * Control Int:
     * Hook index is used in conjunction with the "h" and "f" flags to allow the game to return to a determined index when needed.
     */
    [SerializeField]
    private int hook_index;


    /*
     * One of the most important variables in the entire game. Index is used to navigate through the multitude of text that are present in the game. It's like a bookmark, telling the program at which page they are
     * currently at. It's used in a multitude of functions in both this script and the SO_Director. It's handled with the utmost care and it's always beeing kept track of. Other two indexes, act_index and play_index are used to specifically
     * keep track of where in the general game progression one is.
     * 
     */
    [SerializeField]
    private int index;
    [SerializeField]
    private int act_index = 0;
    [SerializeField]
    private int play_index = 0;
    [SerializeField]
    private int chapter_progression_index = 0;
    [SerializeField]
    private int encore_index = 0;

    /*
     * These two booleans are activated by the menu options present in the game, also questionare has an exclusive flag to activate i
     * 
     * gamedata_save can be activated by the flag q or by the Data Gathering toggle in the options menu
     * autosave is on by default and can be toggled to be off
     */


    // The string containing the quantity of player choices they have made. Nothing spectacular.
    private string gamedata;

    public int[] exits_addendum_array = { 1, 1, 1, 1 };

    private bool reset_scenario = false;



    private void Awake()
    {
        so_director = GetComponent<SO_Director>();
        timer_director = GetComponent<Timer_Director>();

        info_handler = GetComponent<Information_Handler>();
        input = GetComponent<Input_Handler>();


        if (timer_director == null)
        {
            Debug.Log("Timer Director not set");
        }
        //writing_director = GetComponent<Writing_Director>();


        button1 = GameObject.Find("Button 1");
        button2 = GameObject.Find("Button 2");
        button3 = GameObject.Find("Button 3");
        button4 = GameObject.Find("Button 4");

        GameObject audiodirector = GameObject.Find("AudioDirector");
        audio_director = audiodirector.GetComponent<Audio_Director>();
        if (audio_director == null)
        {
            Debug.Log("Audio Director not set!");
        }
        else
        {
            Debug.Log("Audio Director set!");
        }

        button_1.onClick.AddListener(() => buttonClick("Button 1"));
        button_2.onClick.AddListener(() => buttonClick("Button 2"));
        button_3.onClick.AddListener(() => buttonClick("Button 3"));
        button_4.onClick.AddListener(() => buttonClick("Button 4"));

        save_button.onClick.AddListener(() => SaveGame());



        Debug.Log("Main Gameplay Loop Awake");


        audio_director.Silence();
    }

    // This function is used to hide elements of the game before the first scenario is loaded
    public void cleanScreen()
    {
        passcode_field.SetActive(false);
        fake_passcode_field.SetActive(false);

        buttonsHide();
    }

    // This function hides the text buttons used during the game.
    public void buttonsHide()
    {
        button1Text.text = "";
        button1.SetActive(false);

        button2Text.text = "";
        button2.SetActive(false);

        button3Text.text = "";
        button3.SetActive(false);

        button4Text.text = "";
        button4.SetActive(false);

    }

    public void buttonsSet(int[] exits)
    {
        bool[] button_active = { false, false, false, false };

        for (int x = 0; x < 4; x++)
        {
            if (exits[x] > 0)
            {
                button_active[x] = true;

                switch (x)
                {
                    case 0:
                        button1Exit = exits[x];
                        //button1.SetActive(true);
                        break;
                    case 1:
                        button2Exit = exits[x];
                        //button2.SetActive(true);
                        break;
                    case 2:
                        button3Exit = exits[x];
                        //button3.SetActive(true);
                        break;
                    case 3:
                        button4Exit = exits[x];
                        //button4.SetActive(true);
                        break;
                    default:
                        Debug.Log("There was supposed to be a cool error message here but I forgor :skull:");
                        break;
                }
            }
        }


        ui_director.ButtonAlign(button_active);

        progressText = false;
    }

    public void buttonClick(string buttonname)
    {
        button_response = buttonname;

        if (so_director.current_scene is SOT_Dialogue && !isResponse)
        {
            Debug.Log("This is a response!");
            text_check = so_director.DialogueResponse(buttonname);
            isResponse = true;
        }
        else
        {
            Debug.Log("Button Pressed");
            switch (buttonname)
            {
                case "Button 1":
                    so_director.ChangeScenarioText(button1Exit);
                    text_check = so_director.ChangeScenarioText(button1Exit);
                    index = button1Exit;
                    so_director.exit_index += exits_addendum_array[0];
                    break;
                case "Button 2":
                    so_director.ChangeScenarioText(button2Exit);
                    text_check = so_director.ChangeScenarioText(button2Exit);
                    index = button2Exit;
                    so_director.exit_index += exits_addendum_array[1];
                    break;
                case "Button 3":
                    so_director.ChangeScenarioText(button3Exit);
                    text_check = so_director.ChangeScenarioText(button3Exit);
                    index = button3Exit;
                    so_director.exit_index += exits_addendum_array[2];
                    break;
                case "Button 4":
                    so_director.ChangeScenarioText(button4Exit);
                    text_check = so_director.ChangeScenarioText(button4Exit);
                    index = button4Exit;
                    so_director.exit_index += exits_addendum_array[3];
                    break;
                default:
                    Debug.Log("How could this happen to me?");
                    break;
            }
        }

        buttonsHide();
        progressText = true;
    }

    public void readFlag(string[] flag)
    {
        for (int x = 0; x < flag.Length; x++)
        {
            Debug.Log("flag on Gameplay_Director is equal to " + flag[x]);

            switch (flag[x])
            {
                case "a":
                    if (!alternateScene)
                    {
                        index = 0;
                        text_check = so_director.ChangeScenarioText(index);
                        alternateScene = true;
                    }
                    else
                    {
                        index = so_director.alternate_index;
                        so_director.alternate_index = 0;
                        text_check = so_director.ChangeScenarioText(index);
                    }
                    break;
                case "b":
                    buttonsSet(so_director.CallExits());
                    break;
                case "c":
                    index++;
                    text_check = so_director.ChangeScenarioText(index);
                    break;
                case "e":
                    // This flag is checked every time you complete a code in the repeat section!

                    // Find a way to calculate 
                    encore_index++;
                    break;
                case "f":
                    text_check = so_director.ChangeScenarioText(hook_index);
                    index = hook_index;
                    break;
                case "h":
                    hook_index = index;
                    break;
                case string i when Regex.IsMatch(i, "^i[0-9]{2}$"):
                    input.Cleanse();

                    Image passcode_image = passcode_field.GetComponent<Image>();
                    Color passcode_color = new Color(1f, 1f, 1f, 1f);
                    passcode_image.color = passcode_color;

                    EventSystem eventsystem = EventSystem.current;
                    eventsystem.SetSelectedGameObject(null);

                    passcode_field.SetActive(true);
                    input.inputfield.characterLimit = so_director.current_scene.password[so_director.input_index].Length;
                    input.progress = false;
                    break;
                case "q":
                    PlayerPrefs.SetString("datacollection", "true");
                    Debug.Log("Data Collection is enabled!");
                    break;
                case "k":
                    BacktoMainMenu();
                    break;
                case string m when Regex.IsMatch(m, "^m[0-9]{2}$"):
                    int music_index = int.Parse(flag[x].Substring(1, 2));
                    audio_director.PlayMusic(music_index);
                    break;
                case "r":
                    int act_length;

                        if(!reset_scenario)
                        {
                            act_length = sot_play.all_acts[play_index].act_elements.Length;
                        }
                        else
                        {
                            act_length = 0;
                        }
     
                        Debug.Log("Act Length: " + act_length);
                        if ((act_index + 1) == act_length)
                        {

                             Debug.Log("yes");
                             act_index = 1;
                             play_index++;


                            
                        }
                        else
                        {
                            if(!reset_scenario)
                            {
                                Debug.Log("no");
                                act_index++;
                            }
                            else
                            {
                                reset_scenario = false;
                                
                            }
                            
                        }

                    encore_index = 0;
                    Debug.Log(play_index + " is play index and " + act_index + " is act index");
                    index = so_director.ChangeScene(sot_play.all_acts[play_index].act_elements[act_index]);
                    so_director.input_index = 0;
                    text_check = so_director.ChangeScenarioText(index);
                    fixFlagR = false;
                    if(PlayerPrefs.GetString("autosave") == "true")
                    {
                        SaveGame();
                    }
                    audio_director.LoadSceneAudio(so_director.current_scene.scene_sfx);
                    break;
                case string s when Regex.IsMatch(s, "^s[0-9]{2}$"):
                    int sfx_index = int.Parse(flag[x].Substring(1, 2));
                    audio_director.PlaySceneSFX(sfx_index);
                    break;
                case "t":
                    timer_director.TimerCountdown();
                    // do the return and text change in here.
                    break;
                case string u when Regex.IsMatch(u, "^u[0-9]{2}$"):
                    int finale = int.Parse(flag[x].Substring(1, 2));
                    switch(finale)
                    {
                        case 1:
                            SceneManager.LoadScene("Credits");
                            PlayerPrefs.SetInt("credits_scene", 1);
                            break;
                        case 2:
                            SceneManager.LoadScene("Credits");
                            PlayerPrefs.SetInt("credits_scene", 2);
                            break;
                        default:
                            SceneManager.LoadScene("Credits");
                            PlayerPrefs.SetInt("credits_scene", 0);
                            break;
                    }
                    break;
                case "x":
                    if(PlayerPrefs.GetString("datacollection") == "true")
                    {
                        Debug.Log("Data is being collected!");
                        gamedata += ("Player has reached this index: " + (index - 1) + " in this scenario " + (act_index + 1) + " of the act number " + play_index + ", at the attempt number " + timer_director.Get_Attempt_Number() + "\n");
                        info_handler.SaveData(gamedata);
                    }
                    break;
                case string z when Regex.IsMatch(z, "^z[0-9]{2}$"):
                    int progression_check = int.Parse(flag[x].Substring(1, 2));
                    Debug.Log("Progression Check is: " + progression_check);
                    if(chapter_progression_index < progression_check)
                    {
                        chapter_progression_index = progression_check;
                        Debug.Log("chapter_progression_index is now:" + progression_check);
                    }

                    if(progression_check == 0 && encore_index >= chapter_progression_index)
                    {
                        switch(chapter_progression_index)
                        {
                            case 1:                                     // Interlude to Chapter 2
                                play_index = 2;
                                act_index = 1;
                                Debug.Log("Going to the Interlude of Chapter 2, case 1");
                                break;
                            case 2:
                                play_index = 2;
                                act_index = 2;
                                Debug.Log("Going to the start of Chapter 2, case 2");
                                break;
                            case 3:
                                play_index = 3;
                                act_index = 1;
                                Debug.Log("Going to the Interlude of Chapter 3, case 3");
                                break;
                            case 4:
                                play_index = 3;
                                act_index = 2;
                                Debug.Log("Going to the start of Chapter 3, case 4");
                                break;
                            default:
                                play_index = 2;
                                act_index = 1;
                                Debug.Log("Going to the Interlude of Chapter 2, default");
                                break;
                        }
                    }
                    else
                    {
                        if((encore_index == 2 || encore_index == 3) && chapter_progression_index > 2)
                        {
                            play_index = 2;
                            act_index = 2;
                            Debug.Log("Going to the start of Chapter 2... even though you know more than this!");
                        }
                    }
                    break;

                default:
                    Debug.Log("How has this even happened? Is this even possible?");
                    break;
            }
        }

    }

    public void startGame()
    {
        index = so_director.ChangeScene(sot_play.all_acts[play_index].act_elements[act_index]);
        Debug.Log("Act Index: " + act_index);
        Debug.Log("Play Index: " + play_index);
        so_director.ChangeScenarioText(index);

        text_check = so_director.current_scene.text[index];
        progressText = true;

        timer_director.Set_Attempt_Timer(20);
    }
    public void resumeGame()
    {
        so_director.ChangeScene(sot_play.all_acts[play_index].act_elements[act_index]);
        so_director.ChangeScenarioText(index);
        text_check = so_director.current_scene.text[index];
        progressText = true;
        audio_director.LoadSceneAudio(so_director.current_scene.scene_sfx);
    }

    // Start is called before the first frame update
    void Start()
    {
        cleanScreen();

        int load = PlayerPrefs.GetInt("load");
        Debug.Log("PlayerLoad is set to: " + load);
        if (load == 1)
        {
            Debug.Log("Loading your savefile!");
            string playersavedata = PlayerPrefs.GetString("savedata");
            string[] savedata_array = SaveUnpack(playersavedata);

            index = int.Parse(savedata_array[1]);
            act_index = int.Parse(savedata_array[2]);
            play_index = int.Parse(savedata_array[3]);
            so_director.exit_index = int.Parse(savedata_array[4]);
            timer_director.Set_Attempt_Timer(int.Parse(savedata_array[5]));
            timer_director.Set_Attempt_Number(int.Parse(savedata_array[6]));
            timer_director.TimerCheck();
            hook_index = int.Parse(savedata_array[7]);
            so_director.hook_exit_index = hook_index;
            so_director.input_index = int.Parse(savedata_array[8]);
            chapter_progression_index = int.Parse(savedata_array[9]);
            Debug.Log(so_director.input_index);
            Debug.Log(hook_index);
            gamedata = info_handler.LoadData();
            resumeGame();

            audio_director.music_audio_source.Stop();
        }
        else
        {
            startGame();
        }

        timer_director.TimerCheck();
        Debug.Log("Game Started");
    }

    // Update is called once per frame
    void Update()
    {
        if(timer_director.reset)
        {
            AttemptReset();
        }

        if (Input.GetKeyDown(KeyCode.Space) && progressText && input.progress == true)
        {
            if (TextDisplay.text == so_director.text_to_write)                                          // Change to saved text (which can vary in case of a response) <= done
            {
                if (!isResponse)
                {
                    //readFlag(so_director.SceneFlags(index));
                    ProgressGame();
                }
                else
                {
                    Debug.Log("Response detected!");
                    buttonClick(button_response);
                    isResponse = false;
                    text_check = so_director.current_scene.text[index];
                }
            }
            else
            {
                TextDisplay.text = so_director.text_to_write;              // Skips text writing program when it will be done

            }
        }

    }

    private string[] SaveUnpack(string savedata)
    {
        string[] savedata_array = savedata.Split('\n');
        string filler_pattern = @"<(\w+)>(.*?)<\/\1>";


        Match matcher;

        for (int x = 0; x < savedata_array.Length; x++)
        {
            matcher = Regex.Match(savedata_array[x], filler_pattern);
            if (matcher.Success)
            {
                savedata_array[x] = matcher.Groups[2].Value;
            }
        }

        return savedata_array;
    }

    private void SaveGame()
    {
        SOT_Scene current_scene = so_director.current_scene;
        int exit_index = so_director.exit_index;
        int timer = timer_director.Get_Attempt_Timer();
        int password_index = so_director.input_index;
        info_handler.SaveGame(current_scene, index, act_index, play_index, exit_index, timer, 0, hook_index, password_index, chapter_progression_index);

        ui_director.SystemText("Game Saved!", 5);
    }

    public void ProgressGame()
    {
        if(input.correct_password)
        {
            index = so_director.correct_input_index;
            so_director.ChangeScenarioText(index);
            input.correct_password = false;
            so_director.input_index++;
            
        }
        else
        {
            readFlag(so_director.SceneFlags(index));
        }

    }

    public void HideInput()
    {
        passcode_field.SetActive(false);
    }

    private void AttemptReset()
    {
        if (PlayerPrefs.GetString("autosave") == "true")
        {
            SaveGame();
        }
        timer_director.reset = false;
        GameObject eventsystem = GameObject.Find("EventSystem");
        progressText = false;
        act_index = 0;
        play_index = 1;
        text_check = so_director.Faint();
        ui_director.DisableButtons();
        ui_director.CanvasFade();                                                                                   // Lower everything's Alpha
        StartCoroutine(WaitforCanvasFade());
        StartCoroutine(WaitforCanvasSpawn());
        reset_scenario = true;
    }

    IEnumerator WaitforCanvasFade()
    {
        while(ui_director.canvas_active)
        {
            yield return null;
        }

        Debug.Log("Canvas has finished fading, reset starting now");
        text_check = so_director.ClearScreen();
        ui_director.CanvasSpawn();
        //timer_director.AttemptCountup();
        yield break;
    }

    IEnumerator WaitforCanvasSpawn()
    {
        while(!ui_director.canvas_spawn)
        {
            yield return null;
        }

            Debug.Log("Canvas has finished spawning, start now");
            index = so_director.ChangeScene(sot_play.all_acts[1].act_elements[0]);                                     // End the Current Attempt
            so_director.ChangeScenarioText(index);                                                                     // Restart from predetermined Index
            so_director.exit_index = 0;
            so_director.input_index = 0;
            so_director.hook_exit_index = 0;
            hook_index = 0;
            text_check = so_director.current_scene.text[index];
            progressText = true;
            timer_director.Set_Attempt_Timer(20);
            ui_director.EnableButtons();
            ui_director.canvas_spawn = false;
            yield break;


    }

    private void BacktoMainMenu()
    {
        audio_director.music_audio_source.Stop();
        SceneManager.LoadScene("Main Menu");
    }
    
}
