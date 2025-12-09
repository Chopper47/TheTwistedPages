using UnityEngine;
using UnityEngine.UI;   // or TMP
using TMPro;
using System.Collections.Generic;

public class OptionsMenu : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;

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
        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
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
        soundMenu.SetActive(false);
        graphicsMenu.SetActive(false);
        controllerMenu.SetActive(false);
        keyboardMenu.SetActive(false);
    }
    public void SetResolution(int resolutionIndex)
    {
        Debug.Log("Setting resolution" + resolutionIndex);
        Resolution res = resolutions[resolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);
    }

    public void SetScreenDisplayMode(int mode)
    {
        switch (mode)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                Debug.Log("windowed");
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                Debug.Log("borderless");

                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                Debug.Log("full screen");

                break;

        }
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
