using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("InputSystem")]
    public InputActionAsset inputActions;
    private InputAction pauseAction;

    [Header("PauseMenu")]
    public GameObject pauseMenu;

    private PlayerMovement playerMovement;

    private void Awake()
    {
        pauseAction = inputActions.FindActionMap("Player").FindAction("Pause");

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (pauseAction.triggered)
        {
            Time.timeScale = 0f;
            if (pauseMenu != null)
            {
                pauseMenu.SetActive(true);
            }

            playerMovement.enabled = false;
        }
    }

    public void ReturnButton()
    {
        Time.timeScale = 1.0f;
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }

        playerMovement.enabled = true;
    }
}
