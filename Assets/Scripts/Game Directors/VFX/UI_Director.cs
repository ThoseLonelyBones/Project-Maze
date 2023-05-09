using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// UI Director is what governs the Correct displaying of various buttons (their orientation and alignment as they are being inserted into the game) and other smaller features, like Unique System text for the SaveFile Function.
public class UI_Director : MonoBehaviour
{   
    // The Game_Objects and the Layout Group they are inserted in, this is used to align correctly the buttons as they are displayed in the game.
    [SerializeField]
    private GameObject[] ChoiceButtons;
    [SerializeField]
    private HorizontalLayoutGroup button_align;

    // This is used to find and remove the system text group during blackouts
    [SerializeField]
    private CanvasGroup system_text_group;
    // TMP for the system text
    [SerializeField]
    private TextMeshProUGUI sys_text;

    // Used in the Fade Canvas trick from the game's reset, this is very important to have working corectly: various bools determine the current state of the canvas or what it's supposed to do (is it active or not, does it need to start spawning in or not)
    // Everything else is either an element required to disappear the canvas or a canvas group to retrieve all elements of the game at once.
    private bool fade;
    public bool canvas_active = true, canvas_spawn = false;
    private Audio_Director audio_director;
    private GameObject game_canvas_object;
    private Canvas game_canvas;
    [SerializeField]
    private CanvasGroup game_canvas_group;
    [SerializeField]
    private Button savebutton, optionsbutton;

    // Initialization of various functions and scripts in here
    private void Start()
    {
        GameObject audiodirector = GameObject.Find("AudioDirector");
        audio_director = audiodirector.GetComponent<Audio_Director>();
        if (audio_director == null)
        {
            Debug.Log("Audio Director not set!");
        }


        game_canvas_object = GameObject.Find("Canvas");
        game_canvas = game_canvas_object.GetComponent<Canvas>();

        if (game_canvas == null)
        {
            Debug.Log("Game Canvas not found");
        }

    }


    // This function is used to align buttons! They are positioned inside their layout element, then they are either made to ignore the layout (if not active) or to be set active.
    public void ButtonAlign(bool[] active_buttons)
    {
        Debug.Log(active_buttons.Length);

        for(int x = 0; x < active_buttons.Length; x++)
        {
            if(ChoiceButtons[x].GetComponent<LayoutElement>() == null)
            {
                Debug.Log("This should physically be impossible.");
            }
            else
            {
                // This is used to find and activate the layout buttons that need to be part of the layout. If there is somehow an unactive button in here, it will be made to ignore the layout.
                ChoiceButtons[x].GetComponent<LayoutElement>().ignoreLayout = !active_buttons[x];
                ChoiceButtons[x].SetActive(active_buttons[x]);
            }

        }
        // Then afterwards, the layout is set to horizontal
        button_align.SetLayoutHorizontal();

    }

    // System Text is used to display system text.
    public void SystemText(string text, int time)
    {
        system_text_group.alpha = 1;
        StartCoroutine(FadeOut(time));
    }

    private void Update()
    {

    }

    // Disables buttons for the fade-out function
    public void DisableButtons()
    {
        savebutton.interactable = false;
        optionsbutton.interactable = false;
    }

    // Enables buttons for the fade-in function
    public void EnableButtons()
    {
        savebutton.interactable = true;
        optionsbutton.interactable = true;
    }

    // Fade Out Coroutine, used to diminsh the alpha of the system text group.
    IEnumerator FadeOut(int time)
    {
        yield return new WaitForSeconds(time);
        fade = true;
        while(fade)
        {
            system_text_group.alpha -= 0.2f;
            yield return new WaitForSeconds(0.05f);
        }
        fade = false;
    }

    // Canvas Fade starts the coroutine of CanvasFadeOut, which takes in a time and a starting alpha to make the Canvas disappear and then reappear for the players as they faint
    public void CanvasFade()
    {
        float alpha_start = game_canvas_group.alpha;
        StartCoroutine(CanvaseFadeOut(5f, alpha_start));
    }

    // The opposite function to CanvasFadeOut, Canvas Spawn sets the canvas as active and then starts the fade in
    public void CanvasSpawn()
    {
        game_canvas.gameObject.SetActive(true);
        StartCoroutine(CanvasFadeIn(2f));
    }

    // Initiates a CanvasFadeOut, taking a total time and a starting alpha
    IEnumerator CanvaseFadeOut(float time, float alpha_start)
    {
        // sets deltatime to 0f
        float deltatime = 0f;

        // As long as deltatime is under the total time required...
        while(deltatime < time)
        {
           // Use Linear Estrapolation to get the transition effect (aka, transitionary alpha) to look real nice and then...
           float newalpha = Mathf.Lerp(alpha_start, 0f, deltatime / time);
           // Set the old canvas alpha to the new alpha...
           game_canvas_group.alpha = newalpha;
           // Increase Deltatime by... well, deltatime. This is used in Coroutines to check how much time has passed from the original deltatime. This means that this function runs a BUNCH of times, giving a good transition effect over the time period.
           // The transition looks smooth and on time because Linear Estrapolation takes in as an argument both the deltatime and the time, which in turn can regulate how much the canvas' alpha should change at any given instance the function is ran.
           deltatime += Time.deltaTime;
           yield return null;
        }
        // Disable Canvas until it is reappeared with the FadeIn
        game_canvas.gameObject.SetActive(false);
        canvas_active = false;
    }
    // Initiates a CanvasFadeIn, taking in a total time
    IEnumerator CanvasFadeIn(float time)
    {
        // sets starting deltatime to 0
        float deltatime = 0f;

        // As long as deltatime is under the total time required...
        while (deltatime < time)
        {
            // Use Linear Estrapolation to get the transition effect (aka, transitionary alpha) to look real nice and then...
            float newalpha = Mathf.Lerp(0f, 1f, deltatime / time);
            // set the alpha to the new alpha. This is just like the previous function, but it insteads raises the Alpha instead of lowering it.
            game_canvas_group.alpha = newalpha;
            deltatime += Time.deltaTime;
            yield return null;
        }

        // The canvas is once again active and ready to be used!
        canvas_active = true;
        canvas_spawn = true;

    }




}
