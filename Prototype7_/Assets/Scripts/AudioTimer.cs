using System;
using UnityEngine;

/// <summary>
/// Ticks a clock sound at a fixed interval for the game's duration.
/// Exposes NormalisedElapsed for other systems (e.g. PlayerShrinker).
/// Does NOT play win or lose sounds — that is the responsibility of PaintCell.
/// Call Stop() from outside to silence the tick at game end.
/// </summary>
public class AudioTimer : MonoBehaviour
{
    [Header("Duration")]
    public float totalTime = 30f;

    [Header("Audio")]
    public AudioClip tickClip;

    /// <summary>Fired when the timer reaches totalTime naturally (loss condition).</summary>
    public event Action OnExpired;

    // ── public read-only state ────────────────────────────────────────────────

    /// <summary>0 (start) → 1 (expired). Read by PlayerShrinker.</summary>
    public float NormalisedElapsed => Mathf.Clamp01(elapsed / totalTime);

    /// <summary>True once the timer has been stopped or has expired.</summary>
    public bool IsFinished => finished;

    // ── private state ─────────────────────────────────────────────────────────
    private AudioSource tickSource;

    private float elapsed  = 0f;
    private bool  finished = false;

    void Awake()
    {
        tickSource             = gameObject.AddComponent<AudioSource>();
        tickSource.playOnAwake = false;
        tickSource.loop        = true;
        tickSource.pitch       = 1f;
        tickSource.clip        = tickClip;
    }

    void Start()
    {
        tickSource.clip = tickClip;
        tickSource.Play();
    }

    void Update()
    {
        if (finished) return;

        elapsed += Time.deltaTime;

        if (elapsed >= totalTime)
        {
            Stop();
            OnExpired?.Invoke();
        }
    }

    /// <summary>Immediately silences the tick. Call this on win or lose.</summary>
    public void Stop()
    {
        finished = true;
        tickSource.Stop();
    }

    /// <summary>Restarts the timer from zero.</summary>
    public void Restart()
    {
        elapsed  = 0f;
        finished = false;
        tickSource.clip = tickClip;
        tickSource.Play();
    }
}
