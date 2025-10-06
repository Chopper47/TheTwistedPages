using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using JetBrains.Annotations;
using Unity.VisualScripting;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement: MonoBehaviour
{
    private CharacterController controller;

    private Vector3 direction;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 10.0f;
    [SerializeField] private float bulletTimeSpeed = 15.0f;
    [SerializeField] private float rotationSpeed = 10.0f;

    [SerializeField] private float slowTimeDuration = 1.2f;

    public bool canMove = true;
    public bool canBullet = true;

    [Header("Rolling")]
    
    [SerializeField] PerfectDodgeTrigger dodgeTrigger;

    [SerializeField] private float rollSpeed = 20.0f;
    [SerializeField] private float rollDuration = 0.5f;
    [SerializeField] private float rollCooldown = 1.0f;

    private bool isRolling = false;
    private bool canRoll = true;
    private bool isInvincible = false;
    private float rollTimer;

    private Vector3 rollDirection;

    [Header("Input System")]
    public InputActionAsset inputActions;

    private InputAction moveAction;
    private InputAction rollAction;

    private Vector2 moveAmount;

    [Header("Prototype Reference")]

    [SerializeField] Image slowTimer;

    private void OnEnable()
    {
        inputActions.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        inputActions.FindActionMap("Player").Disable();
    }

    private void Awake()
    {
        moveAction = inputActions.FindActionMap("Player").FindAction("Move");
        rollAction = inputActions.FindActionMap("Player").FindAction("Roll");
    }

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }
    
    private void Update()
    {

        if (!controller.enabled)
        {
            return;
        }

        if (!isRolling)
        {
            Move();

            if (rollAction.triggered && canRoll)
            {
                if (dodgeTrigger.CanDodge() && canBullet)
                {
                    canBullet = false;
                    TimeManager.SlowTime();
                    StartCoroutine(Timer());
                    StartCoroutine(WaitForNormalTime());
                }
                StartCoroutine(Roll());
            }
        }
        else
        {
            controller.Move(rollDirection * rollSpeed * Time.unscaledDeltaTime);
        }
    }

    private void Move()
    {
        moveAmount = moveAction.ReadValue<Vector2>();

        direction = new Vector3(moveAmount.x, 0f, moveAmount.y).normalized;

        if (direction.magnitude >= 0.1f && canMove)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            controller.transform.rotation = Quaternion.Slerp(controller.transform.rotation, targetRotation, Time.unscaledDeltaTime * rotationSpeed);

            controller.Move(direction * moveSpeed * Time.unscaledDeltaTime);
        }
    }

    private IEnumerator Roll()
    {
        isRolling = true;
        canRoll = false;
        isInvincible = true;

        moveAmount = moveAction.ReadValue<Vector2>();
        Vector3 inputDirection = new Vector3(moveAmount.x, 0f, moveAmount.y).normalized;

        if (inputDirection.sqrMagnitude > 0.01f)
        {
            rollDirection = inputDirection;
        }
        else
        {
            rollDirection = transform.forward;
        }

        transform.rotation = Quaternion.LookRotation(rollDirection);

        rollTimer = 0f;

        while (rollTimer < rollDuration)
        {
            rollTimer += Time.unscaledDeltaTime;
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

    public IEnumerator WaitForNormalTime()
    {
        yield return new WaitForSecondsRealtime(slowTimeDuration);

        TimeManager.ReturnTime();
        canBullet = true;
    }

    public IEnumerator Timer()
    {
        float time = 0f;
        slowTimer.fillAmount = 1f;

        while (time < slowTimeDuration)
        {
            slowTimer.fillAmount = Mathf.Lerp(1f,0f, time/slowTimeDuration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        slowTimer.fillAmount = 0f;
        yield return null;
    }
}
