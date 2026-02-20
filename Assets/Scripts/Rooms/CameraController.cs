using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [Header("References")]
    public Transform player;

    public LevelGenerator levelGenerator;
    private RoomController currentRoom;

    [Header("Camera Follow Settings")]
    public float followSmoothTime = 0.15f;
    private Vector3 followVelocity = Vector3.zero;

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
        followVelocity = Vector3.zero;
    }

    private void LateUpdate()
    {
        if (currentRoom == null || player == null)
        {
            return;
        }

        UpdateLookAhead();
        UpdateCameraFollow();
    }

    private void UpdateLookAhead()
    {
        Vector3 playerDelta = player.position - lastPlayerPosition;

        Vector3 targetLookAhead = Vector3.zero;

        if (playerDelta.sqrMagnitude > 0.0001f)
        {
            Vector3 moveDirection = playerDelta.normalized;

            if (Mathf.Abs(moveDirection.x) > Mathf.Abs(moveDirection.z))
            {
                moveDirection = new Vector3(moveDirection.x, 0, 0);
            }
            else
            {
                moveDirection = new Vector3(0, 0, moveDirection.z);
            }

            targetLookAhead = moveDirection * lookAheadDistance;
        }

        lookAheadOffset = Vector3.Lerp(lookAheadOffset, targetLookAhead, lookAheadSmooth * Time.deltaTime);

        lastPlayerPosition = player.position;
    }

    private void UpdateCameraFollow()
    {
        float targetX = Mathf.Clamp(player.position.x, minX, maxX);
        float targetZ = Mathf.Clamp(player.position.z, minZ, maxZ);

        Vector3 targetPosition = new Vector3(targetX, transform.position.y, targetZ);

        targetPosition += lookAheadOffset;

        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.z = Mathf.Clamp(targetPosition.z, minZ, maxZ);
        
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref followVelocity, followSmoothTime);
    }

    public bool IsCurrentRoomFinished()
    {
        return currentRoom.isCleared;
    }
}
