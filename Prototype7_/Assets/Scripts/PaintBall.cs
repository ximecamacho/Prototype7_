using System;
using UnityEngine;

/// <summary>
/// Assigned to a spawned paintball. Randomly spawns as either:
///   - White painter  (60%): white fill, black outline — paints cells white on hit.
///   - Black eraser   (40%): black fill, white outline — erases cells back to black on hit.
/// Only collides with the Player.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class PaintBall : MonoBehaviour
{
    /// <summary>Fired when a black eraser ball hits the player.</summary>
    public static event Action OnEraserHit;
    /// <summary>Fired when a white paint ball hits the player.</summary>
    public static event Action OnPaintHit;
    private const float LifetimeSeconds  = 8f;
    private const float CellSearchRadius = 1f;
    private const float OutlineScale     = 1.25f;
    private const float EraserChance     = 0.4f;

    private bool isEraser;

    void Awake()
    {
        isEraser = UnityEngine.Random.value < EraserChance;

        Color fillColor    = isEraser ? Color.black : Color.white;
        Color outlineColor = isEraser ? Color.white : Color.black;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color          = fillColor;
        sr.sortingOrder   = 2;   // ball renders above outline (1) and grid cells (0)

        // Outline — slightly larger copy of the same sprite rendered behind
        GameObject outline              = new GameObject("Outline");
        outline.transform.SetParent(transform, false);
        outline.transform.localScale    = Vector3.one * OutlineScale;
        outline.transform.localPosition = Vector3.zero;

        SpriteRenderer outlineSr = outline.AddComponent<SpriteRenderer>();
        outlineSr.sprite         = sr.sprite;
        outlineSr.color          = outlineColor;
        outlineSr.sortingLayerID = sr.sortingLayerID;
        outlineSr.sortingOrder   = 1;   // above grid cells (0), below ball fill (2)

        Destroy(gameObject, LifetimeSeconds);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (isEraser)
        {
            // Always shrink the player on eraser contact, regardless of cell state.
            OnEraserHit?.Invoke();

            // Erase the underlying cell and fire the shake/flash event only when a
            // painted tile is actually removed (fireEvent = true).
            Collider2D[] hits = Physics2D.OverlapCircleAll(other.transform.position, CellSearchRadius);
            foreach (Collider2D hit in hits)
            {
                PaintCell cell = hit.GetComponent<PaintCell>();
                if (cell != null)
                {
                    cell.Erase(fireEvent: true);
                    break;
                }
            }
        }
        else
        {
            OnPaintHit?.Invoke();

            Collider2D[] hits = Physics2D.OverlapCircleAll(other.transform.position, CellSearchRadius);
            foreach (Collider2D hit in hits)
            {
                PaintCell cell = hit.GetComponent<PaintCell>();
                if (cell != null)
                {
                    cell.Paint();
                    break;
                }
            }
        }

        Destroy(gameObject);
    }
}
