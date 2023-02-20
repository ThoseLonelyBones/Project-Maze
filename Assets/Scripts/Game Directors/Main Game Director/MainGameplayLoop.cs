using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainGameplayLoop : MonoBehaviour
{
    public SO_Director     so_director;
    public  SOT_Play        sot_play;

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

    public GameObject       title;

    public bool             progressText = true;
    private bool            alternateScene = false;
    public bool             isResponse = false;


    private string          button_response;
    public string           text_check;

    public SOT_Scene scene;
    

    /*
     * One of the most important variables in the entire game. Index is used to navigate through the multitude of text that are present in the game. It's like a bookmark, telling the program at which page they are
     * currently at. It's used in a multitude of functions in both this script and the SO_Director. It's handled with the utmost care and it's always beeing kept track of.
     * 
     */
    private int             index;

    private void Awake()
    {
        so_director = GetComponent<SO_Director>();                              // This isn't working, returns NULL.
        if(so_director == null)
        {
            Debug.Log("Ehm... we... don't have a director?");
        }

        button1 = GameObject.Find("Button 1");
        button2 = GameObject.Find("Button 2");
        button3 = GameObject.Find("Button 3");
        button4 = GameObject.Find("Button 4");

        Debug.Log("Awoke");

        //title = GameObject.Find("Title");
    }

    // This function is used in the Demo Pre-Alpha to hide elements of the game before the first scenario is loaded
    public void demoHide()
    {
        title.SetActive(false);
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
        for(int x = 0; x < 4; x++)
        {
            Debug.Log("ButtonSet exit for button n�" + (x + 1) + "is " + exits[x]);
            if(exits[x] > 0)
            {
                switch(x)
                {
                    case 0:
                        button1Exit = exits[x];
                        button_1.onClick.AddListener(() => buttonClick("Button 1"));                       // This may not work?
                        button1.SetActive(true);
                        break;
                    case 1:
                        button2Exit = exits[x];
                        button_2.onClick.AddListener(() => buttonClick("Button 2"));                       // This may not work?
                        button2.SetActive(true);
                        break;
                    case 2:
                        button3Exit = exits[x];
                        button_3.onClick.AddListener(() => buttonClick("Button 3"));                       // This may not work?
                        button3.SetActive(true);
                        break;
                    case 3:
                        button4Exit = exits[x];
                        button_4.onClick.AddListener(() => buttonClick("Button 4"));                       // This may not work?
                        button4.SetActive(true);
                        break;
                    default:
                        Debug.Log("There was supposed to be a cool error message here but I forgor :skull:");
                        break;
                }
            }
        }

        progressText = false;
    }

    public void buttonClick(string buttonname)
    {
        button_response = buttonname;

        if (so_director.current_scene is SOT_Dialogue)
        {
            Debug.Log("This current scene is a dialogue");
        }
        else
        {
            Debug.Log("The is flag just doesn't work!");
        }

        if (so_director.current_scene is SOT_Dialogue && !isResponse)
        {
            Debug.Log("This is a response!");
            text_check = so_director.DialogueResponse(buttonname);
            isResponse = true;
        }
        else
        {
            switch (buttonname)
            {
                case "Button 1":
                    so_director.ChangeScenarioText(button1Exit);
                    index = button1Exit;
                    break;
                case "Button 2":
                    so_director.ChangeScenarioText(button2Exit);
                    index = button2Exit;
                    break;
                case "Button 3":
                    so_director.ChangeScenarioText(button3Exit);
                    index = button3Exit;
                    break;
                case "Button 4":
                    so_director.ChangeScenarioText(button4Exit);
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




    public void readFlag(char flag)
    {
        switch(flag)
        {
            case 'a':
                if(!alternateScene)
                {
                    index = 0;
                    alternateScene = true;
                }
                else
                {
                    index = so_director.alternate_index;
                    so_director.alternate_index = 0;
                }
                return;
            case 'b':
                buttonsSet(so_director.CallExits());
                return;
            case 'c':
                index++;
                text_check = so_director.ChangeScenarioText(index);
                return;
            default:
                Debug.Log("How has this even happened? Is this even possible?");
                return;
        }
    }

    public void startGame()
    {
        /*
            SOT_Act start_act = sot_play.all_acts[0];
            if(start_act == null)
            {
                Debug.Log("Start Act is null?");
            }
            else
            {
                Debug.Log("Starting Act!");
            }
            SOT_Scene start_scene = start_act.act_elements[0];
            if(start_scene == null)
            {
                Debug.Log("Start Scene is null?");
            }
            else
            {
                Debug.Log("Starting Scene!");
            }

            index = so_director.ChangeScene(start_scene);
        */

        index = so_director.ChangeScene(sot_play.all_acts[0].act_elements[0]);
        so_director.ChangeScenarioText(index);

        text_check = so_director.current_scene.text[index];

        progressText = true;
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
        if(Input.GetKeyDown(KeyCode.Space) && progressText)
        {
            if(TextDisplay.text == text_check)               // Change to saved text (which can vary in case of a response)
            {
                if(!isResponse)
                {
                    readFlag(so_director.SceneFlags(index));
                }
                else
                {
                    buttonClick(button_response);
                    isResponse = false;
                }
            }
            else
            {
                TextDisplay.text = so_director.current_scene.text[index];               // Skips text writing program when it will be done.
            }
        }

    }
}
