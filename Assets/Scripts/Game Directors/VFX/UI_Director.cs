using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Director : MonoBehaviour
{
    [SerializeField]
    private GameObject[] ChoiceButtons;

    [SerializeField]
    private HorizontalLayoutGroup button_align;

    [SerializeField]
    private CanvasGroup system_text_group;

    [SerializeField]
    private TextMeshProUGUI sys_text;

    private bool fade;
    public bool canvas_active = true, canvas_spawn = false;

    private Audio_Director audio_director;

    private GameObject game_canvas_object;

    private Canvas game_canvas;
    [SerializeField]
    private CanvasGroup game_canvas_group;

    [SerializeField]
    private Button savebutton, optionsbutton;

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
                ChoiceButtons[x].GetComponent<LayoutElement>().ignoreLayout = !active_buttons[x];
                ChoiceButtons[x].SetActive(active_buttons[x]);
            }

        }

        button_align.SetLayoutHorizontal();

    }

    public void SystemText(string text, int time)
    {
        system_text_group.alpha = 1;
        StartCoroutine(FadeOut(time));
    }

    // This is tied to framerate, needs to be changed.
    private void Update()
    {

    }

    public void DisableButtons()
    {
        savebutton.interactable = false;
        optionsbutton.interactable = false;
    }

    public void EnableButtons()
    {
        savebutton.interactable = true;
        optionsbutton.interactable = true;
    }

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

    public void CanvasFade()
    {
        float alpha_start = game_canvas_group.alpha;
        StartCoroutine(CanvaseFadeOut(5f, alpha_start));
    }

    public void CanvasSpawn()
    {
        game_canvas.gameObject.SetActive(true);
        StartCoroutine(CanvasFadeIn(2f));
    }

    IEnumerator CanvaseFadeOut(float time, float alpha_start)
    {
        float deltatime = 0f;

        while(deltatime < time)
        {
            float newalpha = Mathf.Lerp(alpha_start, 0f, deltatime / time);
           game_canvas_group.alpha = newalpha;
            deltatime += Time.deltaTime;
            yield return null;
        }

        game_canvas.gameObject.SetActive(false);
        canvas_active = false;
    }

    IEnumerator CanvasFadeIn(float time)
    {
        float deltatime = 0f;

        while(deltatime < time)
        {
            float newalpha = Mathf.Lerp(0f, 1f, deltatime / time);
            game_canvas_group.alpha = newalpha;
            deltatime += Time.deltaTime;
            yield return null;
        }

        canvas_active = true;
        canvas_spawn = true;

    }




}
