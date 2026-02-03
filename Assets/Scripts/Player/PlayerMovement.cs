using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UI;
using NUnit.Framework;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement: MonoBehaviour
{
    private CharacterController controller;

    private Vector3 direction;
    [SerializeField] private UpgradeStats upgradeStatsScriptable;


    [Header("Movement")]
    [SerializeField] private float baseMoveSpeed = 10.0f;
    [SerializeField] private float rotationSpeed = 10.0f;

    public static bool canMove = true;

    [Header("Rolling")]
    [SerializeField] private float rollSpeed = 20.0f;
    [SerializeField] private float rollDuration = 0.5f;
    [SerializeField] private float rollCooldown = 1.0f;
    [SerializeField] private GameObject destructionCheck;

    private bool isRolling = false;
    private bool canRoll = true;
    public static bool isInvincible = false;
    private float rollTimer;

    private Vector3 rollDirection;

    [Header("Bullet Time")]
    [SerializeField] private BulletTimeTrigger bulletTimeTrigger;

    [SerializeField] private float bulletTimeSpeed;             // 15.0f
    [SerializeField] private float bulletTimeDuration = 1.2f;
    [SerializeField] private float angleAmplitude = 2f;
    [SerializeField] private float frontAmplitude = 5f;
    [SerializeField] private float coneAngle = 35f;
    [SerializeField] private float maxPushDistance = 9f;

    private GameObject toPush;

    public bool canTriggerBulletTime = true;
    bool isInBulletTime = false;

    [Header("Input System")]
    public InputActionAsset inputActions;

    private InputAction moveAction;
    private InputAction rollAction;

    private Vector2 moveAmount;

    [Header("UI")]
    [SerializeField] private Image bulletTimeTimer;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    [Header("SFX")]
    [SerializeField] private AudioManager AM;
    [SerializeField] private AudioSource Main;


    
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
        baseMoveSpeed += upgradeStatsScriptable.speed;
        controller = GetComponent<CharacterController>();
        Debug.Log("Position at Start: " + transform.position);

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
        AM = FindFirstObjectByType<AudioManager>();
        Main=GetComponentInChildren<AudioSource>();
        canMove = true;
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
                    isInBulletTime = true;
                    toPush = bulletTimeTrigger.FirstHarm();
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
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }

    public IEnumerator Roll()
    {
        isRolling = true;
        canRoll = false;
        isInvincible = true;
        AM.Play("Dash", Main);
        destructionCheck.SetActive(true);

        moveAmount = moveAction.ReadValue<Vector2>();
        Vector3 inputDirection = new Vector3(moveAmount.x, 0f, moveAmount.y).normalized;
        animator.SetBool("isRolling", true);

        CameraShake.Shake(0.1f, 0.1f, inputDirection);
        CameraShake.DoDashZoom(-0.5f, 0.5f);

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
        animator.SetBool("isRolling", false);
        destructionCheck.SetActive(false);

        yield return new WaitForSecondsRealtime(rollCooldown);
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
        isInBulletTime = false;
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

    public bool CanPush(GameObject enemyChecking)
    {
        return isRolling && isInBulletTime && enemyChecking == toPush;
    }

    public Vector3 PushTarget()
    {
        Vector3 pushDir = new Vector3(toPush.transform.position.x - transform.position.x,0, toPush.transform.position.z - transform.position.z).normalized * frontAmplitude;
        Collider[] hits = Physics.OverlapSphere(toPush.transform.position, frontAmplitude);
        GameObject nearestTrap = null;
        foreach (Collider hit in hits)
        {
            if (hit.gameObject.CompareTag("Trap"))
            {
                if (nearestTrap == null)
                {
                    if (Vector3.SignedAngle(pushDir, hit.gameObject.transform.position - toPush.transform.position, Vector3.up) <= coneAngle && Vector3.SignedAngle(pushDir, hit.gameObject.transform.position - toPush.transform.position, Vector3.up) > -coneAngle)
                    {
                        nearestTrap = hit.gameObject;
                    }
                }
                else
                {
                    if(Vector3.SignedAngle(pushDir,hit.gameObject.transform.position - toPush.transform.position,Vector3.up) <= coneAngle && Vector3.SignedAngle(pushDir, hit.gameObject.transform.position - toPush.transform.position, Vector3.up) > -coneAngle)
                    {
                        if(Vector3.Distance(toPush.transform.position, hit.gameObject.transform.position) <= maxPushDistance)
                        {
                            if (Vector3.Distance(toPush.transform.position, hit.gameObject.transform.position) < Vector3.Distance(toPush.transform.position, nearestTrap.transform.position))
                            {
                                nearestTrap = hit.gameObject;
                            }
                        }   
                    }
                }
            }
        }

        if (nearestTrap != null)
        { 
            return toPush.transform.position + (nearestTrap.transform.position - toPush.transform.position).normalized * maxPushDistance;
        }
        else
        {
            return toPush.transform.position + pushDir.normalized*maxPushDistance;
        }
    }
    
    void OnDrawGizmos()
    {
        if(isInBulletTime)
        {
            Vector3 pushDir = (toPush.transform.position - transform.position).normalized;

            Vector3 rightAngle = Vector3.RotateTowards(pushDir, new Vector3(pushDir.z, pushDir.y, -pushDir.x), coneAngle,0f);
            Vector3 leftAngle = Vector3.RotateTowards(pushDir, -new Vector3(pushDir.z, pushDir.y, -pushDir.x), coneAngle,0f);
            //Vector3.SignedAngle(); usado para ver angulos entre vectores

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(toPush.transform.position +leftAngle * angleAmplitude + pushDir * frontAmplitude, toPush.transform.position + rightAngle*angleAmplitude + pushDir * frontAmplitude);
            Gizmos.DrawLine(toPush.transform.position + rightAngle * angleAmplitude + pushDir * frontAmplitude, toPush.transform.position);
            Gizmos.DrawLine(toPush.transform.position, toPush.transform.position + leftAngle * angleAmplitude + pushDir * frontAmplitude);
        }
    }
}
