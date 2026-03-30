using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    private Gnome gnome;
   
    private List<VineTrap> vines=new List<VineTrap>();
    [Header("Room components")]
    public RoomPreset roomPreset;
    public RoomInstance roomInstance;
    public List<DoorController> doors = new List<DoorController>();
    public Transform[] enemySpawnPoints; 
    public Transform respawnPoint;

    [Header("Flags")]
    private int enemiesRemaining = 0;
    public bool isCleared = false;
    private bool isActive = false;
    private bool treeSpawnedThisWave = false;
    private int treeIndex = 1;

    [Header("References")]
    private GameObject gameWonMenu;
    private MinimapManager minimapManager;

    [Header("Waves")]
    private int totalWaves;
    private int currentWave;
    private TextMeshProUGUI nextWaveText;

    [Header("Music")]
    private MusicManager musicManager;
    private int musicIndex = 1;

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

        if (ReferenceManager.Instance)
        {
            minimapManager = ReferenceManager.Instance.minimapManager;
            musicManager = ReferenceManager.Instance.musicManager;
        }

        if (UIManager.Instance)
        {
            gameWonMenu = UIManager.Instance.gameWonMenu;
            nextWaveText = UIManager.Instance.nextWaveText;

        }

        if (gameWonMenu != null)
        {
            gameWonMenu.SetActive(false);
        }

        if (nextWaveText != null)
        {
            nextWaveText.gameObject.SetActive(false);
        }
    }

    public void ActivateRoom()
    {
        //PlayerFall player = FindFirstObjectByType<PlayerFall>();
        //player.SetRoomRespawn(respawnPoint);

        if (isActive || isCleared)
        {
            return;
        }

        isActive = true;
        enemiesRemaining = 0;

        gnome = GetComponentInChildren<Gnome>();
        if (gnome != null)
        {
            gnome.Laugh();
        }

        vines.AddRange(GetComponentsInChildren<VineTrap>()); 
        if (vines.Count > 0)
        {
            for (int i = 0; i < vines.Count; i++)
            {
                vines[i].Main.volume = 1.0f;
            }
        }

        if (minimapManager != null)
        {
            minimapManager.SetCurrentRoom(roomInstance.gridPosition);
        }

        totalWaves = roomPreset.difficultyRating;
        currentWave = 0;

        if (roomInstance.roomType != RoomType.Start && roomPreset.enemyPrefabs != null && roomPreset.enemyPrefabs.Length > 0)
        {
            StartCoroutine(StartNextWave());
        }

        if (enemiesRemaining > 0)
        {
            foreach (DoorController door in doors)
            {
                door.CloseDoor();
            }

            if (musicIndex == 1)
            {
                musicManager.SwitchStageSynced(musicIndex);
                musicIndex = 0;
            }
        }
        else
        {
            OpenDoors();
            isCleared = true;
            musicManager.SwitchStageSynced(0);
        }
    }

    private IEnumerator StartNextWave()
    {
        currentWave++;

        if (currentWave > 1)
        {
            if (nextWaveText != null)
            {
                nextWaveText.gameObject.SetActive(true);

                yield return new WaitForSeconds(2.0f);

                nextWaveText.gameObject.SetActive(false);
            }
        }

        treeSpawnedThisWave = false;

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

            List<GameObject> enemyCandidates = new List<GameObject>(roomPreset.enemyPrefabs);

            // If the tree already spawned this wave, it is removed from the list.
            if (treeSpawnedThisWave && treeIndex >= 0 && treeIndex < enemyCandidates.Count)
            {
                enemyCandidates.RemoveAt(treeIndex);
            }

            if (enemyCandidates.Count == 0)
            {
                enemyCandidates.Add(roomPreset.enemyPrefabs[0]);
            }

            GameObject enemyPrefab = enemyCandidates[Random.Range(0, enemyCandidates.Count)];

            if (treeIndex >= 0 && treeIndex < roomPreset.enemyPrefabs.Length && enemyPrefab == roomPreset.enemyPrefabs[treeIndex])
            {
                treeSpawnedThisWave = true;
            }

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
            StartCoroutine(StartNextWave());
            return;
        }

        isCleared = true;
        OpenDoors();

        if (CurrencyManager.instance)
        {
            CurrencyManager.instance.AddCurrency(roomPreset.difficultyRating);
        }

        PlayerUpgrades player = FindFirstObjectByType<PlayerUpgrades>();
        if (player != null && player.passiveUpgrade != null)
        {
            if (player.passiveUpgrade.behaviour is MoneyBoostUpgrade moneyBoost)
            {
                moneyBoost.OnRoomCleared(player);
            }
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
            door.GetComponentInChildren<Animator>().SetTrigger("OpenDoor");
            door.GetComponentInChildren<Light>().enabled = true;

        }
        if (vines.Count > 0)
        {
            for (int i = 0; i < vines.Count; i++)
            {
                vines[i].Main.volume = 0f;
            }
            vines.Clear();
        }
    }

    private void OnBossRoomCleared()
    {
        Debug.Log("Congratulations!");
        gameWonMenu.SetActive(true);
    }
}
