using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Writing_Director : MonoBehaviour
{
    private TMP_Text Textbox;
    public string Text;
    private float time_to_write;
    private float script_timer;

    private int char_pos;

    private void Awake()
    {
        Textbox = GameObject.Find("Text Display").GetComponent<TextMeshProUGUI>();
    }

    public void TypingEffect(TMP_Text textbox, string newText, float ttw)
    {
        Textbox = textbox;
        Textbox.text = "";
        Text = newText;
        char_pos = 0;
        time_to_write = ttw;
        StartCoroutine(Writing());
    }

    IEnumerator Writing()
    {
        while (char_pos < Text.Length && Textbox.text != Text)
        {
            Textbox.text += Text[char_pos].ToString();
            char_pos++;
            yield return new WaitForSeconds(time_to_write);                     // Need to make this work letter by letter (this writes more characters if the framerate allows it)
        }
    }

}
