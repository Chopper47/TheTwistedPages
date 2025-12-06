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
    public void MoveToRoom(RoomController room, Vector3 playerFinalPosition)
    {
        currentRoom = room;

        followVertical = room.roomPreset.allowVerticalCameraFollow;

        Vector3 minPoint = room.transform.position + room.transform.rotation * new Vector3(0f, 0f, room.roomPreset.minFollowZ);
        Vector3 maxPoint = room.transform.position + room.transform.rotation * new Vector3(0f, 0f, room.roomPreset.maxFollowZ);

        minZ = Mathf.Min(minPoint.z, maxPoint.z);
        maxZ = Mathf.Max(minPoint.z, maxPoint.z);

        float targetZ = Mathf.Clamp(playerFinalPosition.z, minZ, maxZ);

        staticCameraPosition = new Vector3(room.transform.position.x, transform.position.y, targetZ);

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
            float targetZ = Mathf.Clamp(player.position.z, minZ, maxZ);

            Vector3 targetPosition = new Vector3(staticCameraPosition.x, staticCameraPosition.y, targetZ);

            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, staticCameraPosition, followSpeed * Time.deltaTime);
        }
    }
}
