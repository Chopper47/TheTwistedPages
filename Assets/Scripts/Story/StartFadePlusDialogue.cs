using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFadePlusDialogue : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;


    [SerializeField] private DialogueData firstDialogue;
    [SerializeField] private DialogueData secondDialogue;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = FindFirstObjectByType<PlayerMovement>();
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return null;
        DisablePlayerMovement();
        StartCoroutine(PlayBookFallSound());

    }

    public IEnumerator PlayBookFallSound()
    {
        yield return new WaitForSeconds(0.5f);
        //AudioManager.Instance.Play("BookFall");
        yield return new WaitForSeconds(0.5f);
        DialogueManager.instance.currentDialogue = firstDialogue;
        DialogueManager.instance.StartDialogue();
    }
    public void StartFirstFadeInMethod()
    {
        DisablePlayerMovement();
        RoomTransitionFade.Instance.fadeDuration = 4f;
        StartCoroutine(StartFirstFadeIn());
    }

    IEnumerator StartFirstFadeIn()
    {
        yield return StartCoroutine(RoomTransitionFade.Instance.FadeIn());
        DialogueManager.instance.currentDialogue = secondDialogue;
        DialogueManager.instance.StartDialogue();
        RoomTransitionFade.Instance.fadeDuration = 0.4f;

    }


    void DisablePlayerMovement()
    {
        PlayerMovement.canMove = false;
        player.canRoll = false;
    }
    void EnablePlayerMovement()
    {
        PlayerMovement.canMove = true;
        player.canRoll = true;
    }
}
