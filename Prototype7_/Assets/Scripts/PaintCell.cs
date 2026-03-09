using System.Threading;
using UnityEngine;

/// <summary>
/// Passive paint target. Cells are only painted when a PaintBall hits the player
/// while the player is standing inside this cell.
/// </summary>
public class PaintCell : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private bool isPainted = false;

    public static int paintedNum;
    public GameObject winObj;
    public GameObject loseObj;
    public CountDown timer;
    void Awake()
    {
        timer = FindAnyObjectByType<CountDown>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.black;
        winObj.SetActive(false);
        loseObj.SetActive(false);
    }
    void Update()
    {

        if (paintedNum == 9)
        {
            Time.timeScale = 0;
            winObj.SetActive(true);
        }
        else if (paintedNum < 9 && (timer.timeRemaining == 0))
        {
            Time.timeScale = 0;
            loseObj.SetActive(true);
        }
    }

    /// <summary>Paints the cell with the given colour. Ignored if already painted.</summary>
    public void Paint(Color color)
    {
        if ((color == Color.black) && isPainted)
        {
            isPainted = false;
            paintedNum--;
            spriteRenderer.color = Color.black;
            return;
        }
        if (isPainted) return;

        if (color != Color.black)
        {
            isPainted = true;
            paintedNum++;
            spriteRenderer.color = color;
        }
        
    }
}
