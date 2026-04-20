using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class HubBedInteraction : MonoBehaviour
{
    private bool playerInRange = false;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private GameObject upgradeMenu;
    private InputAction interactAction;
    private TextMeshProUGUI interactText;
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
            if(interactText)interactText.gameObject.SetActive(false);
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
        PauseMenu.canShow = false;
        StartCoroutine(RoomTransitionFade.Instance.FadeOut(RoomTransitionFade.Instance.fadeDuration));
        yield return new WaitForSeconds(1f);
        upgradeMenu.SetActive(true);
        //StartCoroutine(RoomTransitionFade.Instance.FadeIn());

    }

    public void ReturnToGame()
    {
        PlayerMovement.canMove = true;
        player.canRoll = true;
        PauseMenu.canShow = true;
        upgradeMenu.SetActive(false);
        StartCoroutine(RoomTransitionFade.Instance.FadeIn(RoomTransitionFade.Instance.fadeDuration));
    }
    
}
