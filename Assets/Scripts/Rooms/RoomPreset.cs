using UnityEngine;

/// <summary>
/// Scriptable object that stores all relevant details of a room.
/// </summary>
[CreateAssetMenu(menuName = "ScriptableObjects/Room")]
public class RoomPreset : ScriptableObject
{
    [Header("Room Settings")]
    public GameObject roomPrefab;
    public bool canBeStart;
    public bool canBeShop;
    public bool canBeHealth;
    public bool canBeFloorChange;
    public bool canBeBoss;
    public int difficultyRating;

    [Header("Enemy Waves")]
    public GameObject[] enemyPrefabs;
    public int enemyCount = 3;
}
