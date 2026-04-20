using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactable : MonoBehaviour
{
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private float range;
    [SerializeField] private TextMeshProUGUI promptUI;

    private void Start()
    {
        if(!promptUI) promptUI = ReferenceManager.Instance.interactText;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            string binding = interactAction.action.GetBindingDisplayString(1);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
