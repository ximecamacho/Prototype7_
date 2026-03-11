using UnityEngine;

/// <summary>
/// Erases any white PaintCell the player lands on after each move or dash.
/// Also turns the player sprite black in LateUpdate whenever they are standing
/// on a white cell, overriding the colour set by GridMovement.
/// </summary>
[RequireComponent(typeof(GridMovement))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerEraser : MonoBehaviour
{
    private const float CellSearchRadius = 0.4f;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        GetComponent<GridMovement>().OnMoved += EraseCurrentCell;
    }

    void OnDisable()
    {
        GetComponent<GridMovement>().OnMoved -= EraseCurrentCell;
    }

    /// <summary>Erases the cell the player just landed on.</summary>
    void EraseCurrentCell()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, CellSearchRadius);
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

    /// <summary>
    /// Runs after GridMovement.Update. Sets the player colour to black when
    /// standing on a white cell, otherwise leaves GridMovement's colour in place.
    /// </summary>
    void LateUpdate()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, CellSearchRadius);
        foreach (Collider2D hit in hits)
        {
            PaintCell cell = hit.GetComponent<PaintCell>();
            if (cell != null)
            {
                if (cell.IsPainted)
                    spriteRenderer.color = Color.black;
                return;
            }
        }
    }
}
