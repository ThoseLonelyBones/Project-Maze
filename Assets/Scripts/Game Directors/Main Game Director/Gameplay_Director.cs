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

    // This is used to check how much to add to the exit_index from each button's click. If nothing is put in, only +1 is added to it (as usual)
    public int[] exits_addendum_array = { 1, 1, 1, 1 };

    // This is used in checking whether the current scenario is the reset scenario.
    private bool reset_scenario = false;


    // Initialize the WORLD! Everything needs to be initalized, EVERYTHING! You get an initalization aaaand you get an initalization aaaaand everything gets initalized!
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

    // This function sets the buttons, and with that I mean that each time you apply the b index, the game goes through the respective button_exits on the current scene, an array of which the index is determined by exit_index. Whenever you click a button,
    // you are lead to that exit, taken from the array's unique formatting (aka {x1,x2,x3,x4}, where x1 is the exit of the first button and so on and so forth.) This updates the buttons when they are put on screen. They use one function BUT their unique
    // exit allows them to serve different purposes even compared to being the same button in a previous exit (the left-most button (button 1) will obviously lead to a different text than any previous button that was also button 1)
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

        // This aligns whichever buttons are currently active in the lower parts of the screen. It prevents me from doing manual setting badness and makes the game look more sleek.
        ui_director.ButtonAlign(button_active);

        progressText = false;
    }

    // Whenever a button is clicked, it first checks if it should display a response. If it has to, it allows the game to play a response, then this function is called again and does the rest of its needed functions.
    // Based on which button pressed which, send the user to that specific exit, updating the text to be correctly the one the user has decided to move towards as well as update the exit index by the addendum of the button (which can just be 1 if nothing else was set)
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

        // Hide the buttons and then progress the text!
        buttonsHide();
        progressText = true;
    }

    // This massive function is essentially the game's way of applying unique conditions to each differnt text that the game is shown. readFlag takes in a string array, flag[]. Truthfully, the string it takes in is nothing more than all the various flag of
    // a given index on the scene_flags string array, separated in the various values of this string array. Here, for the length of the array, every time a flag and their flag options are detected it does a different effect. Going through them...
    public void readFlag(string[] flag)
    {
        for (int x = 0; x < flag.Length; x++)
        {
            Debug.Log("flag on Gameplay_Director is equal to " + flag[x]);

            switch (flag[x])
            {
                // Flag A causes the current scenario to swap to an internal dialogue, or to swap back from an internal dialogue to the actual scenario.
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
                    // Set the buttons 's exits and shows them to the player
                case "b":
                    buttonsSet(so_director.CallExits());
                    break;
                    // Simply progress through the game! It's nothing too complex, really.
                case "c":
                    index++;
                    text_check = so_director.ChangeScenarioText(index);
                    break;
                case "e":
                    // This flag is checked every time you complete a code in the repeat section, used to determine where you actually get to skip in the repeat area.
                    encore_index++;
                    break;
                case "f":
                    // Send a fish which means you have to return to a hook. Used for trial and error in buttons and for dialogue dead ends, as well as to set an overall scene for the game.
                    text_check = so_director.ChangeScenarioText(hook_index);
                    index = hook_index;
                    break;
                case "h":
                    // Set a hook. Whenever fish is called, this is the index they'll be reaching. It's a way to allow the game to backtrack without having to hardcode a button every single time.
                    hook_index = index;
                    break;
                    // Display the input field and start the input field's various initalization's and requirments (the rest is handled by the input handler function).
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
                    // Data collection enabled, exclusively used in that one section of the warnings at the start of the game.
                case "q":
                    PlayerPrefs.SetString("datacollection", "true");
                    Debug.Log("Data Collection is enabled!");
                    break;
                    // Returns to the main menu... as straightforward as that.
                case "k":
                    BacktoMainMenu();
                    break;
                    // Play the specific music clip the flag options signal
                case string m when Regex.IsMatch(m, "^m[0-9]{2}$"):
                    int music_index = int.Parse(flag[x].Substring(1, 2));
                    audio_director.PlayMusic(music_index);
                    break;
                    // Progress thorugh the next scene or the next if its the last scene in an act. This has a special interaction with the reset scenario, where it is ignored in favour of the reset scenario's own changes to act_index and play_index.
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
                    // if autosave is true it also saves.
                    if(PlayerPrefs.GetString("autosave") == "true")
                    {
                        SaveGame();
                    }
                    audio_director.LoadSceneAudio(so_director.current_scene.scene_sfx);
                    break;
                // Play the scene sfx whose index in the scene_sfx array matches with the flag options
                case string s when Regex.IsMatch(s, "^s[0-9]{2}$"):
                    int sfx_index = int.Parse(flag[x].Substring(1, 2));
                    audio_director.PlaySceneSFX(sfx_index);
                    break;
                    // Decrease the timer by one. Easy as that.
                case "t":
                    timer_director.TimerCountdown();
                    break;
                    // based on the conditions, flag U sends you to the credits scene with a different title attached to it. Either a GameOver or a Thanks for Playing.
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
                    // the X flag harvests data from those succulent and ripe gamers, like Daddy Bezos intended. Of course the data is safely protected.
                case "x":
                    if(PlayerPrefs.GetString("datacollection") == "true")
                    {
                        Debug.Log("Data is being collected!");
                        gamedata += ("Player has reached this index: " + (index - 1) + " in this scenario " + (act_index + 1) + " of the act number " + play_index + ", at the attempt number " + timer_director.Get_Attempt_Number() + "\n");
                        info_handler.SaveData(gamedata);
                    }
                    break;
                // Z is a unique flag. When used with a flag option differnt than zero, it attempts to update the chapter progression index of the game. When it does, then that's it, that's the new maximum point you have reached in the game.
                // When it's used with flag 0, it instead checks your progress in the Memory Gauntlet in the reset scenario. Based on your chapter progression index and the passwords you know, you are sent to the last known point up until you can remember
                // the passwords for (IE, you will be sent to chapter 3 if you meet these conditions: have reached chapter 3 already and in the memory gauntlet have gone through each and every password of chapter 1 and chapter 2).
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

    // Starts game, simply and normally. Sets the Attempt Timer to 20.
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
    // Resumegame is the alt-goth version of start game, because it just swings in a differnt direciton man...
    // Stupidities aside, it's a different versionof start game used when the game is loaded
    public void resumeGame()
    {
        so_director.ChangeScene(sot_play.all_acts[play_index].act_elements[act_index]);
        so_director.ChangeScenarioText(index);
        text_check = so_director.current_scene.text[index];
        progressText = true;
        audio_director.LoadSceneAudio(so_director.current_scene.scene_sfx);
    }

    // Start is called before the first frame update
    // WHen start is called and load is one, every information is unpacked and then distributed around the various parts of the game that might need them.
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

            index = int.Parse(savedata_array[1]);                               // we have the index here, to tell us where in the text are we
            act_index = int.Parse(savedata_array[2]);                           // at which scene
            play_index = int.Parse(savedata_array[3]);                          // and in which scenario
            so_director.exit_index = int.Parse(savedata_array[4]);              // it also, conveniently, shows you the current exit as saved here
            timer_director.Set_Attempt_Timer(int.Parse(savedata_array[5]));     // saves your attempt timer
            timer_director.Set_Attempt_Number(int.Parse(savedata_array[6]));    // and your attempt number
            timer_director.TimerCheck();                                        // then it runs a quick little check here
            hook_index = int.Parse(savedata_array[7]);                          // it saves the current hook index, so you can resume from wherever you like
            so_director.hook_exit_index = hook_index;                           // it saves the hook exit index
            so_director.input_index = int.Parse(savedata_array[8]);             // the input index
            chapter_progression_index = int.Parse(savedata_array[9]);           // and the chapter progression index
            Debug.Log(so_director.input_index);
            Debug.Log(hook_index);
            gamedata = info_handler.LoadData();                                 // and if you have any data, it loads that data in gamedata
            resumeGame();                                                       // then the game resumes

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
        // If this is valid, then reset the game
        if(timer_director.reset)
        {
            AttemptReset();
        }

        // this allows you to pass through the game by pressing space bar.
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

    // This simply divides the text using a matcher and a filler pattern, to extrapolate the savefile
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

    // Saving the game requires a bit of extra things from a lot of different scripts so they are all gathered here
    private void SaveGame()
    {
        SOT_Scene current_scene = so_director.current_scene;
        int exit_index = so_director.exit_index;
        int timer = timer_director.Get_Attempt_Timer();
        int password_index = so_director.input_index;
        info_handler.SaveGame(current_scene, index, act_index, play_index, exit_index, timer, 0, hook_index, password_index, chapter_progression_index);

        ui_director.SystemText("Game Saved!", 5);
    }

    // ProgressGame is used regularly just to read the flags but as a duplicitous function when an input field is out, to check if that's correct to give it it's unique properties. Otherwise, it attempts to read the flag of the input line (which is just a c, that continues into the reset).
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

    // This just hides the passcode field.
    public void HideInput()
    {
        passcode_field.SetActive(false);
    }

    // Save the game if autosave is on, then reset the game. To do that, use the various functions in so_director and ui director. Plus some extra setup like setting progressText to false and the act and play index to go to act 0 of chapter 1
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

    // This just waits for the canvas to finish fading, then clears the screen and spawns the canvas.
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

    // This waits for the canvas to spawn and then does a series of setup funtions, such as setting the exit_index, the input_index, the hook_exit index, etc. all to 0, as well as setting the attempt timer back to 20.
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

    // This just goes back to the main menu
    private void BacktoMainMenu()
    {
        audio_director.music_audio_source.Stop();
        SceneManager.LoadScene("Main Menu");
    }
    
}
