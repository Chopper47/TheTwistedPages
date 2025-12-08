using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [Header("Main Menu")]
    public GameObject mainMenu;

    [Header("Options Menu")]
    public GameObject optionsMenu;

    [Header("Credits Menu")]
    public GameObject creditsMenu;

    [Header("Upgrades Menu")]
    public GameObject upgradesMenu;

    [Header("Upgrades Menu")]
    public GameObject controlsMenu;
    private void Awake()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        upgradesMenu.SetActive(false);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("alpha");
    }

    public void OptionsMenu()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void CreditsMenu()
    {
        mainMenu.SetActive(false);
        creditsMenu.SetActive(true);
    }
    public void UpgradessMenu()
    {
        mainMenu.SetActive(false);
        upgradesMenu.SetActive(true);
    }

    public void ControlsMenu()
    {
        mainMenu.SetActive(false);
        controlsMenu.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        creditsMenu.SetActive(false);
        optionsMenu.SetActive(false);
        upgradesMenu.SetActive(false);
        controlsMenu.SetActive(false);
        mainMenu.SetActive(true);

    }
    public void ExitGame()
    {
        Application.Quit();
    }
}
