using System.Collections;
using TMPro;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public TextMeshProUGUI characterName;
    public DialogueData dialogue;
    

    private int index;
    private Coroutine typingCoroutine;

    private void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == dialogue.lines[index])
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

                textComponent.text = dialogue.lines[index];
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        characterName.text = dialogue.characterName;
        typingCoroutine = StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        textComponent.text = string.Empty;

        foreach (char c in dialogue.lines[index])
        {
            textComponent.text += c;
            yield return new WaitForSeconds(dialogue.textSpeed);
        }

        typingCoroutine = null;
    }

    void NextLine()
    {
        if (index < dialogue.lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            typingCoroutine = StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
