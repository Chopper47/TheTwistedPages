using System.Collections;
using UnityEngine;

public class TutorialDoor : MonoBehaviour
{
    [SerializeField] private GameObject nextDoor;
    [SerializeField] private GameObject player;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            player = other.gameObject;
            StartCoroutine(MoveToNextRoom());
        }
    }

    IEnumerator MoveToNextRoom()
    {

        PlayerMovement.canMove = false;
        player.GetComponent<PlayerMovement>().canRoll = false;
        yield return RoomTransitionFade.Instance.FadeOut();
        player.transform.position = nextDoor.transform.position;


        yield return new WaitForSeconds(0.4f);

        yield return RoomTransitionFade.Instance.FadeIn();
    }
}
