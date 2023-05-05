using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsScript : MonoBehaviour
{
    [SerializeField]
    int finale;

    [SerializeField]
    GameObject text_box;

    [SerializeField]
    TMP_Text textBox, credits_text;

    public Writing_Director writing_director;

    int fontsize;

    private Audio_Director director;

    void Awake()
    {
        fontsize = PlayerPrefs.GetInt("textsize");
    }

    // Start is called before the first frame update
    void Start()
    {
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

        GameObject audiodirector = GameObject.Find("AudioDirector");
        director = audiodirector.GetComponent<Audio_Director>();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            returnToMenu();
        }
    }

    public void returnToMenu()
    {
        PlayerPrefs.SetInt("credits_scene", 0);
        Debug.Log("Return to Main Menu!");
        SceneManager.LoadScene("Main Menu");

        director.music_audio_source.Stop();
    }

    void DisplayCredits()
    {
        StartCoroutine(DisplayCreditsCoroutine());

    }

    IEnumerator GameOver()
    {
        PlayerPrefs.SetInt("textsize", 80);
        writing_director.TypingEffect(textBox, "<align=center><color=#FF0000>Game Over...</align></color>");
        yield return new WaitForSeconds(3f);
        StartCoroutine(DisplayCreditsCoroutine());
    }

    IEnumerator ThanksForPlaying()
    {
        PlayerPrefs.SetInt("textsize", 80);
        writing_director.TypingEffect(textBox, "<align=center>Thanks for Playing!</align>");
        yield return new WaitForSeconds(3f);
        StartCoroutine(DisplayCreditsCoroutine());
    }

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
