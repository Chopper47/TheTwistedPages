using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    [Header("Tabs")]
    [SerializeField] private RectTransform soundTab;
    [SerializeField] private RectTransform graphicsTab;
    [SerializeField] private RectTransform controllerTab;
    [SerializeField] private RectTransform keyboardTab;

    [SerializeField] private RectTransform bookPage;

    private RectTransform currentActiveTab;

    [Header("Sound Menu")]
    public GameObject soundMenu;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider audioVolumeSlider;

    [Header("Graphics Menu")]
    public GameObject graphicsMenu;

    [Header("Controller Menu")]
    public GameObject controllerMenu;

    [Header("Keyboard Menu")]
    public GameObject keyboardMenu;

    [Header("Display Mode Selector")]
    [SerializeField] private TMP_Text displayModeText;

    private int currentScreenModeIndex = 0;

    private FullScreenMode[] screenModes = new FullScreenMode[]
    {
        FullScreenMode.Windowed,
        FullScreenMode.FullScreenWindow,
        FullScreenMode.ExclusiveFullScreen
    };

    private string[] screenModeNames = new string[]
    {
        "Windowed",
        "Borderless",
        "Fullscreen"
    };

    [Header("Resolution Selector")]
    [SerializeField] private TMP_Text resolutionText;

    private Resolution[] resolutions;
    private int currentResolutionIndex = 0;

    private void Awake()
    {
        int width = PlayerPrefs.GetInt("resWidth", 1920);
        int height = PlayerPrefs.GetInt("resHeight", 1080);

        currentScreenModeIndex = PlayerPrefs.GetInt("ScreenMode", 0);

        ApplyScreenMode(false);

        Screen.SetResolution(width, height, screenModes[currentScreenModeIndex]);
    }

    private void Start()
    {
        resolutions = Screen.resolutions;

        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.width &&
                resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
                break;
            }
        }

        UpdateResolutionText();

        ChangeVolumeSliderValue(PlayerPrefs.GetFloat("MusicVolume", 0.5f));
        ChangeAudioVolumeSliderValue(PlayerPrefs.GetFloat("AudioVolume", 0.5f));

        OpenSound();
    }

    public void NextScreenMode()
    {
        currentScreenModeIndex++;

        if (currentScreenModeIndex >= screenModes.Length)
        {
            currentScreenModeIndex = 0;
        }

        ApplyScreenMode();
    }

    public void PreviousScreenMode()
    {
        currentScreenModeIndex--;

        if (currentScreenModeIndex < 0)
        {
            currentScreenModeIndex = screenModes.Length - 1;
        }

        ApplyScreenMode();
    }

    private void ApplyScreenMode(bool save = true)
    {
        Screen.fullScreenMode = screenModes[currentScreenModeIndex];

        if (save)
        {
            PlayerPrefs.SetInt("ScreenMode", currentScreenModeIndex);
            PlayerPrefs.Save();
        }

        UpdateDisplayModeText();
    }

    private void UpdateDisplayModeText()
    {
        displayModeText.text = screenModeNames[currentScreenModeIndex];
    }

    public void NextResolution()
    {
        currentResolutionIndex++;

        if (currentResolutionIndex >= resolutions.Length)
        {
            currentResolutionIndex = 0;
        }

        ApplyResolution();
    }

    public void PreviousResolution()
    {
        currentResolutionIndex--;

        if (currentResolutionIndex < 0)
        {
            currentResolutionIndex = resolutions.Length - 1;
        }

        ApplyResolution();
    }

    private void ApplyResolution()
    {
        Resolution res = resolutions[currentResolutionIndex];

        Screen.SetResolution(res.width, res.height, Screen.fullScreenMode);

        PlayerPrefs.SetInt("resWidth", res.width);
        PlayerPrefs.SetInt("resHeight", res.height);
        PlayerPrefs.Save();

        UpdateResolutionText();
    }

    private void UpdateResolutionText()
    {
        Resolution res = resolutions[currentResolutionIndex];
        resolutionText.text = res.width + " x " + res.height;
    }

    public void OpenSound()
    {
        CloseAllOptionMenus();
        soundMenu.SetActive(true);
        SetActiveTab(soundTab);
    }

    public void OpenGraphics()
    {
        CloseAllOptionMenus();
        graphicsMenu.SetActive(true);
        SetActiveTab(graphicsTab);
    }

    public void OpenControllerControls()
    {
        CloseAllOptionMenus();
        controllerMenu.SetActive(true);
        SetActiveTab(controllerTab);
    }

    public void OpenKeyboardControls()
    {
        CloseAllOptionMenus();
        keyboardMenu.SetActive(true);
        SetActiveTab(keyboardTab);
    }

    private void CloseAllOptionMenus()
    {
        if (soundMenu) soundMenu.SetActive(false);
        if (graphicsMenu) graphicsMenu.SetActive(false);
        if (controllerMenu) controllerMenu.SetActive(false);
        if (keyboardMenu) keyboardMenu.SetActive(false);
    }

    private void SetActiveTab(RectTransform activeTab)
    {
        if (currentActiveTab == activeTab)
        {
            return;
        }

        currentActiveTab = activeTab;

        int pageIndex = bookPage.GetSiblingIndex();
        int behindIndex = Mathf.Max(0, pageIndex - 1);

        soundTab.SetSiblingIndex(behindIndex);
        graphicsTab.SetSiblingIndex(behindIndex);
        controllerTab.SetSiblingIndex(behindIndex);
        keyboardTab.SetSiblingIndex(behindIndex);

        activeTab.SetAsLastSibling();
    }

    public void ChangeVolumeSliderValue(float volume)
    {
        MusicManager.instance.SetVolumeOfStagers(volume);
        musicVolumeSlider.value = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void ChangeAudioVolumeSliderValue(float volume)
    {
        AudioManager.Instance.defaultSource.volume = volume;
        audioVolumeSlider.value = volume;
        PlayerPrefs.SetFloat("AudioVolume", volume);
    }
}
