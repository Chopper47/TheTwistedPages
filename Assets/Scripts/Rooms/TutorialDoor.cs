using System.Collections;
using UnityEngine;

public class TutorialDoor : MonoBehaviour
{
    [SerializeField] private GameObject nextDoor;
    [SerializeField] private GameObject player;
    [SerializeField] private bool isActive = true;
    bool isClosed = true;

    private void Start()
    {
        CloseDoor();
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !isClosed && isActive)
        {
            player = other.gameObject;
            StartCoroutine(MoveToNextRoom());
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            if(isClosed)
            {
                OpenDoor();
            }
            else
            {
                CloseDoor();
            }
        }
    }
    IEnumerator MoveToNextRoom()
    {

        PlayerMovement.canMove = false;
        player.GetComponent<PlayerMovement>().canRoll = false;
        yield return RoomTransitionFade.Instance.FadeOut(RoomTransitionFade.Instance.fadeDuration);
        player.transform.position = nextDoor.transform.position;
        
        yield return new WaitForSeconds(0.4f);
        
        yield return RoomTransitionFade.Instance.FadeIn(RoomTransitionFade.Instance.fadeDuration);
    }

    public void CloseDoor()
    {
        GetComponentInChildren<Light>().enabled = false;
        isClosed = true;
    }

    public void OpenDoor()
    {
        GetComponentInChildren<Animator>().SetTrigger("OpenDoor");
        GetComponentInChildren<Light>().enabled = true;
        isClosed = false;
    }
}
