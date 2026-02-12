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

    private float minZ;
    private float maxZ;

    private float minX;
    private float maxX;

    [Header("Look Ahead Settings")]
    public float lookAheadDistance = 2.0f;
    public float lookAheadSmooth = 4.0f;

    private Vector3 lastPlayerPosition;
    private Vector3 lookAheadOffset;

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

        lastPlayerPosition = player.position;
    }

    /// <summary>
    /// Teleports the camera to follow the player inside the room bounds.
    /// </summary>
    public void MoveToRoom(RoomController room, Vector3 playerFinalPosition)
    {
        currentRoom = room;

        minZ = room.transform.position.z + room.roomPreset.minFollowZ;
        maxZ = room.transform.position.z + room.roomPreset.maxFollowZ;

        minX = room.transform.position.x + room.roomPreset.minFollowX;
        maxX = room.transform.position.x + room.roomPreset.maxFollowX;

        float targetZ = Mathf.Clamp(playerFinalPosition.z, minZ, maxZ);
        float targetX = Mathf.Clamp(playerFinalPosition.x, minX, maxX);

        transform.position = new Vector3(targetX, transform.position.y, targetZ);

        lastPlayerPosition = playerFinalPosition;
        lookAheadOffset = Vector3.zero;
    }

    private void Update()
    {
        if (currentRoom == null || player == null)
        {
            return;
        }

        Vector3 playerDelta = player.position - lastPlayerPosition;
        Vector3 targetLookAhead = Vector3.zero;

        if (playerDelta.sqrMagnitude > 0.0001f)
        {
            Vector3 moveDir = playerDelta.normalized;
            targetLookAhead = moveDir * lookAheadDistance;
        }

        lookAheadOffset = Vector3.Lerp(lookAheadOffset, targetLookAhead, lookAheadSmooth * Time.deltaTime);
        lastPlayerPosition = player.position;

        float targetX = Mathf.Clamp(player.position.x, minX, maxX);
        float targetZ = Mathf.Clamp(player.position.z, minZ, maxZ);

        Vector3 targetPosition = new Vector3(targetX, transform.position.y, targetZ) + lookAheadOffset;

        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}
