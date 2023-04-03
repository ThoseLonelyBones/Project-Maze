using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Gameplay_Director : MonoBehaviour
{
   // private Writing_Director    writing_director; 
    private SO_Director         so_director;
    public  SOT_Play            sot_play;
    public Timer_Director      timer_director;

    public static GameObject       button1,
                                   button2,
                                   button3,
                                   button4;

    public GameObject       textDisplay;

    public Button           button_1,
                            button_2,
                            button_3,
                            button_4;


    public int              button1Exit,
                            button2Exit,
                            button3Exit,
                            button4Exit;


    public TextMeshProUGUI TextDisplay;

    public TextMeshProUGUI  button1Text,
                            button2Text,
                            button3Text,
                            button4Text;

    public GameObject       passcode_field;

    public bool             progressText = true;
    public bool            alternateScene = false;
    public bool             isResponse = false;


    private string          button_response;
    public string           text_check;

    public SOT_Scene scene;

    [SerializeField]
    private int             hook_index;

    [SerializeField]
    private UI_Director     ui_director;

    [SerializeField]
    private Information_Handler info_handler;
    

    /*
     * One of the most important variables in the entire game. Index is used to navigate through the multitude of text that are present in the game. It's like a bookmark, telling the program at which page they are
     * currently at. It's used in a multitude of functions in both this script and the SO_Director. It's handled with the utmost care and it's always beeing kept track of.
     * 
     */
    private int             index;
    private int             act_index = 0;
    private int             play_index = 0;

    private void Awake()
    {
        so_director = GetComponent<SO_Director>();
        timer_director = GetComponent<Timer_Director>();
        ui_director = GetComponent<UI_Director>();
        info_handler = GetComponent <Information_Handler>();
        if(timer_director == null)
        {
            Debug.Log("Timer Director not set");
        }
        //writing_director = GetComponent<Writing_Director>();
        

        button1 = GameObject.Find("Button 1");
        button2 = GameObject.Find("Button 2");
        button3 = GameObject.Find("Button 3");
        button4 = GameObject.Find("Button 4");

        button_1.onClick.AddListener(() => buttonClick("Button 1"));                       
        button_2.onClick.AddListener(() => buttonClick("Button 2"));                       
        button_3.onClick.AddListener(() => buttonClick("Button 3"));                       
        button_4.onClick.AddListener(() => buttonClick("Button 4"));                      

        Debug.Log("Awoke");

    }

    // This function is used in the Demo Pre-Alpha to hide elements of the game before the first scenario is loaded
    public void demoHide()
    {
        passcode_field.SetActive(false);
        buttonsHide();
    }

    // This function hides the text buttons used during the game
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

        for(int x = 0; x < 4; x++)
        {
            if(exits[x] > 0)
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
                    break;
                case "Button 2":
                    so_director.ChangeScenarioText(button2Exit);
                    text_check = so_director.ChangeScenarioText(button2Exit);
                    index = button2Exit;
                    break;
                case "Button 3":
                    so_director.ChangeScenarioText(button3Exit);
                    text_check = so_director.ChangeScenarioText(button3Exit);
                    index = button3Exit;
                    break;
                case "Button 4":
                    so_director.ChangeScenarioText(button4Exit);
                    text_check = so_director.ChangeScenarioText(button4Exit);
                    index = button4Exit;
                    break;
                default:
                    Debug.Log("How could this happen to me?");
                    break;
            }
        }

        buttonsHide();
        progressText = true;
    }

    public void readFlag(char[] flag)
    {
        for(int x = 0; x < flag.Length; x++)
        {
            switch (flag[x])
            {
                case 'a':
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
                case 'b':
                    buttonsSet(so_director.CallExits());
                    break;
                case 'c':
                    index++;
                    text_check = so_director.ChangeScenarioText(index);
                    break;
                case 'f':
                    text_check = so_director.ChangeScenarioText(hook_index);
                    index = hook_index;
                    break;
                case 'h':
                    hook_index = index;
                    break;
                case 'i':
                    // Input stuff
                    break;
                case 'r':
                    if (act_index == sot_play.all_acts[play_index].act_elements.Length)
                    {
                        play_index++;
                        act_index = 0;
                        index = 0;
                    }
                    else
                    {
                        act_index++;
                        index = 0;
                    }
                    so_director.ChangeScene(sot_play.all_acts[play_index].act_elements[act_index]);
                    Debug.Log(play_index + " is play index and " + act_index + " is act index");
                    text_check = so_director.ChangeScenarioText(index);
                    break;
                case 's':
                    SOT_Scene current_scene = so_director.current_scene;
                    int exit_index          = so_director.exit_index;
                    int timer               = timer_director.Get_Attempt_Timer();
                    info_handler.SaveGame(current_scene, index, act_index, play_index, exit_index, timer, 0);

                    string save_data_test = info_handler.LoadGame();
                    Debug.Log(save_data_test);

                    break;
                case 't':
                    timer_director.TimerCountdown();
                    // do the return and text change in here.
                    break;
                default:
                    Debug.Log("How has this even happened? Is this even possible?");
                    break;
            }
        }
        
    }

    // This function is used to start game. This function will include random generation in itself further along development.
    public void startGame()
    {

        index = so_director.ChangeScene(sot_play.all_acts[play_index].act_elements[act_index]);
        so_director.ChangeScenarioText(index);

        text_check = so_director.current_scene.text[index];
        progressText = true;

        timer_director.Set_Attempt_Timer(30);
    }

    // Start is called before the first frame update
    void Start()
    {
        demoHide();
        startGame();
        Debug.Log("Started");
    }

    // Update is called once per frame
    void Update()
    {                                                          

        if (Input.GetKeyDown(KeyCode.Space) && progressText)
        {
            if(TextDisplay.text == so_director.text_to_write)                                          // Change to saved text (which can vary in case of a response) <= done
            {
                if(!isResponse)
                {
                    readFlag(so_director.SceneFlags(index));
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

                TextDisplay.text = so_director.text_to_write;              // Skips text writing program when it will be done.
            }
        }

    }

    // TODO:
    /*
     *  1) Integrate Inputs via Input Flag
     *  2) Fully finish timer implementation
     *  3) Create SFX and VFX directors
     * 
     * 
     * 
     */
}
