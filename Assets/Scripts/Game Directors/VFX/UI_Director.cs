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

    private void Start()
    {

    }

    public void ButtonAlign(bool[] active_buttons)
    {
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




}
