using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class HubBedInteraction : MonoBehaviour
{
    private bool playerInRange = false;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private GameObject upgradeMenu;
    [SerializeField] private PauseMenu pauseMenu;
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
        PlayerMovement.canMove = false;
        player.canRoll = false;
        pauseMenu.SetCanShowFalse();
        StartCoroutine(RoomTransitionFade.Instance.FadeOut());
        yield return new WaitForSeconds(1f);
        upgradeMenu.SetActive(true);
        //StartCoroutine(RoomTransitionFade.Instance.FadeIn());

    }

    public void ReturnToGame()
    {
        PlayerMovement.canMove = true;
        player.canRoll = true;
        pauseMenu.SetCanShowTrue();
        upgradeMenu.SetActive(false);
        StartCoroutine(RoomTransitionFade.Instance.FadeIn());
    }
    
}
