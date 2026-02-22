using UnityEngine;

public class TutorialSceneEntrance : MonoBehaviour
{
    [SerializeField] private DialogueData dialogue;
    [SerializeField] private PlayerMovement player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(player == null)
        {
            player = FindFirstObjectByType<PlayerMovement>();
        }

        DialogueManager.instance.currentDialogue = dialogue;
        DialogueManager.instance.StartDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
