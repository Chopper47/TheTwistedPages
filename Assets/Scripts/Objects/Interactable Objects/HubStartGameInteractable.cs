using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class HubStartGameInteractable : MonoBehaviour
{
    private bool playerInRange = false;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private GameObject interactText;
    private InputAction interactAction;

    private void Start()
    {
        player = FindFirstObjectByType<PlayerMovement>();

        interactAction = player.inputActions.FindActionMap("Player").FindAction("Interact");
    }

    private void Update()
    {
        if (playerInRange && interactAction.triggered)
        {
            interactText.SetActive(false);
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
        StartCoroutine(RoomTransitionFade.Instance.FadeOut(RoomTransitionFade.Instance.fadeDuration));
        yield return new WaitForSeconds(0.5f);
        StartGame();
    }

    private void StartGame()
    {
        if(SaveManager.instance != null)
        {
            if(SaveManager.instance.player.tutorialCompleted)
            {
                SceneManager.LoadScene("MainGame");
            }
            else SceneManager.LoadScene("Tutorial");
        }
        else SceneManager.LoadScene("Tutorial");

        
    }

    
}
