using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public class DialogueManager : MonoBehaviour
{
    [Header("References")]
    public Image dialoguePanel;
    public Image lumberjackNamePanel;
    public Image wolfNamePanel;
    public TextMeshProUGUI textComponent;
    public InputAction interactAction;
    private PlayerMovement player;

    [Header("Dialogue Components")]
    public DialogueData currentDialogue;
    public TextMeshProUGUI characterName;
    public UnityEvent onDialogueEnd;
    public UnityEvent onDialogueStart;

    private int index;
    private bool active = false;
    private Coroutine typingCoroutine;
    public static DialogueManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        textComponent.text = string.Empty;
        player = FindFirstObjectByType<PlayerMovement>();
        interactAction = player.inputActions.FindActionMap("Player").FindAction("Interact");

        //StartDialogue();
    }

    private void Update()
    {
        if (interactAction.triggered)
        {
            if (active)
            {
                if (textComponent.text == currentDialogue.lines[index])
                {
                    NextLine();
                }
                else
                {
                    if (typingCoroutine != null)
                    {
                        StopCoroutine(typingCoroutine);
                        typingCoroutine = null;
                    }

                    textComponent.text = currentDialogue.lines[index];
                }

            }
        }
    }

    public void StartDialogue()
    {
        PlayerMovement.canMove = false;
        player.canRoll = false;

        PauseMenu.canShow = false;
        index = 0;
        active = true;
        dialoguePanel.sprite = currentDialogue.dialogueBox;
        EnableCorrectNameplate();   
        dialoguePanel.gameObject.SetActive(true);
        //characterName.text = currentDialogue.characterName;
        typingCoroutine = StartCoroutine(TypeLine());
    }

    private IEnumerator LateStart()
    {
        yield return new WaitForSeconds(0.1f);
    }
    IEnumerator TypeLine()
    {
        textComponent.text = string.Empty;

        foreach (char c in currentDialogue.lines[index])
        {
            textComponent.text += c;
            yield return new WaitForSeconds(currentDialogue.textDelay);
        }

        typingCoroutine = null;
    }

    void NextLine()
    {
        if (index < currentDialogue.lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            typingCoroutine = StartCoroutine(TypeLine());
        }
        else
        {
            dialoguePanel.gameObject.SetActive(false);
            DisableNameplates();
            active = false;
            

            PlayerMovement.canMove = true;
            player.canRoll = true;
            PauseMenu.canShow = true;

            var tempEvent = onDialogueEnd;
            onDialogueEnd = new UnityEngine.Events.UnityEvent();

            tempEvent?.Invoke();

            //onDialogueEnd?.Invoke();
            //Debug.Log("Listeners count: " + onDialogueEnd.GetPersistentEventCount());

            //onDialogueEnd.RemoveAllListeners();
        }
    }

    private void EnableCorrectNameplate()
    {
        if(currentDialogue.characterName == "Lumberjack")
        {
            lumberjackNamePanel.gameObject.SetActive(true);
            wolfNamePanel.gameObject.SetActive(false);
        }
        else if (currentDialogue.characterName == "Big Bad Wolf")
        {
            lumberjackNamePanel.gameObject.SetActive(false);
            wolfNamePanel.gameObject.SetActive(true);
        }
        else
        {
            lumberjackNamePanel.gameObject.SetActive(false);
            wolfNamePanel.gameObject.SetActive(false);
        }
    }

    private void DisableNameplates()
    {
        lumberjackNamePanel.gameObject.SetActive(false);
        wolfNamePanel.gameObject.SetActive(false);
    }

}
