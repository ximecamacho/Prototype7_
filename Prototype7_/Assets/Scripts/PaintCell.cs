using UnityEngine;

/// <summary>
/// Passive paint target. Cells are only painted when a PaintBall hits the player
/// while the player is standing inside this cell.
/// </summary>
public class PaintCell : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private bool isPainted = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.black;
    }

    /// <summary>Paints the cell with the given colour. Ignored if already painted.</summary>
    public void Paint(Color color)
    {
        if (isPainted) return;

        isPainted = true;
        spriteRenderer.color = color;
    }
}
