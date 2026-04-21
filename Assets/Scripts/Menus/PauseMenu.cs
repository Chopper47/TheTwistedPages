using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement player;

    [Header("Input System")]
    public InputActionAsset inputActions;
    private InputAction pauseAction;

    [Header("Pause State")]
    public bool isPaused = false;
    public static bool canShow = true;

    [Header("Menus")]
    public GameObject pauseMenu;
    public GameObject optionsMenu;

    [Header("SFX")]
    public AudioManager AM;
    public AudioSource Main;

    private void Awake()
    {
        pauseAction = inputActions.FindActionMap("Player").FindAction("Pause");

        player = FindFirstObjectByType<PlayerMovement>();
        AM = FindFirstObjectByType<AudioManager>();
        Main = GetComponentInChildren<AudioSource>();

    }

    private void Start()
    {
        UIManager.Instance.CloseMenu(pauseMenu);

    }

    private void OnEnable()
    {
        pauseAction.Enable();
    }

    private void OnDisable()
    {
        pauseAction.Disable();
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
        if (!canShow)
        {
            return;
        }

        isPaused = !isPaused;

        if (isPaused)
        {
            Pause();
        }
        else
        {
            Unpause();
        }
    }

    private void Pause()
    {
        Time.timeScale = 0;

        PlayerMovement.canMove = false;
        player.canRoll = false;

        UIManager.Instance.OpenMenu(pauseMenu);
        UIManager.Instance.ShowHUD(false);

        AM.Play("Pause", Main);
    }

    private void Unpause()
    {
        Time.timeScale = 1;

        PlayerMovement.canMove = true;
        player.canRoll = true;

        UIManager.Instance.CloseMenu(optionsMenu);
        UIManager.Instance.CloseMenu(pauseMenu);
        UIManager.Instance.ShowHUD(true);

        AM.Play("Unpause", Main);
    }

    public void OptionsMenu()
    {
        Debug.Log("CALLED");
        UIManager.Instance.SwitchMenu(pauseMenu, optionsMenu);
    }

    public void CloseOptionsMenu()
    {
        UIManager.Instance.SwitchMenu(optionsMenu, pauseMenu);
    }

    public void BackToPause()
    {
        UIManager.Instance.SwitchMenu(optionsMenu, pauseMenu);
    }

    public void ReturnToMainMenu()
    {
        if(SaveManager.instance != null)
        {
            SaveManager.instance.SavePlayerData();
        }
        Time.timeScale = 1;
        SceneManager.LoadScene("main_menu");
    }
}
