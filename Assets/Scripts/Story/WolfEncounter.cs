using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class WolfEncounter : MonoBehaviour
{
    [Header("Dialogue Data")]
    [SerializeField] private DialogueData dialogue;
    [SerializeField] private DialogueData secondDialogue;
    [SerializeField] private DialogueData thirdDialogue;

    [Header("References")]
    [SerializeField] private PlayerMovement player;
    [SerializeField] private GameObject wolfPrefab;
    [SerializeField] private Transform wolfSpawnPoint;
    [SerializeField] private Transform wolfTarget;
    [SerializeField] private GameObject wolfEyes;
    [SerializeField] private Volume darknessVolume;
    [SerializeField] private Vignette darknessVignette;
    [SerializeField] private GameObject pressShiftCanvas;

    [SerializeField] private bool frozenTime = false;
    //[SerializeField] private bool pressedShift = false; 
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
            pressShiftCanvas.SetActive(false);
            StartCoroutine(EndTutorial());

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            StartCoroutine(WolfEncounterCoroutine());
            
            gameObject.transform.position = new Vector3(-9999, -9999, -9999);
        }
    }
    IEnumerator WolfEncounterCoroutine()
    {
        PlayerMovement.canMove = false;

        darknessVolume.profile.TryGet(out darknessVignette);
        yield return DarkenVignette();
        wolfEyes.SetActive(true);

        yield return new WaitForSeconds(2f);
        DialogueManager.instance.currentDialogue = dialogue;
        DialogueManager.instance.onDialogueEnd.AddListener(SpawnWolfAndStartSecondDialogueMethod);
        DialogueManager.instance.StartDialogue();
    }

    private void SpawnWolfAndStartSecondDialogueMethod()
    {
        StartCoroutine(SpawnWolfAndStartSecondDialogue());   
    }

    private IEnumerator SpawnWolfAndStartSecondDialogue() 
    {

        DialogueManager.instance.onDialogueEnd.RemoveListener(SpawnWolfAndStartSecondDialogueMethod);

        wolfEyes.SetActive(false);
        wolfGO = Instantiate(wolfPrefab, wolfSpawnPoint.position, Quaternion.identity);
        wolfGO.GetComponent<NavMeshAgent>().enabled = false;
        wolfGO.GetComponent<ChaseAgent>().enabled = false;
        wolfGO.GetComponent<DecisionTreeAgent>().enabled = false;

        wolfGO.transform.GetChild(0).gameObject.SetActive(false);
        yield return LightenVignette();
        //TODO Take off darkness
        DialogueManager.instance.currentDialogue = secondDialogue;
        DialogueManager.instance.onDialogueEnd.AddListener(EnableWolfChase);
        DialogueManager.instance.StartDialogue();
    }
        
    

    private void EnableWolfChase()
    {
        PlayerMovement.canMove = false;
        wolfGO.GetComponent<NavMeshAgent>().enabled = true;
        wolfGO.GetComponent<ChaseAgent>().enabled = true;
        wolfGO.GetComponent<DecisionTreeAgent>().enabled = true;
        wolfGO.GetComponent<ChaseAgent>().target = wolfTarget;
        StartCoroutine(waitToStopWolf());
    }

    IEnumerator waitToStopWolf()
    {
        yield return new WaitForSeconds(0.8f);
        //wolfGO.GetComponent<NavMeshAgent>().enabled = false;
        frozenTime = true;
        pressShiftCanvas.SetActive(true);
        Time.timeScale = 0f;

    }
    
    IEnumerator EndTutorial()
    {
        yield return DarkenVignette();
        wolfEyes.SetActive(true);
        yield return new WaitForSeconds(1f);
        DialogueManager.instance.currentDialogue = thirdDialogue;
        DialogueManager.instance.onDialogueEnd.AddListener(EndTutorialEvent);

        DialogueManager.instance.StartDialogue();
    }

    IEnumerator DarkenVignette()
    {
        float intensity = darknessVignette.intensity.value;
        float smoothness = darknessVignette.smoothness.value;
        //darknessVignette.intensity.value = 1f;
        float timer = 0f;
        while (timer < 4f)
        {
            darknessVignette.intensity.value = Mathf.Lerp(intensity, 1f, timer / 4f);
            darknessVignette.smoothness.value = Mathf.Lerp(smoothness, 1f, timer / 4f);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    void EndTutorialEvent()
    {
        StartCoroutine(LightenVignette());
        wolfEyes.SetActive(false);
    }
    IEnumerator LightenVignette()
    {
        float intensity = darknessVignette.intensity.value;
        float smoothness = darknessVignette.smoothness.value;
        float timer = 0f;
        while (timer < 4f)
        {
            darknessVignette.intensity.value = Mathf.Lerp(intensity, 0.45f, timer / 4f);
            darknessVignette.smoothness.value = Mathf.Lerp(smoothness, 0.2f, timer / 4f);
            timer += Time.deltaTime;
            yield return null;
        }
    }

}
