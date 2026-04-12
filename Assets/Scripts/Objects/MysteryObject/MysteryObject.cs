using UnityEngine;
using UnityEngine.InputSystem;

public class MysteryObject : MonoBehaviour
{
    public MysteryEffect[] effects;

    private bool playerInRange = false;
    private bool used = false;

    private PlayerMovement player;
    private InputAction interactAction;

    private void Start()
    {
        player = FindFirstObjectByType<PlayerMovement>();

        interactAction = player.inputActions.FindActionMap("Player").FindAction("Interact");
    }

    private void Update()
    {
        if (!playerInRange || used)
        {
            return;
        }

        if (interactAction.triggered)
        {
            Activate();
        }
    }

    private void Activate()
    {
        used = true;

        if (effects.Length == 0)
        {
            return;
        }

        MysteryEffect effect = effects[Random.Range(0, effects.Length)];

        effect.Apply(player.gameObject);

        UIManager.Instance.StartCoroutine(UIManager.Instance.ShowEffectMessage(effect.effectMessage, 2.0f));

        Debug.Log("Effect applied: " + effect.name);

        Destroy(gameObject);
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
