using UnityEngine;

public class SimpleCameraController : MonoBehaviour
{
    [Header("References")]
    public Transform player;

    [Header("Camera Follow Settings")]
    public float followSmoothTime = 0.15f;
    private Vector3 followVelocity = Vector3.zero;

    [Header("Bounds")]
    public float minX;
    public float maxX;

    public float minZ;
    public float maxZ;

    [Header("Look Ahead Settings")]
    public float lookAheadDistance = 2.0f;
    public float lookAheadSmooth = 4.0f;

    private Vector3 lastPlayerPosition;
    private Vector3 lookAheadOffset;

    private void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        lastPlayerPosition = player.position;
    }

    private void LateUpdate()
    {
        if (player == null)
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
}
