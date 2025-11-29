using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [Header("Room components")]
    public RoomPreset roomPreset;
    public RoomInstance roomInstance;
    public List<DoorController> doors = new List<DoorController>();
    public Transform[] enemySpawnPoints; 
    public Transform respawnPoint;

    [Header("Flags")]
    private int enemiesRemaining = 0;
    private bool isCleared = false;
    private bool isActive = false;
    private MusicManager musicManager;
    private int musicIndex=1;
    public GameObject gameWonMenu;

    [Header("Waves")]
    private int totalWaves;
    private int currentWave;
    private bool wavesStarted = false;

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

        gameWonMenu = GameObject.FindGameObjectWithTag("Game Won Menu");
        musicManager = GameObject.FindFirstObjectByType<MusicManager>();
    }

    public void ActivateRoom()
    {
        PlayerFall player = FindFirstObjectByType<PlayerFall>();
        player.SetRoomRespawn(respawnPoint);
        
        if (isActive || isCleared)
        {
            return;
        }

        isActive = true;

        totalWaves = roomPreset.difficultyRating;
        currentWave = 0;
        wavesStarted = true;

        if (roomInstance.roomType != RoomType.Start && roomPreset.enemyPrefabs != null && roomPreset.enemyPrefabs.Length > 0)
        {
            StartNextWave();
        }

        if (enemiesRemaining > 0)
        {
            foreach (DoorController door in doors)
            {
                door.CloseDoor();
                //if (musicIndex==1)
                //{
                //    musicManager.SwitchStageSynced(musicIndex);
                //    musicIndex = 0;
                //}
            }
        }
        else
        {
            OpenDoors();
            isCleared = true;
        }
    }

    private void StartNextWave()
    {
        currentWave++;

        SpawnEnemies();

        if (enemiesRemaining == 0)
        {
            CompleteWave();
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

            EnemyHealth enemyHealth = enemyInstance.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.roomController = this;
            }

            enemiesRemaining++;
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
            CompleteWave();

            if (roomInstance != null && roomInstance.roomType == RoomType.Boss)
            {
                OnBossRoomCleared();
            }
        }
    }

    private void CompleteWave()
    {
        if (currentWave < totalWaves)
        {
            StartNextWave();
            return;
        }

        isCleared = true;
        OpenDoors();

        if (CurrencyManager.instance)
        {
            CurrencyManager.instance.AddCurrency(roomPreset.difficultyRating);
        }

        if (roomInstance.roomType == RoomType.Boss)
        {
            OnBossRoomCleared();
        }
    }

    private void OpenDoors()
    {
        foreach (DoorController door in doors)
        {
            door.OpenDoor();
        }
    }

    private void OnBossRoomCleared()
    {
        Debug.Log("Congratulations!");
        gameWonMenu.SetActive(true);
    }
}
