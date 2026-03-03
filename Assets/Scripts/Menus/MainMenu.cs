using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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

    [Header("Controls Menu")]
    public GameObject controlsMenu;

    private void Awake()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
       creditsMenu.SetActive(false);
        upgradesMenu.SetActive(false);
        controlsMenu.SetActive(false);

        SelectFirst(mainMenu);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("HubRoom");
    }
    public void GoToBoss()
    {
        SceneManager.LoadScene("Scene_06_BossRoomTest");
    }

    public void GoToMainScene()
    {
        SceneManager.LoadScene("alpha");
    }

    public void OptionsMenu()
    {
        SwitchMenu(mainMenu, optionsMenu);
    }

    public void CreditsMenu()
    {
        SwitchMenu(mainMenu, creditsMenu);
    }

    public void UpgradessMenu()
    {
        SwitchMenu(mainMenu, upgradesMenu);
    }

    public void ControlsMenu()
    {
        SwitchMenu(mainMenu, controlsMenu);
    }

    public void ReturnToMainMenu()
    {
        optionsMenu.SetActive(false);
        creditsMenu.SetActive(false);
        upgradesMenu.SetActive(false);
        controlsMenu.SetActive(false);

        mainMenu.SetActive(true);
        SelectFirst(mainMenu);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void SwitchMenu(GameObject from, GameObject to)
    {
        Debug.Log("SWITCH MENU");
        from.SetActive(false);
        to.SetActive(true);
        SelectFirst(to);
    }

    private void SelectFirst(GameObject menu)
    {
        MenuNavigator navigator = menu.GetComponent<MenuNavigator>();
        if (navigator)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(navigator.firstSelected);
        }
    }
}
