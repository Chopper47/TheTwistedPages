using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Input System")]
    public InputActionAsset inputActions;
    private InputAction pauseAction;

    [Header("Pause Menu")]
    public bool isPaused = false;
    public GameObject pauseMenu;

    [Header("Options Menu")]
    public GameObject optionsMenu;

    private void Awake()
    {
        pauseAction = inputActions.FindActionMap("Player").FindAction("Pause");

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
    }

    private void Update()
    {
        if (pauseAction.triggered)
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);

        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            PlayerMovement.canMove = true;
        }
        else
        {
            Time.timeScale = 0;
            PlayerMovement.canMove = false;
        }
    }

    public void OptionsMenu()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("main_menu");
    }
}
