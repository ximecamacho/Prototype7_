using System;
using UnityEngine;

/// <summary>
/// Passive paint target. Tracks total painted cells and fires static events on win/lose.
/// Win  = all cells painted.
/// Lose = triggered by <see cref="PlayerShrinker.OnPlayerTooSmall"/>.
/// </summary>
public class PaintCell : MonoBehaviour
{
    // Counted dynamically so any grid size wins at the right number.
    private static int  totalCells = 0;
    public  static int  paintedNum;
    private static bool gameEnded  = false;

    /// <summary>Fired once when all cells are painted.</summary>
    public static event Action OnWin;

    /// <summary>Fired once when the player shrinks to the losing size.</summary>
    public static event Action OnLose;

    /// <summary>
    /// Fired when a ball erases a painted cell.
    /// Movement-based erasure (PlayerEraser) does NOT fire this.
    /// </summary>
    public static event Action OnCellErased;

    [Header("Game State Objects")]
    public GameObject winObj;
    public GameObject loseObj;

    private SpriteRenderer spriteRenderer;
    private bool isPainted = false;

    /// <summary>True when this cell has been painted white.</summary>
    public bool IsPainted => isPainted;

    void Awake()
    {
        totalCells++;
        spriteRenderer       = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.black;
        winObj.SetActive(false);
        loseObj.SetActive(false);
        gameEnded  = false;
        paintedNum = 0;
    }

    void OnEnable()  => PlayerShrinker.OnPlayerTooSmall += TriggerLose;
    void OnDisable() => PlayerShrinker.OnPlayerTooSmall -= TriggerLose;

    void OnDestroy() => totalCells--;

    void Update()
    {
        if (gameEnded) return;
        if (paintedNum >= totalCells)
            TriggerWin();
    }

    /// <summary>Paints the cell white. Ignored if already painted.</summary>
    public void Paint()
    {
        if (isPainted) return;
        isPainted            = true;
        paintedNum++;
        spriteRenderer.color = Color.white;
    }

    /// <summary>
    /// Erases the cell back to black. Ignored if not already painted.
    /// Pass <c>fireEvent = true</c> (used by PaintBall) to broadcast
    /// <see cref="OnCellErased"/> for the shake/flash effect.
    /// Movement-based erasure (PlayerEraser) leaves this false.
    /// </summary>
    public void Erase(bool fireEvent = false)
    {
        if (!isPainted) return;
        isPainted            = false;
        paintedNum--;
        spriteRenderer.color = Color.black;
        if (fireEvent) OnCellErased?.Invoke();
    }

    // ── private helpers ───────────────────────────────────────────────────────

    void TriggerWin()
    {
        if (gameEnded) return;
        EndGame();
        winObj.SetActive(true);
        OnWin?.Invoke();
    }

    void TriggerLose()
    {
        if (gameEnded) return;
        EndGame();
        loseObj.SetActive(true);
        OnLose?.Invoke();
    }

    void EndGame()
    {
        gameEnded      = true;
        Time.timeScale = 0f;
    }
}
