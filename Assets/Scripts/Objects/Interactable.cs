using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private float range;
    [SerializeField] private GameObject promptUI;

    private void Start()
    {
        promptUI = transform.GetChild(0).gameObject;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            promptUI.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            promptUI.SetActive(false);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
