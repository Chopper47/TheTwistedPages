using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Elements")]
    public TextMeshProUGUI nextWaveText;
    public TextMeshProUGUI effectText;

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

        if (effectText != null)
        {
            effectText.gameObject.SetActive(false);
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

    public IEnumerator ShowEffectMessage(string message, float time)
    {
        if (effectText == null)
        {
            yield break;
        }

        effectText.gameObject.SetActive(true);
        effectText.text = message;

        float t = 0f;
        float fadeTime = 0.5f;

        Color color = effectText.color;
        color.a = 0f;
        effectText.color = color;

        while (t < fadeTime)
        {
            t += Time.deltaTime;

            color.a = Mathf.Lerp(0f, 1f, t / fadeTime);
            effectText.color = color;

            yield return null;
        }

        yield return new WaitForSeconds(time);

        t = 0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;

            color.a = Mathf.Lerp(1.0f, 0f, t / fadeTime);
            effectText.color = color;

            yield return null;
        }

        effectText.gameObject.SetActive(false);
    }
}
