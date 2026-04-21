using System.Collections;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossRoomStart : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject bossGO;
    [SerializeField] BossRoomFunctionality bossRoom;
    [SerializeField] Transform player;
    [SerializeField] SimpleCameraController cameraController;
    [SerializeField] GameObject endScreen;
    [SerializeField] GameObject bossHealthBar;

    [Header("Dialogues")]
    [SerializeField] DialogueData startDialogue;   
    [SerializeField] DialogueData endDialogue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bossGO.GetComponent<DecisionTreeAgent>().enabled = false;  
        StartCoroutine(StartBossDelayed());
    }

    IEnumerator StartBossDelayed()
    {
        yield return null; // wait one frame
        player = GameObject.FindGameObjectWithTag("Player").transform;
        StartBoss();
    }

    void StartBoss()
    {
        bossGO.GetComponent<DecisionTreeAgent>().enabled = false;
        cameraController.player = bossGO.transform;
        bossRoom.onBossEnd.AddListener(EndBoss);
        DialogueManager.instance.currentDialogue = startDialogue;
        DialogueManager.instance.onDialogueEnd.AddListener(EnableBossActions);
        DialogueManager.instance.StartDialogue();
    }

    void EndBoss()
    {

        bossHealthBar.SetActive(false);
        cameraController.player = bossGO.transform;
        bossGO.GetComponent<DecisionTreeAgent>().enabled = false;
        CameraShake.Shake(9999f, 0.05f);
        DialogueManager.instance.currentDialogue = endDialogue;
        DialogueManager.instance.onDialogueEnd.AddListener(EndingFadeMethod);
        DialogueManager.instance.StartDialogue();

    }
    void EnableBossActions()
    {
        bossHealthBar.SetActive(true);
        bossGO.GetComponent<DecisionTreeAgent>().enabled = true;
        cameraController.player = player;
    }

    void EndingFadeMethod()
    {
        StartCoroutine(EndingFadeCoroutine());
    }

    IEnumerator EndingFadeCoroutine()
    {

        RoomTransitionFade.Instance.fadeDuration = 4f;
        CameraShake.Stop();
        yield return StartCoroutine(RoomTransitionFade.Instance.FadeOut(RoomTransitionFade.Instance.fadeDuration));
        bossGO.SetActive(false);
        //endScreen.SetActive(true);
        SceneManager.LoadScene("WinScene");


    }

}
