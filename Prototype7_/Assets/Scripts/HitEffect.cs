using System.Collections;
using UnityEngine;

/// <summary>
/// Plays a camera shake and red-tint flash whenever a black eraser ball hits the player.
/// Attach this to the Main Camera. Wire <see cref="hitTintRenderer"/> to a full-screen
/// red SpriteRenderer in the scene (sortingOrder above all game objects, alpha starts at 0).
/// </summary>
public class HitEffect : MonoBehaviour
{
    [Header("Camera Shake")]
    public float shakeDuration  = 0.25f;
    public float shakeMagnitude = 0.15f;

    [Header("Red Flash")]
    public SpriteRenderer hitTintRenderer;
    public float flashDuration = 0.35f;
    public float flashMaxAlpha = 0.45f;

    private Vector3    originalCamPos;
    private Coroutine  shakeRoutine;
    private Coroutine  flashRoutine;

    void OnEnable()
    {
        PaintBall.OnEraserHit    += HandleEraserHit;   // black ball hits player
        PaintCell.OnCellErased   += HandleEraserHit;   // player or ball erases a white tile
    }

    void OnDisable()
    {
        PaintBall.OnEraserHit    -= HandleEraserHit;
        PaintCell.OnCellErased   -= HandleEraserHit;
    }

    void Start()
    {
        originalCamPos = transform.localPosition;

        if (hitTintRenderer != null)
            SetAlpha(0f);
    }

    // ── event handler ─────────────────────────────────────────────────────────

    private void HandleEraserHit()
    {
        if (shakeRoutine != null) StopCoroutine(shakeRoutine);
        if (flashRoutine  != null) StopCoroutine(flashRoutine);

        shakeRoutine = StartCoroutine(Shake());
        flashRoutine = StartCoroutine(RedFlash());
    }

    // ── coroutines ────────────────────────────────────────────────────────────

    private IEnumerator Shake()
    {
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            float t = 1f - (elapsed / shakeDuration);   // magnitude fades out
            float x = Random.Range(-1f, 1f) * shakeMagnitude * t;
            float y = Random.Range(-1f, 1f) * shakeMagnitude * t;
            transform.localPosition = new Vector3(originalCamPos.x + x, originalCamPos.y + y, originalCamPos.z);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        transform.localPosition = originalCamPos;
        shakeRoutine = null;
    }

    private IEnumerator RedFlash()
    {
        if (hitTintRenderer == null) yield break;

        float half    = flashDuration * 0.5f;
        float elapsed = 0f;

        while (elapsed < half)
        {
            SetAlpha(Mathf.Lerp(0f, flashMaxAlpha, elapsed / half));
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < half)
        {
            SetAlpha(Mathf.Lerp(flashMaxAlpha, 0f, elapsed / half));
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        SetAlpha(0f);
        flashRoutine = null;
    }

    private void SetAlpha(float alpha)
    {
        Color c = hitTintRenderer.color;
        c.a = alpha;
        hitTintRenderer.color = c;
    }
}
