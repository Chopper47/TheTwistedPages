using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactable : MonoBehaviour
{
    [SerializeField] private InputAction interactAction;
    [SerializeField] private float range;
    [SerializeField] private TextMeshProUGUI promptUI;
    private PlayerMovement player;

    private void Start()
    {
        if(!promptUI) promptUI = ReferenceManager.Instance.interactText;
        player = FindFirstObjectByType<PlayerMovement>();
        interactAction = player.inputActions.FindActionMap("Player").FindAction("Interact");
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            string binding = interactAction.GetBindingDisplayString(1);
            promptUI.text = "Press " + binding + " to Interact";
            promptUI.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            promptUI.gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (promptUI != null)
        {
            promptUI.gameObject.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
