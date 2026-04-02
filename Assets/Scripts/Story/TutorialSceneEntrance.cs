using UnityEngine;

public class TutorialSceneEntrance : MonoBehaviour
{
    [SerializeField] private DialogueData dialogue;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private TutorialDoor doorToOpen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(player == null)
        {
            player = FindFirstObjectByType<PlayerMovement>();
            player.gameObject.GetComponent<PlayerHealth>().inTutorial = true;
        }

        DialogueManager.instance.currentDialogue = dialogue;
        DialogueManager.instance.StartDialogue();
        doorToOpen.OpenDoor();
    }


    
}
