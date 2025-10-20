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
    private Collider doorCollider;

    private void Awake()
    {
        doorCollider = GetComponent<Collider>();
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
