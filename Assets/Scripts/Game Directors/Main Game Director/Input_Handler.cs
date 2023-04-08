using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Input_Handler : MonoBehaviour
{

    private string input;
    private string password;

    public  bool   progress;
    public bool inputoff = false;

    Color newColor = Color.white;

    [SerializeField]
    private TMP_InputField inputfield;

    [SerializeField]
    private TMP_Text placeholderfield;

    [SerializeField]
    private GameObject placeholder;

    [SerializeField]
    private GameObject inputfieldobject;

    private string textshown;

    // Start is called before the first frame update
    void Start()
    {
        //inputfield.onValueChanged.AddListener(BackspaceFix);
        newColor.a = 1f;
        inputfield.characterLimit = 10;

        Cleanse();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void BackspaceFix(string text)
    {
        if (Input.GetKeyDown(KeyCode.Backspace) && string.IsNullOrEmpty(text))
        {
            StartCoroutine(BackspaceFixCoroutine());
        }
    }

    public void PlaceholderActive()
    {
        placeholder.SetActive(true);
        newColor.a = 0f;
        inputfield.image.color = newColor;
    }

    IEnumerator BackspaceFixCoroutine()
    {
        yield return new WaitForEndOfFrame();

        int characters = inputfield.text.Length;
        if(characters > 0)
        {
            inputfield.text = inputfield.text.Substring(0, characters - 1);
        }
        
    }

    public void ReadInput(string player_input)
    {
        input = player_input;
        Debug.Log("Retriving Input: " + input);
        progress = true;
        inputoff = true;
        inputfield.text = "";

        Cleanse();
        placeholder.SetActive(false);
        //return input;
    }

    public void PasswordEffect(string input)
    {
        InputFormatter();
    }

    public void Cleanse()
    {
        progress = true;
        
        placeholderfield.text = new string('_', inputfield.characterLimit);

        if(newColor.a == 0f)
        {
            newColor.a = 1f;
        }
        else
        {
            newColor.a = 0f;
        }

    }

    public void InputFormatter()
    {
        string input_text = inputfield.text;
        int characters = inputfield.characterLimit - input_text.Length;

        if(characters < 0)
        {
            characters = 0;
        }
        string edited_input = input_text + new string('_', characters);

        placeholderfield.text = edited_input;

    }

}
