using System.Collections;
using UnityEngine;

/// <summary>
/// Plays the countdown clip once at level start, then hands control to the player.
/// Disables <see cref="GridMovement"/> and all <see cref="Spawner"/>s in Awake —
/// before their Start() runs — so nothing is active while the audio plays.
/// Also resets Time.timeScale to 1 in case the previous scene left it at 0.
/// </summary>
public class LevelCountdown : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip countdownClip;

    [Tooltip("Seconds from the start of the clip when 'go' plays. " +
             "Gameplay unlocks at this point. Set to 0 to wait for the full clip.")]
    public float gameStartOffset = 0f;

    private AudioSource audioSource;

    void Awake()
    {
        Time.timeScale = 1f;

        audioSource                     = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake         = false;
        audioSource.loop                = false;
        audioSource.ignoreListenerPause = true;

        SetGameplayEnabled(false);
    }

    void Start()
    {
        StartCoroutine(RunCountdown());
    }

    private IEnumerator RunCountdown()
    {
        if (countdownClip != null)
        {
            audioSource.PlayOneShot(countdownClip);

            float delay = gameStartOffset > 0f
                ? gameStartOffset
                : countdownClip.length;

            yield return new WaitForSeconds(delay);
        }

        SetGameplayEnabled(true);
    }

    /// <summary>Enables or disables GridMovement and every Spawner in the scene.</summary>
    private void SetGameplayEnabled(bool enabled)
    {
        GridMovement movement = FindAnyObjectByType<GridMovement>();
        if (movement != null)
            movement.enabled = enabled;

        foreach (Spawner s in FindObjectsByType<Spawner>(FindObjectsSortMode.None))
            s.enabled = enabled;
    }
}
