using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer_Director : MonoBehaviour
{
    /* One of, if not, the most important variable in the entire project. Attempt_Timer determines when the timer is supposed to tick down and all functions related to the timer moving up
    *  or down in the project, such as music changing, certain text displaying, etc. Once it reaches 0, displays a unique Scenario and then resets the game.
    *  Timer gets decreased when the flag T is called during a certain piece of text. In case for buttons, the flag is called AFTER the choice is made.
    *  Certain events may be gatekept by time, using flag G.
    */
    [SerializeField]
    private int Attempt_Timer;
    private int Test_Timer;

    [SerializeField]
    private int Attempt_Number;

    [SerializeField]
    private bool stage0 = true, stage1 = true, stage2 = true, stage3 = true;

    private Audio_Director audio_director;

    [SerializeField]
    private SO_Director so_director;

    public bool reset = false;

    // Getter and Setter for Attempt_Timer
    public int Get_Attempt_Timer()
    {
        return Attempt_Timer;
    }
    public void Set_Attempt_Timer(int timer)
    {
        Attempt_Timer = timer;
        Test_Timer = timer;
    }

    public int Get_Attempt_Number()
    {
        return Attempt_Number;
    }

    public void Set_Attempt_Number(int attempt)
    {
        Attempt_Number = attempt;
    }

    public void TimerCheck()
    {
        //Debug.Log("Timer Check!");

        if(Attempt_Timer >= 30 && stage0)
        {
            Debug.Log("Game has Started");
            audio_director.PlayMusic(2);
            stage0 = false;
        }
        else if(Attempt_Timer < 30 && Attempt_Timer >= 20 && stage1)
        {
          Debug.Log("Timer Stage 1 Reached");
            audio_director.PlayMusic(3);
            stage1 = false;
          // Display Text: The sun is still high in the sky
          // Play: At Rest
        }
        else if (Attempt_Timer < 20 && Attempt_Timer >= 10 && stage2)
        {
            Debug.Log("Timer Stage 2 Reached");
            audio_director.PlayMusic(4);
            stage2 = false;
            // Display Text: The sun is lower than you remember
            // Play: Worried
        }
        else if (Attempt_Timer < 10 && Attempt_Timer > 0 && stage3)
        {
            Debug.Log("Timer Stage 3 Reached");
            audio_director.PlayMusic(5);
            stage3 = false;
            // Display Text: The sun is beginning to set
            // Play: Anxious
        }
        else if(Attempt_Timer == 0)
        {
            Debug.Log("Game End");
            audio_director.StopMusic();
            reset = true;
            // Disable Save, Options, Spacebar.
            // Lower everything's Alpha
            // Restart from predetermined Index
            // End the Current Attempt
        }

    }

    public void TimerCountdown()
    {
        if(Attempt_Timer != 0)
        {
            Attempt_Timer--;
        }
        else
        {
            Debug.Log("Game has ended, TimerCountdown() called after it was 0");
            // End the game, show message
        }
        
    }

    public void AttemptCountup()
    {
        Attempt_Number++;
        Debug.Log("Attempt Number: " + Attempt_Number + 1 + "failed, resuming the game from the start.");
    }

    private void Start()
    {
        stage0 = true;
        stage1 = true;
        stage2 = true;
        stage3 = true;

        GameObject audiodirector = GameObject.Find("AudioDirector");
        audio_director = audiodirector.GetComponent<Audio_Director>();
    }

    private void Update()
    {
        if(Test_Timer > Attempt_Timer)
        {
            TimerCheck();
            Test_Timer = Attempt_Timer;
        }
    }
}
