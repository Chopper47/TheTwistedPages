using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Menus")]
    public GameObject mainMenu;
    public GameObject optionsMenu;
    public GameObject creditsMenu;
    public GameObject upgradesMenu;
    public GameObject controlsMenu;

    private void Awake()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        upgradesMenu.SetActive(false);
        controlsMenu.SetActive(false);

        UIManager.Instance.OpenMenu(mainMenu);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Hub");
    }

    public void GoToBoss()
    {
        SceneManager.LoadScene("Scene_06_BossRoomTest");
    }

    public void GoToMainScene()
    {
        SceneManager.LoadScene("MainGame");
    }

    public void OptionsMenu()
    {
        UIManager.Instance.SwitchMenu(mainMenu, optionsMenu);
    }

    public void CreditsMenu()
    {
        UIManager.Instance.SwitchMenu(mainMenu, creditsMenu);
    }

    public void UpgradesMenu()
    {
        UIManager.Instance.SwitchMenu(mainMenu, upgradesMenu);
    }

    public void ControlsMenu()
    {
        UIManager.Instance.SwitchMenu(mainMenu, controlsMenu);
    }

    public void ReturnToMainMenu()
    {
        UIManager.Instance.CloseMenu(optionsMenu);
        UIManager.Instance.CloseMenu(creditsMenu);
        UIManager.Instance.CloseMenu(upgradesMenu);
        UIManager.Instance.CloseMenu(controlsMenu);

        UIManager.Instance.OpenMenu(mainMenu);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
