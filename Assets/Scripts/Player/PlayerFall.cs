using System.Collections;
using UnityEngine;

public class PlayerFall : MonoBehaviour
{
    [Header("Fall Settings")]
    [SerializeField] private float fallSpeed = 5.0f;
    [SerializeField] private float respawnDelay = 1.0f;

    private CharacterController controller;
    private Transform currentRoomRespawn;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    /// <summary>
    /// Llamado cuando el jugador cae en un agujero.
    /// </summary>
    public void FallIntoHole()
    {
        StartCoroutine(Fall());
    }

    /// <summary>
    /// Asigna el punto de respawn de la habitación actual.
    /// </summary>
    public void SetRoomRespawn(Transform roomSpawnPoint)
    {
        currentRoomRespawn = roomSpawnPoint;
    }

    private IEnumerator Fall()
    {
        PlayerMovement.canMove = false;
        controller.enabled = false;

        float targetY = transform.position.y - 3.0f;
        while (transform.position.y > targetY)
        {
            transform.position += Vector3.down * fallSpeed * Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(respawnDelay);

        if (currentRoomRespawn != null)
        {
            transform.position = currentRoomRespawn.position;
        }

        controller.enabled = true;
        PlayerMovement.canMove = true;
    }
}
