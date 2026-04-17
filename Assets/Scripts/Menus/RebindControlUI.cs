using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindControlUI : MonoBehaviour
{
    public InputActionReference actionReference;
    public int bindingIndex = 0;
    public TextMeshProUGUI bindingText;

    private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

    void Start()
    {
        if (PlayerPrefs.HasKey("rebinds"))
        {
            actionReference.action.actionMap.LoadBindingOverridesFromJson(
                PlayerPrefs.GetString("rebinds")
            );
        }
        UpdateBindingDisplay();
    }

    public void StartRebind()
    {
        bindingText.text = "Press a key...";

        actionReference.action.Disable();

        rebindingOperation = actionReference.action
            .PerformInteractiveRebinding(bindingIndex)
            .WithControlsExcluding("Mouse")
            .OnComplete(operation =>
            {
                actionReference.action.Enable();

                PlayerPrefs.SetString("rebinds", actionReference.action.actionMap.SaveBindingOverridesAsJson());
                PlayerPrefs.Save();

                operation.Dispose();
                UpdateBindingDisplay();

            })
            .Start();
    }

    void UpdateBindingDisplay()
    {
        bindingText.text = InputControlPath.ToHumanReadableString(
            actionReference.action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

}
