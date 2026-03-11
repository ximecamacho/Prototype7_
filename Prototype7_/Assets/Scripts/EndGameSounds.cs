using UnityEngine;

/// <summary>
/// Listens for win/lose events from PaintCell and plays the appropriate clip.
/// Runs on a single dedicated AudioSource so there is no competition across
/// the 9 PaintCell instances. ignoreListenerPause is set in Awake so the clip
/// always plays even after Time.timeScale is set to 0.
/// </summary>
public class EndGameSounds : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip winClip;
    public AudioClip loseClip;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource                    = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake        = false;
        audioSource.loop               = false;
        audioSource.ignoreListenerPause = true;
    }

    void OnEnable()
    {
        PaintCell.OnWin  += HandleWin;
        PaintCell.OnLose += HandleLose;
    }

    void OnDisable()
    {
        PaintCell.OnWin  -= HandleWin;
        PaintCell.OnLose -= HandleLose;
    }

    // ── handlers ──────────────────────────────────────────────────────────────

    void HandleWin()
    {
        Play(winClip);
    }

    void HandleLose()
    {
        Play(loseClip);
    }

    void Play(AudioClip clip)
    {
        if (clip == null) return;
        audioSource.PlayOneShot(clip);
    }
}
