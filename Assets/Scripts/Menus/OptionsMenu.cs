using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    [Header("Sound Menu")]
    public GameObject soundMenu;

    [Header("Graphics Menu")]
    public GameObject graphicsMenu;

    [Header("Controller Menu")]
    public GameObject controllerMenu;

    [Header("Keyboard Menu")]
    public GameObject keyboardMenu;
    public void OpenSound()
    {
        CloseAllOptionMenus();
        soundMenu.SetActive(true);
    }
    public void OpenGraphics()
    {
        CloseAllOptionMenus();
        graphicsMenu.SetActive(true);
    }
    public void OpenControllerControls()
    {
        CloseAllOptionMenus();
        controllerMenu.SetActive(true);
    }
    public void OpenKeyboardControls()
    {
        CloseAllOptionMenus();
        keyboardMenu.SetActive(true);
    }

    private void CloseAllOptionMenus()
    {
        soundMenu.SetActive(false);
        graphicsMenu.SetActive(false);
        controllerMenu.SetActive(false);
        keyboardMenu.SetActive(false);
    }
}
