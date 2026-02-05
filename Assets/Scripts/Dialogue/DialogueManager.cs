using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public TextMeshProUGUI characterName;
    public Image dialoguePanel;
    public DialogueData currentDialogue;
    

    private int index;
    private bool active = false;
    private Coroutine typingCoroutine;

    public static DialogueManager instance;
    private void Awake()
    {
        if(instance != this)
        {
            instance = this;
        }
    }
    private void Start()
    {
        textComponent.text = string.Empty;
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
        index = 0;
        active = true;
        dialoguePanel.gameObject.SetActive(true);
        characterName.text = currentDialogue.characterName;
        typingCoroutine = StartCoroutine(TypeLine());
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
        }
    }
}
