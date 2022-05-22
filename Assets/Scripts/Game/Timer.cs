using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public float timeLeft;
    [HideInInspector] public bool timerOn = true;

    public TMP_Text timerText;
   
    void Start()
    {
        timerOn = true;
    }

    void Update()
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

    void UpdateTimer(float currentTime)
    {
        currentTime += 1;

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        timerText.text = $"{minutes:00}:{seconds:00}";
    }
}
