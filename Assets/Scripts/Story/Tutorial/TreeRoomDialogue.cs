using System.Collections;
using UnityEngine;

public class TreeRoomDialogue : MonoBehaviour
{
    Transform player;
    [SerializeField] GameObject treeEnemy;
    [SerializeField] SimpleCameraController cameraController;
    [SerializeField] DialogueData dialogue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<PlayerMovement>().gameObject.transform;
        if(cameraController == null) cameraController = FindFirstObjectByType<SimpleCameraController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            StartCoroutine(FocusOnEnemyAndStartDialogue());
            GetComponent<Collider>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator FocusOnEnemyAndStartDialogue()
    {
        Debug.Log("Player entered dialogue trigger, focusing on tree enemy and starting dialogue");
        cameraController.player = treeEnemy.transform;
        yield return new WaitForSeconds(0.5f);
        DialogueManager.instance.currentDialogue = dialogue;
        DialogueManager.instance.onDialogueEnd.AddListener(FocusBackOnPlayer);
        Debug.Log("EVENT: " + DialogueManager.instance.onDialogueEnd);
        //DialogueManager.instance.onDialogueEnd.AddListener(FocusBackOnPlayer);
        DialogueManager.instance.StartDialogue();
    }

    void FocusBackOnPlayer()
    {
        Debug.Log("Dialogue ended, focusing back on player");
        cameraController.player = player;
    }
}
