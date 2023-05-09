using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Input_Handler : MonoBehaviour
{
    // Simple, just see the input and the password of the current input
    private string input;
    private string password;

    // Check if you are allowed to progress, to set inputs or if the password is correct or not.
    public  bool   progress;
    public bool inputoff = false;
    public bool correct_password = false;

    // Sets the colour of the placeholder, this is used to change its alpha and the alpha of the input bar. Remember that the input bar still needs to be active to work, so the best fix is to make it transparent.
    Color newColor = Color.white;

    // Various inputfield, placeholders and objects, initialized and used throughout the script.
    [SerializeField]
    public TMP_InputField inputfield;

    [SerializeField]
    private TMP_Text placeholderfield;

    [SerializeField]
    private GameObject placeholder;

    [SerializeField]
    private GameObject inputfieldobject;

    private string textshown;

    [SerializeField]
    private SO_Director so_director;

    // Start is called before the first frame update
    void Start()
    {
        // Set alpha to 1, make the inputbar visible!
        newColor.a = 1f;
        Cleanse();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // This is a fix for a crash that occurs if you backspace in an input field.
    public void BackspaceFix(string text)
    {
        if (Input.GetKeyDown(KeyCode.Backspace) && string.IsNullOrEmpty(text))
        {
            StartCoroutine(BackspaceFixCoroutine());
        }
    }
    
    // To give the illusion that the password is actually a series of characters you are inputting in (similar to old password terminals or the classic mental image of '_ _ _ _ _ ' symbolizing a password (to further enhance the retro feel)) a placeholder
    // element, identical to the actual index input is spawned on top of the input bar. Here I can edit the text all I want without the game throwing a royal hissy fit.
    public void PlaceholderActive()
    {
        placeholder.SetActive(true);
        newColor.a = 0f;
        inputfield.image.color = newColor;
    }

    // This fix is required to make backspace not crash the game in the input field. It's a fairly simple thing that instead of directly editing the input bar it changes its text to a substring of the characters - 1.
    IEnumerator BackspaceFixCoroutine()
    {
        yield return new WaitForEndOfFrame();

        int characters = inputfield.text.Length;
        if(characters > 0)
        {
            inputfield.text = inputfield.text.Substring(0, characters - 1);
        }
        
    }
    // This is applied on input submission on the input bar. This checks whether or not the input is correct and, based on that, determines what to progress and what to do.
    public void ReadInput(string player_input)
    {
        input = player_input;
        Debug.Log("Retriving Input: " + input);
        inputfield.text = "";
        Cleanse();
        placeholder.SetActive(false);

        if(input == so_director.current_scene.password[so_director.input_index])
        {
            correct_password = true;
        }
        else
        {
            correct_password = false;
            so_director.input_index = 0;
        }

        progress = true;
        inputoff = true;

        //return input;
    }

    // This is used to give the Password Looking effect on the input bar.
    public void PasswordEffect(string input)
    {
        InputFormatter();
    }

    // Cleanse is used to update the input bar and either make it visible or make it invisible depending on it current status
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

    // This takes the inputfield text, aka what the player is typing in and then creates an edited input string, which is made of input text + fill the remaining space with _ chracters for the length of inputfield's character limit (aka the password's length)
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
