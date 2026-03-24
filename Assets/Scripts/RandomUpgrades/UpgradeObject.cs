using UnityEngine;
using UnityEngine.InputSystem;

public class UpgradeObject : MonoBehaviour
{
    [SerializeField] private RandomUpgrade upgrade;

    private bool playerInRange = false;
    private PlayerUpgrades playerUpgrades;
    private PlayerMovement player;

    private InputAction interactAction;

    private void Start()
    {
        player = FindFirstObjectByType<PlayerMovement>();
        playerUpgrades = FindFirstObjectByType<PlayerUpgrades>();

        interactAction = player.inputActions.FindActionMap("Player").FindAction("Interact");
    }

    private void Update()
    {
        if (playerInRange && interactAction.triggered)
        {
            playerUpgrades.AddUpgrade(upgrade);
            Destroy(gameObject);
        }
    } 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void Initialize(RandomUpgrade randomUpgrade)
    {
        upgrade = randomUpgrade;
    }
}