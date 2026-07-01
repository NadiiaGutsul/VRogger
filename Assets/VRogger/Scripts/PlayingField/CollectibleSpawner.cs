using System.Collections.Generic;
using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    private int maxSpawns = 5;
    private GameObject timerCollectible;
    private List<GameObject> spawnedObjects = new List<GameObject>();
    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip collectSound;

    void Awake()
    {
        timerCollectible = Resources.Load<GameObject>("Prefabs/PowerUps/TimerCollectible");
    }

    public void SpawnCollectibles(List<Transform> snapGrid)
    {
        List<Transform> validSnapGrid = new List<Transform>(snapGrid);

        validSnapGrid.RemoveAll(cube =>
            cube.parent.parent.parent.GetComponentInChildren<GoalController>() != null);
        
        if (validSnapGrid.Count == 0)
        {
            
            return;
        }

        int spawnPointIndex = 0;
        Vector3 lastSpawnPoint = Vector3.negativeInfinity;
        int safetyNet = 0;

        for (int i = 0; i < maxSpawns && safetyNet < 10000; i++)
        {
            spawnPointIndex = Random.Range(0, validSnapGrid.Count);
            Vector3 spawnPoint = validSnapGrid[spawnPointIndex].position;

            if (Vector3.Distance(spawnPoint, lastSpawnPoint) < 0.3f)
            {
                i--;
                safetyNet++;
                continue;
            }

            GameObject currentSpawn = Instantiate(timerCollectible, validSnapGrid[spawnPointIndex]);
            currentSpawn.transform.localPosition = Vector3.zero;
            currentSpawn.transform.localRotation = Quaternion.identity;
            if (audioSource && collectSound)
                audioSource.PlayOneShot(collectSound, 0.5f);

            spawnedObjects.Add(currentSpawn);

            spawnedObjects.Add(currentSpawn);
            lastSpawnPoint = spawnPoint;
        }
    }
    
    public void DestroyCollectibles()
    {
        foreach (GameObject spawnedObject in spawnedObjects)
        {
            Destroy(spawnedObject);
        }
        
        spawnedObjects.Clear();
    }
}
