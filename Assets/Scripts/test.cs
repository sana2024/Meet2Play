using UnityEngine;
using UnityEngine.UI;
using System;

public class test : MonoBehaviour
{
    public float totalTime = 60f; // Total time for the countdown in seconds
    private float remainingTime;
    public Text timerText; // Reference to the UI Text component

    private bool isRunning = false;

    void Start()
    {
        remainingTime = totalTime;
        UpdateTimerText();

        // Check if a previous session exists
        if (PlayerPrefs.HasKey("RemainingTime"))
        {
            remainingTime = PlayerPrefs.GetFloat("RemainingTime");
            long lastTimeTicks = long.Parse(PlayerPrefs.GetString("LastTime"));
            DateTime lastTime = new DateTime(lastTimeTicks);
            DateTime currentTime = DateTime.Now;

            double elapsedSeconds = (currentTime - lastTime).TotalSeconds;
            remainingTime -= (float)elapsedSeconds;

            // If the remaining time is less than zero, set it to zero
            if (remainingTime < 0)
            {
                remainingTime = 0;
            }

            UpdateTimerText();
        }

        isRunning = true;
    }

    void Update()
    {
        if (isRunning && remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;
            UpdateTimerText();

            if (remainingTime <= 0)
            {
                remainingTime = 0;
                OnTimerEnd();
                isRunning = false;
            }
        }
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60F);
        int seconds = Mathf.FloorToInt(remainingTime % 60F);
        int milliseconds = Mathf.FloorToInt((remainingTime * 1000F) % 1000F);

        timerText.text = string.Format("{0:0}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

    void OnTimerEnd()
    {
        // Code to execute when the timer ends
        Debug.Log("Timer has ended!");
        PlayerPrefs.DeleteKey("RemainingTime");
        PlayerPrefs.DeleteKey("LastTime");
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // App is going to background, save the remaining time and current time
            PlayerPrefs.SetFloat("RemainingTime", remainingTime);
            PlayerPrefs.SetString("LastTime", DateTime.Now.Ticks.ToString());
            PlayerPrefs.Save();
        }
    }
}
