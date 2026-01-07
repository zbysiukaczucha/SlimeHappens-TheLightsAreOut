using System.Collections.Generic;
using UnityEngine;

public class LampSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject lampPrefab;
    private int numberOfLamps = 2;

    [Header("Position Settings")]
    private float minX = -25f;
    private float maxX = 25f;
    private float spawnHeight = 1.1f;
    private float spawnZ = 0f;
    private float heightVariance = 0.5f;
    
    private float minDistance = 5f;
    private List<float> _spawnedXPositions = new List<float>();

    [Header("Flip Settings")]
    [Tooltip("If true, lamps will randomly flip horizontally.")]
    private bool randomFlipX = true;
    

    void Start()
    {
        for (int i = 0; i < numberOfLamps; i++)
        {
            TrySpawnLamp();
        }
    }

    void TrySpawnLamp()
    {
        float candidateX = 0f;
        bool validPositionFound = false;
        int attempts = 0;

        // Try up to 20 times to find a valid spot
        // (Prevents infinite loop if your map is too small for the lamps)
        while (!validPositionFound && attempts < 20)
        {
            attempts++;
            candidateX = Random.Range(minX, maxX);

            // Check if this X is valid
            if (IsPositionValid(candidateX))
            {
                validPositionFound = true;
            }
        }

        if (validPositionFound)
        {
            SpawnLampAt(candidateX);
        }
        else
        {
            Debug.LogWarning("Could not find a valid spot for a lamp. Map might be too small or minDistance too high.");
        }
    }

    bool IsPositionValid(float newX)
    {
        // Loop through all existing lamp positions
        foreach (float existingX in _spawnedXPositions)
        {
            // If the distance is less than our minimum, it's invalid
            if (Mathf.Abs(newX - existingX) < minDistance)
            {
                return false;
            }
        }
        return true;
    }

    void SpawnLampAt(float xPos)
    {
        // Add to list so future lamps can check against this one
        _spawnedXPositions.Add(xPos);

        // Height Calculation
        float randomY = spawnHeight + Random.Range(-heightVariance, heightVariance);
        Vector3 spawnPos = new Vector3(xPos, randomY, spawnZ);

        // Instantiate
        GameObject newLamp = Instantiate(lampPrefab, spawnPos, Quaternion.identity);

        // Flip Logic
        if (randomFlipX && Random.value > 0.5f)
        {
            Vector3 currentScale = newLamp.transform.localScale;
            currentScale.x *= -1;
            newLamp.transform.localScale = currentScale;
        }
    }
}