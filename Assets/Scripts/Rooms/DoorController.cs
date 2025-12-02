using UnityEngine;

public enum DoorDirection
{
    North,
    South,
    East,
    West
}

[RequireComponent(typeof(Collider))]
public class DoorController : MonoBehaviour
{
    public DoorDirection direction;
    public RoomController parentRoom;

    private Collider doorCollider;
    private RoomTransition roomTransition;
    //public MusicManager music;

    private void Awake()
    {
        doorCollider = GetComponent<Collider>();
        roomTransition = FindFirstObjectByType<RoomTransition>();
        //music = GameObject.FindFirstObjectByType<MusicManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(roomTransition.TryMoveToAdjacentRoom(parentRoom, direction));
        }
    }

    public void OpenDoor()
    {
        doorCollider.isTrigger = true;
        //MusicManager.instance.SwitchStageSynced(0);
    }

    public void CloseDoor()
    {
        doorCollider.isTrigger = false;
    }
}
