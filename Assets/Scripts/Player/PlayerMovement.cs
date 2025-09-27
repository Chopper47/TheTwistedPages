using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement: MonoBehaviour
{
    private CharacterController controller;

    private Vector3 direction;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private float rotationSpeed = 10.0f;

    [Header("Rolling")]
    [SerializeField] private float rollSpeed = 15.0f;
    [SerializeField] private float rollDuration = 0.5f;
    [SerializeField] private float rollCooldown = 1.0f;

    private bool isRolling = false;
    private bool canRoll = true;
    private bool isInvincible = false;
    private float rollTimer;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }
    
    private void Update()
    {
        if (!isRolling)
        {
            Move();

            if (Input.GetKeyDown(KeyCode.LeftShift) && canRoll)
            {
                StartCoroutine(Roll());
            }
        }
        else
        {
            controller.Move(transform.forward * rollSpeed * Time.deltaTime);
        }
    }

    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            controller.transform.rotation = Quaternion.Slerp(controller.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            
            controller.Move(direction * moveSpeed * Time.deltaTime);
        }
    }

    private IEnumerator Roll()
    {
        isRolling = true;
        canRoll = false;
        isInvincible = true;

        rollTimer = 0f;

        while (rollTimer < rollDuration)
        {
            rollTimer += Time.deltaTime;
            yield return null;
        }

        isRolling = false;
        isInvincible = false;

        yield return new WaitForSeconds(rollCooldown);
        canRoll = true;
    }

    public bool IsInvincible()
    {
        return isInvincible;
    }
}
