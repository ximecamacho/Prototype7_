using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages level flow: transitions on win, restart on R, quit on ESC.
/// Place one instance in every level scene.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Level Order")]
    [Tooltip("Scene names to load in sequence. Wraps back to the first after the last.")]
    public string[] levelOrder = { "Level1", "Level2", "Level3" };

    [Tooltip("Seconds to wait after winning before the next level loads (lets the win sound finish).")]
    public float winDelay = 2f;

    void OnEnable()  => PaintCell.OnWin += HandleWin;
    void OnDisable() => PaintCell.OnWin -= HandleWin;

    void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
            RestartLevel();
        else if (Keyboard.current.escapeKey.wasPressedThisFrame)
            Application.Quit();
    }

    // ── event handlers ────────────────────────────────────────────────────────

    private void HandleWin()
    {
        StartCoroutine(LoadNextLevel());
    }

    // ── coroutines ────────────────────────────────────────────────────────────

    private IEnumerator LoadNextLevel()
    {
        // Wait for the full length of the win clip so it isn't cut off.
        // Falls back to winDelay if EndGameSounds or its clip can't be found.
        float delay = winDelay;
        EndGameSounds endGameSounds = FindAnyObjectByType<EndGameSounds>();
        if (endGameSounds != null && endGameSounds.winClip != null)
            delay = Mathf.Max(delay, endGameSounds.winClip.length);

        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 1f;
        SceneManager.LoadScene(GetNextSceneName());
    }

    // ── helpers ───────────────────────────────────────────────────────────────

    /// <summary>Reloads the active scene, resetting timeScale first.</summary>
    private void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Returns the scene name that follows the active scene in <see cref="levelOrder"/>.
    /// Wraps to index 0 after the last entry.
    /// </summary>
    private string GetNextSceneName()
    {
        string current = SceneManager.GetActiveScene().name;
        for (int i = 0; i < levelOrder.Length; i++)
        {
            if (levelOrder[i] == current)
                return levelOrder[(i + 1) % levelOrder.Length];
        }
        // Fallback: current scene not found in the array — restart it.
        return current;
    }
}
