using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class WolfEncounter : MonoBehaviour
{
    [SerializeField] private DialogueData dialogue;
    [SerializeField] private PlayerMovement player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            StartCoroutine(WolfEncounterCoroutine());
            DialogueManager.instance.currentDialogue = dialogue;
            DialogueManager.instance.StartDialogue();
        }
    }
    IEnumerator WolfEncounterCoroutine()
    {
        //asdasdasda
        yield return new WaitForSeconds(2f); 
    }
}
