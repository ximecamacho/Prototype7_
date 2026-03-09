using UnityEngine;
using TMPro; // Required for TextMeshPro UI elements
using UnityEngine.UI; // Required for legacy UI text (if used instead of TMPro)

public class CountDown : MonoBehaviour
{
    public float timeRemaining = 30f; // The starting time
    public bool timerIsRunning = false;
    public TMP_Text timerText; // Reference to your UI TextMeshPro element

    private void Start()
    {
        // Set the timer to start immediately when the scene loads
        timerIsRunning = true;
    }

    void Update()
    {
        if (timerIsRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime; // Decrease time by the time passed since the last frame
                DisplayTime(timeRemaining);
            }
            else
            {
                Debug.Log("Timer has finished!"); // Log a message when the timer ends
                timeRemaining = 0;
                timerIsRunning = false; // Stop the timer
                // Add actions here for when the timer ends (e.g., game over, load scene)
            }
        }
    }

    void DisplayTime(float timeToDisplay)
    {
        // Ensures the displayed time never goes below zero
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }

        // Optional: Format the time to display minutes and seconds (e.g., "01:00")
        //float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        //float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        // Update the UI text
        //timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        // If you only want to display whole seconds (e.g., "60", "59", etc.)
        // timerText.text = Mathf.FloorToInt(timeToDisplay).ToString();
    }
}

