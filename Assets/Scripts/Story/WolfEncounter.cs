using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class WolfEncounter : MonoBehaviour
{
    [Header("Dialogue Data")]
    [SerializeField] private DialogueData dialogue;
    [SerializeField] private DialogueData secondDialogue;

    [Header("References")]
    [SerializeField] private PlayerMovement player;
    [SerializeField] private GameObject wolfPrefab;
    [SerializeField] private Transform wolfSpawnPoint;
    [SerializeField] private Transform wolfTarget;

    [SerializeField] private bool frozenTime = false;
    [SerializeField] private bool pressedShift = false; 
    private InputAction rollInput;

    private GameObject wolfGO;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rollInput = player.inputActions.FindActionMap("Player").FindAction("Roll");
    }

    // Update is called once per frame
    void Update()
    {
        if(frozenTime && rollInput.triggered)
        {
            frozenTime = false;
            PlayerMovement.canMove = true;
            Time.timeScale = 1f;
            //wolfGO.GetComponent<NavMeshAgent>().enabled = true;

            StartCoroutine(player.Roll());

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            StartCoroutine(WolfEncounterCoroutine());
            DialogueManager.instance.currentDialogue = dialogue;
            DialogueManager.instance.onDialogueEnd.AddListener(SpawnWolfAndStartSecondDialogue);
            DialogueManager.instance.StartDialogue();
        }
    }
    IEnumerator WolfEncounterCoroutine()
    {
        //TODO start darkness
        yield return new WaitForSeconds(2f); 
        //ADD dialogue start here
    }

    private void SpawnWolfAndStartSecondDialogue()
    {
        DialogueManager.instance.onDialogueEnd.RemoveListener(SpawnWolfAndStartSecondDialogue);
        wolfGO = Instantiate(wolfPrefab, wolfSpawnPoint.position, Quaternion.identity);
        wolfGO.GetComponent<NavMeshAgent>().enabled = false;
        //TODO Take off darkness
        DialogueManager.instance.currentDialogue = secondDialogue;
        DialogueManager.instance.onDialogueEnd.AddListener(EnableWolfChase);
        DialogueManager.instance.StartDialogue();
    }

    private void EnableWolfChase()
    {
        PlayerMovement.canMove = false;
        wolfGO.GetComponent<NavMeshAgent>().enabled = true;
        wolfGO.GetComponent<ChaseAgent>().target = wolfTarget;
        StartCoroutine(waitToStopWolf());
    }

    IEnumerator waitToStopWolf()
    {
        yield return new WaitForSeconds(0.8f);
        //wolfGO.GetComponent<NavMeshAgent>().enabled = false;
        frozenTime = true;
        Time.timeScale = 0f;



    }
}
