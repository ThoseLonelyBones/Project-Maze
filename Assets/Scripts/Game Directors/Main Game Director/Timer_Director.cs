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
    private bool stage0 = true, stage1 = true, stage2 = true, stage3 = true, stage4 = true;

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

    private void TimerCheck()
    {
        //Debug.Log("Timer Check!");

        if(Attempt_Timer >= 30 && stage0)
        {
            Debug.Log("Game has Started");
            stage0 = false;
        }
        else if(Attempt_Timer > 30 && Attempt_Timer < 20 && stage1)
        {
          Debug.Log("Timer Stage 1 Reached");
          stage1 = false;
          // Display Text: The sun is still high in the sky
          // Play: At Rest
        }
        else if (Attempt_Timer <= 20 && Attempt_Timer > 10 && stage2)
        {
            Debug.Log("Timer Stage 2 Reached");
            stage2 = false;
            // Display Text: The sun is lower than you remember
            // Play: Worried
        }
        else if (Attempt_Timer <= 10 && Attempt_Timer > 5 && stage3)
        {
            Debug.Log("Timer Stage 3 Reached");
            stage3 = false;
            // Display Text: The sun is beginning to set
            // Play: Anxious
        }
        else if(Attempt_Timer <= 5 && stage4)
        {
            Debug.Log("Timer Stage 4 Reached");
            stage4 = false;
            // Display Text: There is but a sliver of daylight left
            // Play: Panic
        }
        else if(Attempt_Timer == 0 && !stage4)
        {
            Debug.Log("Game End");
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
        stage4 = true;
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
