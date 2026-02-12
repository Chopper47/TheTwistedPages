using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HubBedInteraction : MonoBehaviour
{
    private bool playerInRange = false;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private GameObject UpgradeMenu;
    private InputAction interactAction;
    private bool inMenu;

    private void Start()
    {
        player = FindFirstObjectByType<PlayerMovement>();

        interactAction = player.inputActions.FindActionMap("Player").FindAction("Interact");
    }

    private void Update()
    {
        if (playerInRange && interactAction.triggered && !inMenu)
        {
            Debug.Log("Interaction");
            inMenu = true;
            StartCoroutine(Fade());
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

    IEnumerator Fade()
    {
        StartCoroutine(RoomTransitionFade.Instance.FadeOut());
        yield return new WaitForSeconds(1f);
        UpgradeMenu.SetActive(true);
        //StartCoroutine(RoomTransitionFade.Instance.FadeIn());

    }

    public void ReturnToGame()
    {
        
        UpgradeMenu.SetActive(false);
        StartCoroutine(RoomTransitionFade.Instance.FadeIn());
    }
    
}
