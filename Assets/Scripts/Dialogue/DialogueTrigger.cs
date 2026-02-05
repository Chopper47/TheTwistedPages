using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueData dialogueToTrigger;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            DialogueManager.instance.currentDialogue = dialogueToTrigger;
            DialogueManager.instance.StartDialogue();
        }
    }
}
