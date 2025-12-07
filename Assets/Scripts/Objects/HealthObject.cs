using UnityEngine;
using UnityEngine.InputSystem;

public class HealthObject : MonoBehaviour
{
    [SerializeField] private int healAmount = 2;

    private bool playerInRange = false;
    private PlayerHealth playerHealth;
    private PlayerMovement player;

    private InputAction interactAction;

    private void Start()
    {
        player = FindFirstObjectByType<PlayerMovement>();
        playerHealth = FindFirstObjectByType<PlayerHealth>();

        interactAction = player.inputActions.FindActionMap("Player").FindAction("Interact");
    }

    private void Update()
    {
        if (playerInRange && interactAction.triggered && playerHealth.currentHealth != playerHealth.maxHealth)
        {
            playerHealth.Heal(healAmount);
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
}
