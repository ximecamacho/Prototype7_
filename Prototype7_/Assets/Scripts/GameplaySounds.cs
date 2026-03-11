using System.Collections;
using UnityEngine;

/// <summary>
/// Plays sound effects in response to gameplay events.
/// Attach to any active GameObject in each level scene.
/// Assign clips in the Inspector — slots left empty are silently skipped.
/// </summary>
public class GameplaySounds : MonoBehaviour
{
    [Header("Clips")]
    [Tooltip("Played when a black ball hits the player OR when any tile is erased.")]
    public AudioClip eraseClip;
    [Tooltip("Played when a white paint ball hits the player.")]
    public AudioClip paintClip;

    private AudioSource audioSource;

    // Prevents the erase clip from playing twice in the same frame when a black
    // ball simultaneously triggers OnEraserHit and OnCellErased.
    private bool eraseSoundUsedThisFrame = false;

    void Awake()
    {
        audioSource            = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop        = false;
    }

    void OnEnable()
    {
        PaintBall.OnEraserHit  += PlayEraseSound;
        PaintCell.OnCellErased += PlayEraseSound;
        PaintBall.OnPaintHit   += PlayPaintSound;
    }

    void OnDisable()
    {
        PaintBall.OnEraserHit  -= PlayEraseSound;
        PaintCell.OnCellErased -= PlayEraseSound;
        PaintBall.OnPaintHit   -= PlayPaintSound;
    }

    // ── handlers ──────────────────────────────────────────────────────────────

    private void PlayEraseSound()
    {
        if (eraseSoundUsedThisFrame || eraseClip == null) return;
        eraseSoundUsedThisFrame = true;
        audioSource.PlayOneShot(eraseClip);
        StartCoroutine(ResetEraseFlag());
    }

    private void PlayPaintSound()
    {
        if (paintClip == null) return;
        audioSource.PlayOneShot(paintClip);
    }

    private IEnumerator ResetEraseFlag()
    {
        yield return null;  // wait one frame
        eraseSoundUsedThisFrame = false;
    }
}
