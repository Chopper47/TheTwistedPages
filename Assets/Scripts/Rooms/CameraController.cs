using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    public Transform player;
    public LevelGenerator levelGenerator;

    private RoomController currentRoom;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        if (levelGenerator == null)
        {
            levelGenerator = FindFirstObjectByType<LevelGenerator>();
        }
    }

    /// <summary>
    /// Teleports the camera to the center of a given room.
    /// </summary>
    public void MoveToRoom(RoomController room)
    {
        currentRoom = room;

        Vector3 roomCenter = room.transform.position;
        transform.position = roomCenter;
    }
}
