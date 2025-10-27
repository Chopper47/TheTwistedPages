using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [Header("Room components")]
    public RoomPreset roomPreset;
    public RoomInstance roomInstance;
    public List<DoorController> doors = new List<DoorController>();
    public Transform[] enemySpawnPoints;

    [Header("Flags")]
    private int enemiesRemaining = 0;
    private bool isCleared = false;
    private bool isActive = false;

    private void Awake()
    {
        foreach (DoorController door in GetComponentsInChildren<DoorController>())
        {
            door.parentRoom = this;
            if (!doors.Contains(door))
            {
                doors.Add(door);
            }
        }
    }

    public void ActivateRoom()
    {
        if (isActive || isCleared)
        {
            return;
        }

        isActive = true;

        if (roomPreset.enemyPrefabs != null && roomPreset.enemyPrefabs.Length > 0)
        {
            SpawnEnemies();
        }

        enemiesRemaining = GetComponentsInChildren<EnemyHealth>().Length;

        if (enemiesRemaining > 0)
        {
            foreach (DoorController door in doors)
            {
                door.CloseDoor();
            }
        }
        else
        {
            OpenDoors();
            isCleared = true;
        }
    }

    private void SpawnEnemies()
    {
        int enemiesToSpawn = Mathf.Min(roomPreset.enemyCount, enemySpawnPoints.Length);

        for (int i = 0; i < enemiesToSpawn; i++)
        {
            Transform spawnPoint = enemySpawnPoints[i];
            GameObject enemyPrefab = roomPreset.enemyPrefabs[Random.Range(0, roomPreset.enemyPrefabs.Length)];

            GameObject enemyInstance = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity, transform);

            // Optionally set up callbacks
            EnemyHealth enemyHealth = enemyInstance.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.roomController = this;
            }
        }
    }

    public void OnEnemyDied()
    {
        if (!isActive || isCleared)
        {
            return;
        }

        enemiesRemaining--;

        if (enemiesRemaining <= 0)
        {
            isCleared = true;
            OpenDoors();
        }
    }

    private void OpenDoors()
    {
        foreach (DoorController door in doors)
        {
            door.OpenDoor();
        }
    }
}
