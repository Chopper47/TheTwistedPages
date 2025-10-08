using System.Collections;
using UnityEngine;

public class PlayerFall : MonoBehaviour
{
    [SerializeField] private float fallSpeed = 5.0f;
    [SerializeField] private float respawnDelay = 1.5f;

    private CharacterController controller;
    private Vector3 respawnPosition;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    public void FallIntoHole(Vector3 holePosition)
    {
        respawnPosition = new Vector3(holePosition.x - 2, transform.position.y, holePosition.z + 2);

        StartCoroutine(Fall());
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

        transform.position = respawnPosition;

        controller.enabled = true;
        PlayerMovement.canMove = true;
    }
}
