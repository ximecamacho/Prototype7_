using System;
using UnityEngine;

/// <summary>
/// Shrinks the player by a fixed amount each time a black eraser ball hits them.
/// Fires <see cref="OnPlayerTooSmall"/> when the player reaches the minimum scale,
/// which acts as the lose condition for the game.
/// </summary>
public class PlayerShrinker : MonoBehaviour
{
    [Header("Shrink Settings")]
    [Tooltip("How much the uniform scale decreases per eraser hit (0–1).")]
    public float shrinkPerHit = 0.2f;
    [Tooltip("Scale at or below which the lose condition triggers.")]
    public float minScale     = 0.15f;

    /// <summary>Fired once when the player's scale drops to or below minScale.</summary>
    public static event Action OnPlayerTooSmall;

    private float currentScale;
    private bool  lostFired = false;

    void OnEnable()  => PaintBall.OnEraserHit += HandleHit;
    void OnDisable() => PaintBall.OnEraserHit -= HandleHit;

    void Start()
    {
        currentScale = 1f;
        ApplyScale();
    }

    private void HandleHit()
    {
        if (lostFired) return;

        currentScale = Mathf.Max(0f, currentScale - shrinkPerHit);
        ApplyScale();

        if (currentScale <= minScale)
        {
            lostFired = true;
            OnPlayerTooSmall?.Invoke();
        }
    }

    /// <summary>Preserves the authored z-scale while setting uniform x/y.</summary>
    private void ApplyScale()
    {
        transform.localScale = new Vector3(currentScale, currentScale, transform.localScale.z);
    }
}
