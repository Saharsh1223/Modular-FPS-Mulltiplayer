using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public float timeLeft;
    [HideInInspector] public bool timerOn = true;
    [HideInInspector] public bool startedGame = false; 

    public TMP_Text timerText;
   
    private void Start()
    {
        Invoke("CountDown3", 3f);
        timerText.text = "Game starting soon...";
    }

    private void Update()
    {
        if(timerOn)
        {
            if(timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
                UpdateTimer(timeLeft);
            }
            else
            {
                timeLeft = 0;
                timerOn = false;
            }
        }
    }

    private void UpdateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private void CountDown3()
    {
        Invoke("CountDown2", 1f);
        timerText.text = "Game starting in 3";
    }
    
    private void CountDown2()
    {
        Invoke("CountDown1", 1f);
        timerText.text = "Game starting in 2";
    }
    
    private void CountDown1()
    {
        Invoke("CountDown0", 1f);
        timerText.text = "Game starting in 1";
    }

    private void CountDown0()
    {
        timerText.text = "Game started!";
        Invoke("StartGame", 1f);
    }

    private void StartGame()
    {
        timerOn = true;
        startedGame = true;
    }
}
