using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[]  spawnPoints;
    public GameObject[] projectilePrefabs;
    private float spawnInterval = 1.0f;

    void OnEnable()
    {
        PaintCell.OnWin  += StopSpawning;
        PaintCell.OnLose += StopSpawning;
    }

    void OnDisable()
    {
        PaintCell.OnWin  -= StopSpawning;
        PaintCell.OnLose -= StopSpawning;
    }

    void Start()
    {
        StartCoroutine(spawnProj(spawnInterval));
    }

    private IEnumerator spawnProj(float interval)
    {
        yield return new WaitForSeconds(interval);
        int randProj      = Random.Range(0, projectilePrefabs.Length);
        int randSpawnPoint = Random.Range(0, spawnPoints.Length);
        Instantiate(projectilePrefabs[randProj], spawnPoints[randSpawnPoint].position, transform.rotation);
        StartCoroutine(spawnProj(spawnInterval));
    }

    /// <summary>Stops the spawn coroutine immediately when the game ends.</summary>
    private void StopSpawning()
    {
        StopAllCoroutines();
        enabled = false;
    }
}
