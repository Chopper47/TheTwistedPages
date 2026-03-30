using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Elements")]
    public TextMeshProUGUI nextWaveText;

    [Header("Menus")]
    public GameObject pauseMenu;
    public GameObject optionsMenu;
    public GameObject gameWonMenu;
    public GameObject gameOverMenu;

    [Header("HUD")]
    public GameObject HUD;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SwitchMenu(GameObject from, GameObject to)
    {
        if (from)
        {
            from.SetActive(false);
        }

        if (to)
        {
            to.SetActive(true);
        }

        SelectFirst(to);
    }

    public void OpenMenu(GameObject menu)
    {
        if (menu)
        {
            menu.SetActive(true);
        }

        SelectFirst(menu);
    }

    public void CloseMenu(GameObject menu)
    {
        if (menu)
        {
            menu.SetActive(false);
        }
    }

    public void ShowHUD(bool show)
    {
        if (HUD)
        {
            HUD.SetActive(show);
        }
    }

    private void SelectFirst(GameObject menu)
    {
        if (menu == null)
        {
            return;
        }

        MenuNavigator nav = menu.GetComponent<MenuNavigator>();

        if (nav)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(nav.firstSelected);
        }
    }
}
