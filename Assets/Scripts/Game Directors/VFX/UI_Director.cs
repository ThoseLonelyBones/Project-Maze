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

    private void Start()
    {

    }
    public void ButtonAlign(bool[] active_buttons)
    {
        for(int x = 0; x < active_buttons.Length; x++)
        {
            if(ChoiceButtons[x].GetComponent<LayoutElement>() == null)
            {
                Debug.Log("how");
            }
            else
            {
                ChoiceButtons[x].GetComponent<LayoutElement>().ignoreLayout = !active_buttons[x];
                ChoiceButtons[x].SetActive(active_buttons[x]);
            }

        }

        button_align.SetLayoutHorizontal();

    }


}
