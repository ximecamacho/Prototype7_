using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform[] spawnPoints;
    public GameObject [] projectilePrefabs;
    private float spawnInterval = 1.0f;
    void Start()
    {
        StartCoroutine(spawnProj(spawnInterval));
    }

    private IEnumerator spawnProj(float interval)
    {
        yield return new WaitForSeconds(interval);
        int randProj = Random.Range(0, projectilePrefabs.Length);
        int randSpawPoint = Random.Range(0, spawnPoints.Length);
        Instantiate(projectilePrefabs[randProj], spawnPoints[randSpawPoint].position, transform.rotation);
        StartCoroutine(spawnProj(spawnInterval));
        
    }


    // Update is called once per frame
    void Update()
    {

        
            
        
    }
}
