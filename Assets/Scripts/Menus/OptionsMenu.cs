using UnityEngine;
using UnityEngine.UI;   // or TMP
using TMPro;
using System.Collections.Generic;

public class OptionsMenu : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;
    Resolution? currentResolution;
    FullScreenMode? currentScreenMode;

    [Header("Sound Menu")]
    public GameObject soundMenu;

    [Header("Graphics Menu")]
    public GameObject graphicsMenu;

    [Header("Controller Menu")]
    public GameObject controllerMenu;

    [Header("Keyboard Menu")]
    public GameObject keyboardMenu;

    void Awake()
    {
        // Load resolution values with defaults
        int width = PlayerPrefs.GetInt("resWidth", 1920);
        int height = PlayerPrefs.GetInt("resHeight", 1080);
        int modeIndex = PlayerPrefs.GetInt("ScreenMode", 0);

        // Apply fullscreen mode
        SetScreenDisplayMode(modeIndex);

        // Apply resolution using the current fullscreen mode
        Screen.SetResolution(width, height, Screen.fullScreenMode);
    }
    private void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        int currentResolutionIndex = 0;
        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.width &&
                resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }
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
        if(soundMenu)soundMenu.SetActive(false);
        if (graphicsMenu) graphicsMenu.SetActive(false);
        if (controllerMenu) controllerMenu.SetActive(false);
        if (keyboardMenu) keyboardMenu.SetActive(false);
    }
    public void SetResolution(int resolutionIndex)
    {
        Debug.Log("Setting resolution" + resolutionIndex);
        currentResolution = resolutions[resolutionIndex];
        PlayerPrefs.SetInt("resWidth", currentResolution.Value.width);
        PlayerPrefs.SetInt("resHeight", currentResolution.Value.height);
        PlayerPrefs.Save();

        Screen.SetResolution(currentResolution.Value.width, currentResolution.Value.height, Screen.fullScreenMode);
    }

    public void SetScreenDisplayMode(int mode)
    {
        switch (mode)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;

            case 1:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;

            case 2:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
        }

        PlayerPrefs.SetInt("ScreenMode", mode);
        PlayerPrefs.Save();
    }
    public void ChangeVolumeSliderValue(float volume)
    {
        MusicManager.instance.SetVolumeOfStagers(volume);
    }
    public void ChangeAudioVolumeSliderValue(float volume)
    {
        AudioManager.Instance.defaultSource.volume = volume;
    }
}
