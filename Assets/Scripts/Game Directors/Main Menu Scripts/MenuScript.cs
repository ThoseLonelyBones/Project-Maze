using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuScript : MonoBehaviour
{
    [SerializeField]
    public GameObject start, load, options, credits;

    [SerializeField]
    public Button startbtn, loadbtn, optionsbtn, creditsbtn;

    [SerializeField]
    private UI_Director ui_director;

    [SerializeField]
    private Information_Handler info_handler;


    public GameObject textDisplay;
    public TextMeshProUGUI TextDisplay;

    // Start is called before the first frame update
    void Start()
    {
        ui_director = GetComponent<UI_Director>();
        info_handler = GetComponent<Information_Handler>();

        startbtn.onClick.AddListener(() => ClickStart());
        loadbtn.onClick.AddListener(() => ClickLoad());
        optionsbtn.onClick.AddListener(() => ClickOptions());
        creditsbtn.onClick.AddListener(() => ClickCredits());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ClickStart()
    {
        PlayerPrefs.SetInt("load", 0);
        SceneManager.LoadScene("Gameplay Scene");     
    }

    void ClickLoad()
    {
        string playersavedata = info_handler.LoadGame();
        if(playersavedata == null)
        {
            TextDisplay.text = "The game incurred in an issue while loading your savefile. Make sure one is present or available!";
        }
        else
        {
            PlayerPrefs.SetString("savedata", playersavedata);
            PlayerPrefs.SetInt("load", 1);
            SceneManager.LoadScene("Gameplay Scene");
        }
       
    }

    void ClickOptions()
    {

    }

    void ClickCredits()
    {

    }
}
