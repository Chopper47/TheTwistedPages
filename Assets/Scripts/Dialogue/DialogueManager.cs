using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public TextMeshProUGUI characterName;
    public Image dialoguePanel;
    public DialogueData currentDialogue;
    public UnityEvent onDialogueEnd;
    public UnityEvent onDialogueStart;

    private int index;
    private bool active = false;
    private PlayerMovement player;
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
        //StartDialogue();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
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
        index = 0;
        active = true;
        dialoguePanel.gameObject.SetActive(true);
        characterName.text = currentDialogue.characterName;
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
            active = false;
            

            PlayerMovement.canMove = true;
            player.canRoll = true;
            var tempEvent = onDialogueEnd;
            onDialogueEnd = new UnityEngine.Events.UnityEvent();

            tempEvent?.Invoke();
        }
    }

    
}
