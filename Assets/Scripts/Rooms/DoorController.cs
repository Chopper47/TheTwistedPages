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

    private void Awake()
    {
        doorCollider = GetComponent<Collider>();
        roomTransition = FindFirstObjectByType<RoomTransition>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            roomTransition.TryMoveToAdjacentRoom(parentRoom, direction);
        }
    }

    public void OpenDoor()
    {
        doorCollider.isTrigger = true;
    }

    public void CloseDoor()
    {
        doorCollider.isTrigger = false;
    }
}
