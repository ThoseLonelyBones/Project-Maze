using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// A simple script that runs the credit scene
public class CreditsScript : MonoBehaviour
{
    // This integer is used to determine whether or not the Credits Scene should display a congratulations message of sorts or not, based on which finale you got... as well as seing if you got a finale.
    [SerializeField]
    int finale;

    // The textbox as an object
    [SerializeField]
    GameObject text_box;

    // Textbox and credits text as TMP, used to write their contents
    [SerializeField]
    TMP_Text textBox, credits_text;

    public Writing_Director writing_director;
    
    // Used specifically to give a new fontsize to credits.
    int fontsize;

    private Audio_Director director;

    void Awake()
    {
        // Set fontsize to medium. Otherwise it breaks on the bigger font.
        fontsize = 46;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Based on the CreditsScene, either display a thanks for playing, game over screen or directly the credits
        switch (PlayerPrefs.GetInt("credits_scene"))
        {
            case 0:
                DisplayCredits();
                break;
            case 1:
                StartCoroutine(ThanksForPlaying());
                break;
            case 2:
                StartCoroutine(GameOver());
                break;
            default:
                break;

        }

        // Initialization
        GameObject audiodirector = GameObject.Find("AudioDirector");
        director = audiodirector.GetComponent<Audio_Director>();

    }

    // Update is called once per frame
    void Update()
    {   
        // I, for the life of me, couldn't get the button to work. I swear I tried everything, it just didn't. So Spacebar it is.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            returnToMenu();
        }
    }
    // This function returns you to the main menu and stops the music. The music restarts on the menu screen.
    public void returnToMenu()
    {
        PlayerPrefs.SetInt("credits_scene", 0);
        Debug.Log("Return to Main Menu!");
        SceneManager.LoadScene("Main Menu");

        director.music_audio_source.Stop();
    }

    // Starts the Coroutine to display the credits.
    void DisplayCredits()
    {
        StartCoroutine(DisplayCreditsCoroutine());

    }
    // The coroutine for the gameover effect... plus the credits.
    IEnumerator GameOver()
    {
        PlayerPrefs.SetInt("textsize", 80);
        writing_director.TypingEffect(textBox, "<align=center><color=#FF0000>Game Over...</align></color>");
        yield return new WaitForSeconds(3f);
        StartCoroutine(DisplayCreditsCoroutine());
    }

    // The coroutine for the Thanks for Playing screen... plus the credits.
    IEnumerator ThanksForPlaying()
    {
        PlayerPrefs.SetInt("textsize", 80);
        writing_director.TypingEffect(textBox, "<align=center>Thanks for Playing!</align>");
        yield return new WaitForSeconds(3f);
        StartCoroutine(DisplayCreditsCoroutine());
    }

    // I KNOW. I KNOW. The credits are hardcoded in, I should have maybe used another thing like a scriptable object like for the rest of the game but I was running out of time. I'd rather have a bad solution than not have a solution at all.
    IEnumerator DisplayCreditsCoroutine()
    {
        PlayerPrefs.SetInt("textsize", fontsize);
        credits_text.text = "<align=center><size=150%>CREDITS:</size></align>";
        textBox.text = "";
        writing_director.TypingEffect(textBox, "<align=center>Development, Concept, Design, Programming, Implementation: \n Fabio 'LonelyBones' Carrara \n Student Number 1903722 \n Abertay University 2022/2023\n \n" +
                                               "<i>\"If it's not fun... why bother?\"</i> \n Reggie Fils-Aime, Former President of Nintendo America \n ");
        yield return new WaitForSeconds(15f);
        textBox.text = "";
        writing_director.TypingEffect(textBox, "<align=center>Main Menu Music and Main Gameplay Music by: Andrea 'Ækold' Frascella \n aekold.bandcamp.com \n \n" +
                                               "<i>\"How did you guess I'm not from Berklee?\"</i> \n \n" +
                                               "Main Game Logo and Title Artwork: Jerry 'Reeso' Huang \n https://twitter.com/Reesoooooo \n \n" +
                                               "<i> \"Pixel Art is fun \\s\" </i> \n");
        yield return new WaitForSeconds(15f);
        textBox.text = "";
        writing_director.TypingEffect(textBox, "<align=center>VOICE ACTING \n" +
                                               "Emperor Dearion: Andrea 'JustTry' Leone \n https://youtube.com/@voxmea \n" +
                                               "<i>\"Laugh and the world laughs with you. Weep and you weep alone\"</i> \n E.W. Wilcox - Solitude \n \n" +
                                               "Court Jester Maurice: Mauro 'ZioNemmee' Galimberti \n \n <i> \"Heyoo!\"</i> \n" +
                                               "Steve, Borderlands Franchise\n");
        yield return new WaitForSeconds(15f);
        textBox.text = "";
        writing_director.TypingEffect(textBox, "<align=center>Additional Music from Pixabay (Royalty Free Music) \n https://pixabay.com/music/ \n" +
                                               "Sound Effects from Zapsplat (Royalty Free Sound Effects) \n https://www.zapsplat.com/ \n" +
                                               "Fonts used: \n" +
                                               "KongText \n https://www.dafont.com/kongtext.font \n" +
                                               "Atlantis Font Family \n https://www.1001fonts.com/atlantis-font.html");
        yield return new WaitForSeconds(15f);
        credits_text.text = "<align=center><size=150%>Rattle me Bones!</size>";
        textBox.text = "";
        writing_director.TypingEffect(textBox, "<align=center>... once again, thank you for the opportunity! Hope you will enjoy or have already enjoyed Horn of Ammon! \n" +
                                               "<size=60%> smol indie dev pls no bulli :)</size>");
        yield return new WaitForSeconds(10f);
        returnToMenu();
    }
}
