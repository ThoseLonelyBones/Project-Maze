using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script governs time. No, really, this script governs how time's passage is calculated in the game. Simply put, it takes a very important variable, called Attempt_Timer and calculates how much progress the player has made based on it.
// It changes the music, adds additional effects based on whatever the player is currently doing.
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

    // The number of attempts the player has taken. Not super important but it's saved as part of the userdata collected (the averege amount of attempts will be put in the dissertation.
    [SerializeField]
    private int Attempt_Number;

    // These bools are used to determine the progress of the timer but not to repeat the various transitions between stages
    [SerializeField]
    private bool stage0 = true, stage1 = true, stage2 = true, stage3 = true;

    private Audio_Director audio_director;

    [SerializeField]
    private SO_Director so_director;

    // Reset determines whether or not the timer has ticked down to zero. Reset activates the reset function.
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

    // Getter and Setter for Attempt_Number
    public int Get_Attempt_Number()
    {
        return Attempt_Number;
    }
    public void Set_Attempt_Number(int attempt)
    {
        Attempt_Number = attempt;
    }

    // This function is called in update. This checks whether or not the Timer has ticked down to a certain points and if any flags need to be applied.
    public void TimerCheck()
    {

        if(Attempt_Timer >= 15 && stage0)
        {
            Debug.Log("Game has Started");
            audio_director.PlayMusic(6);
            stage0 = false;
        }
        else if(Attempt_Timer < 15 && Attempt_Timer >= 10 && stage1)
        {
          Debug.Log("Timer Stage 1 Reached");
            audio_director.PlayMusic(7);
            stage1 = false;

        }
        else if (Attempt_Timer < 10 && Attempt_Timer >= 5 && stage2)
        {
            Debug.Log("Timer Stage 2 Reached");
            audio_director.PlayMusic(8);
            stage2 = false;

        }
        else if (Attempt_Timer < 5 && Attempt_Timer > 0 && stage3)
        {
            Debug.Log("Timer Stage 3 Reached");
            audio_director.PlayMusic(9);
            stage3 = false;
        }
        else if(Attempt_Timer == 0)
        {
            Debug.Log("Game End");
            reset = true;

        }

    }

    // The TimerCountdown is what the GameplayScript's flag T uses to trickle down the timer, -1 at the time.
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

    // Called on reset, when this happens the number of attempts increases.
    public void AttemptCountup()
    {
        Attempt_Number++;
        Debug.Log("Attempt Number: " + Attempt_Number + 1 + "failed, resuming the game from the start.");
    }

    // Initialization
    private void Start()
    {
        stage0 = true;
        stage1 = true;
        stage2 = true;
        stage3 = true;

        GameObject audiodirector = GameObject.Find("AudioDirector");
        audio_director = audiodirector.GetComponent<Audio_Director>();
    }

    // On update, ONLY IF THE TIMER HAS CHANGED, run TimerCheck. Otherwise your PC would take flight.
    private void Update()
    {
        if(Test_Timer > Attempt_Timer)
        {
            TimerCheck();
            Test_Timer = Attempt_Timer;
        }
    }
}
