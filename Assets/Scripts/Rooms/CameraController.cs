using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [Header("References")]
    public Transform player;

    public LevelGenerator levelGenerator;
    private RoomController currentRoom;

    [Header("Camera Follow Settings")]
    public float followSpeed = 5.0f;

    private Vector3 staticCameraPosition;
    private bool followVertical = false;

    private float minZ;
    private float maxZ;

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

        followVertical = room.roomPreset.allowVerticalCameraFollow;

        minZ = room.roomPreset.minFollowZ;
        maxZ = room.roomPreset.maxFollowZ;

        staticCameraPosition = new Vector3(room.transform.position.x, transform.position.y, room.transform.position.z);

        transform.position = staticCameraPosition;
    }

    private void Update()
    {
        if (currentRoom == null || player == null)
        {
            return;
        }

        if (followVertical)
        {
            float targetZ = player.position.z;

            targetZ = Mathf.Clamp(targetZ, minZ, maxZ);

            Vector3 targetPosition = new Vector3(staticCameraPosition.x, staticCameraPosition.y, targetZ);

            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, staticCameraPosition, followSpeed * Time.deltaTime);
        }
    }
}
