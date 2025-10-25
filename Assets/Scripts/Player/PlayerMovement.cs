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
    [SerializeField] private float baseMoveSpeed = 10.0f;
    [SerializeField] private float rotationSpeed = 10.0f;

    public static bool canMove = true;

    [Header("Rolling")]
    [SerializeField] private float rollSpeed = 20.0f;
    [SerializeField] private float rollDuration = 0.5f;
    [SerializeField] private float rollCooldown = 1.0f;

    private bool isRolling = false;
    private bool canRoll = true;
    static bool isInvincible = false;
    private float rollTimer;

    private Vector3 rollDirection;

    [Header("Bullet Time")]
    [SerializeField] private BulletTimeTrigger bulletTimeTrigger;

    [SerializeField] private float bulletTimeSpeed = 15.0f;
    [SerializeField] private float bulletTimeDuration = 1.2f;

    public bool canTriggerBulletTime = true;

    [Header("Input System")]
    public InputActionAsset inputActions;

    private InputAction moveAction;
    private InputAction rollAction;

    private Vector2 moveAmount;

    [Header("UI")]
    [SerializeField] private Image bulletTimeTimer;

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
                if (bulletTimeTrigger.CanDodge() && canTriggerBulletTime)
                {
                    canTriggerBulletTime = false;
                    TimeManager.SlowTime();
                    StartCoroutine(UpdateBulletTimeUI());
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

            controller.Move(direction * baseMoveSpeed * Time.unscaledDeltaTime);
        }
    }

    public IEnumerator Roll()
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

    public static bool IsInvincible()
    {
        return isInvincible;
    }

    public IEnumerator WaitForNormalTime()
    {
        yield return new WaitForSecondsRealtime(bulletTimeDuration);

        TimeManager.ReturnTime();
        canTriggerBulletTime = true;
    }

    public IEnumerator UpdateBulletTimeUI()
    {
        float time = 0f;
        bulletTimeTimer.fillAmount = 1.0f;

        while (time < bulletTimeDuration)
        {
            bulletTimeTimer.fillAmount = Mathf.Lerp(1.0f, 0f, time/bulletTimeDuration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        bulletTimeTimer.fillAmount = 0f;
        yield return null;
    }
}
