using UnityEngine;

[RequireComponent(typeof(Collider))]
public class RoomTrigger : MonoBehaviour
{
    private RoomController roomController;

    private void Awake()
    {
        roomController = GetComponentInParent<RoomController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            roomController.ActivateRoom();
        }
    }
}
