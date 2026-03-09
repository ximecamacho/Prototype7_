using UnityEngine;

/// <summary>
/// Assigned to a spawned paintball. Picks a random colour at birth and applies it
/// to its own SpriteRenderer. Only collides with the Player — when it does, it finds
/// the PaintCell the player is currently standing in and paints it that colour.
/// Destroys itself on hit or after a fallback lifetime.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class PaintBall : MonoBehaviour
{
    private static readonly Color[] PaintColors = new Color[]
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow,
        new Color(0.5f, 0f, 1f),    // purple
        Color.cyan,
        Color.magenta,
        new Color(1f, 0.5f, 0f)     // orange
    };

    private const float LifetimeSeconds = 8f;
    private const float CellSearchRadius = 1f;

    private Color assignedColor;

    void Awake()
    {
        assignedColor = PaintColors[Random.Range(0, PaintColors.Length)];
        GetComponent<SpriteRenderer>().color = assignedColor;

        Destroy(gameObject, LifetimeSeconds);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Find the PaintCell the player is currently occupying and paint it.
        Collider2D[] hits = Physics2D.OverlapCircleAll(other.transform.position, CellSearchRadius);
        foreach (Collider2D hit in hits)
        {
            PaintCell cell = hit.GetComponent<PaintCell>();
            if (cell != null)
            {
                cell.Paint(assignedColor);
                break;
            }
        }

        Destroy(gameObject);
    }
}
