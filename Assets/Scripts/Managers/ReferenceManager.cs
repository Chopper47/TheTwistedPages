using UnityEngine;

public class ReferenceManager : MonoBehaviour
{
    public static ReferenceManager Instance;

    [Header("Player")]
    public Transform player;
    public PlayerMovement playerMovement;
    public PlayerHealth playerHealth;
    public PlayerUpgrades playerUpgrades;

    [Header("Level Generation/Rooms")]
    public LevelGenerator levelGenerator;

    [Header("Minimap")]
    public MinimapManager minimapManager;

    [Header("Music")]
    public MusicManager musicManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
