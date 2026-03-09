using UnityEngine;
using UnityEngine.EventSystems;
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
    public GameObject currencyUI;
    private bool canShow;

    [Header("Options Menu")]
    public GameObject optionsMenu;

    [Header("SFX")]
    public AudioManager AM;
    public AudioSource Main;

    private void Awake()
    {
        pauseAction = inputActions.FindActionMap("Player").FindAction("Pause");
        canShow = true;

        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
        AM = FindFirstObjectByType<AudioManager>();
        Main = GetComponentInChildren<AudioSource>();
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
        if (canShow)
        {

            isPaused = !isPaused;
            pauseMenu.SetActive(isPaused);
            currencyUI.SetActive(isPaused);

            if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
                //PlayerMovement.canMove = true;
                optionsMenu.SetActive(false);
                AM.Play("Unpause", Main);

                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(pauseMenu.GetComponent<MenuNavigator>().firstSelected);
            }
            else
            {
                Time.timeScale = 0;
                //PlayerMovement.canMove = false;
                AM.Play("Pause", Main);
            }
        }
    }

    public void OptionsMenu()
    {
        if (!optionsMenu.activeSelf)
        {
            pauseMenu.SetActive(false);
            optionsMenu.SetActive(true);

        }
        else
        {
            pauseMenu.SetActive(true);
            optionsMenu.SetActive(false);
        }
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("main_menu");
    }

    public void SetCanShowTrue()
    {
        canShow = true;
    }

    public void SetCanShowFalse()
    {
        canShow = false;
    }
}
