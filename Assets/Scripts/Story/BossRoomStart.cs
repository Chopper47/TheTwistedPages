using UnityEngine;

public class BossRoomStart : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject bossGO;
    [SerializeField] BossRoomFunctionality bossRoom;
    [SerializeField] Transform player;
    [SerializeField] SimpleCameraController cameraController;

    [Header("Dialogues")]
    [SerializeField] DialogueData startDialogue;   
    [SerializeField] DialogueData endDialogue;   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        cameraController.player = bossGO.transform;
        bossGO.GetComponent<DecisionTreeAgent>().enabled = false;
        CameraShake.Shake(9999f, 0.05f);
        DialogueManager.instance.currentDialogue = endDialogue;
        DialogueManager.instance.onDialogueEnd.AddListener(EndingFade);
        DialogueManager.instance.StartDialogue();

    }
    void EnableBossActions()
    {
        bossGO.GetComponent<DecisionTreeAgent>().enabled = true;
        cameraController.player = player;
    }

    void EndingFade()
    {

        RoomTransitionFade.Instance.fadeDuration = 4f;
        StartCoroutine(RoomTransitionFade.Instance.FadeOut());
    }

}
