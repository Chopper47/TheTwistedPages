using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

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
    [SerializeField] private float axeConeAngle = 35f;
    [SerializeField] private float maxPushDistance = 9f;
    [SerializeField] private float axeRotationSpeed = 1.0f;
    [SerializeField] private float axeThrowSpeed = 15.0f;
    [SerializeField] private Transform axeHold;
    private GameObject toPush;

    public bool canTriggerBulletTime = true;
    private Coroutine rotationCoroutine;
    private Coroutine bulletTimeCoroutine;
    private Coroutine btUICoroutine;
    private bool isInBulletTime = false;
    private bool axeHolding = false;

    [Header("Input System")]
    public InputActionAsset inputActions;

    private InputAction moveAction;
    private InputAction rollAction;
    private InputAction throwAxeAction;

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
        throwAxeAction = inputActions.FindActionMap("Player").FindAction("ThrowAxe");
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
            if (!axeHolding)
            {
                Move();
            }
            else
            {
                if (throwAxeAction.triggered)
                {
                    ThrowAxeInput();
                }
            }

            if (rollAction.triggered && canRoll)
            {

                if (bulletTimeTrigger.CanDodge() && canTriggerBulletTime)
                {
                    canTriggerBulletTime = false;
                    isInBulletTime = true;
                    toPush = bulletTimeTrigger.FirstHarm();
                    Axe axe = toPush.GetComponent<Axe>();
                    BossAxe bossAxe = toPush.GetComponent<BossAxe>();
                    if (axe != null || bossAxe != null)
                    {
                        axeHolding = true;
                        if (axe != null) 
                        {
                            axe.StopMoving();
                        }
                        else 
                        { 
                            bossAxe.InterruptedThrowAttack(); 
                        }
                        toPush.transform.parent = axeHold;
                        toPush.transform.localPosition = Vector3.zero;
                        rotationCoroutine = StartCoroutine(AxeRotation());
                    }
                    if(toPush.GetComponent<Projectile>() != null)
                    {
                        toPush.GetComponent<SphereCollider>().radius *= 2f;
                    }
                    TimeManager.SlowTime();
                    btUICoroutine = StartCoroutine(UpdateBulletTimeUI());
                    bulletTimeCoroutine = StartCoroutine(WaitForNormalTime());
                }

                if (!axeHolding && !isInBulletTime)
                {
                    StartCoroutine(Roll());
                }
            }
        }
        else
        {
            controller.Move(rollDirection * rollSpeed * Time.unscaledDeltaTime);
        }
    }
    
    public void ThrowAxeInput()                 //what input system should call for axe throwing
    {
        if(axeHolding)
        {
            StopCoroutine(rotationCoroutine);
            StopCoroutine(btUICoroutine);
            StopCoroutine(bulletTimeCoroutine);
            EndBulletTime();
            bulletTimeTimer.fillAmount = 0f;
            ReleaseAxe();
        }
    }

    public void ReleaseAxe()
    {
        toPush.transform.parent = null;
        Axe axe = toPush.GetComponent<Axe>();
        BossAxe bossAxe = toPush.GetComponent<BossAxe>();
        Vector3 newDir = new Vector3(toPush.transform.position.x - transform.position.x, 0, toPush.transform.position.z - transform.position.z).normalized;
        if(axe != null)
        {
            axe.NewDirection(PushTarget(false, true).normalized * axeThrowSpeed);
            axe.ActivateTrail();
        }
        else
        {
            Lumberjack boss = FindAnyObjectByType<Lumberjack>();

            if(Vector3.SignedAngle(newDir, boss.gameObject.transform.position - transform.position, Vector3.up) <= axeConeAngle && Vector3.SignedAngle(newDir, boss.gameObject.transform.position - transform.position, Vector3.up) > -axeConeAngle)
            {
                Debug.LogWarning("found boss within range, sent to him");
                bossAxe.NewDirection(transform.position + Vector3.up * 0.1f, new Vector3(boss.gameObject.transform.position.x - transform.position.x, 0f, boss.gameObject.transform.position.z - transform.position.z).normalized ,.2f);
            }
            else 
            {
                Debug.LogWarning("not even close bruh");
                bossAxe.NewDirection(transform.position + Vector3.up * 0.1f, newDir, .2f);
            }
        }
    }

    private IEnumerator AxeRotation()
    {
        float timer = 0f;

        while (timer < bulletTimeDuration)
        {
            transform.Rotate(Vector3.up, axeRotationSpeed);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        yield return null;
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

        //CameraShake.Shake(0.1f, 0.1f, inputDirection);
        //CameraShake.DoDashZoom(-0.5f, 0.5f);

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

        if(toPush.GetComponent<Axe>() || toPush.GetComponent<BossAxe>())
        {
            ReleaseAxe();
        }

        EndBulletTime();
    }

    public void EndBulletTime()
    {
        TimeManager.ReturnTime();
        axeHolding = false;
        canTriggerBulletTime = true;
        isInBulletTime = false;
    }

    public IEnumerator UpdateBulletTimeUI()
    {
        float time = 0f;
        if(bulletTimeTimer) bulletTimeTimer.fillAmount = 1.0f;

        while (time < bulletTimeDuration)
        {
            bulletTimeTimer.fillAmount = Mathf.Lerp(1.0f, 0f, time/bulletTimeDuration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        if (bulletTimeTimer) bulletTimeTimer.fillAmount = 0f;
        yield return null;
    }

    public bool CanPush(GameObject enemyChecking)
    {
        return isRolling && isInBulletTime && enemyChecking == toPush;
    }

    /// <summary>
    /// Method used for getting lerp direction of enemy pushing, projectiles and axe throws
    /// </summary>
    /// <param name="findTrap">Is the method finding the direction of a trap</param>
    /// <param name="axeTarget">Is the method using the max angle for an axe throw</param>
    /// <returns>Returns max position of the target</returns>
    public Vector3 PushTarget(bool findTrap, bool axeTarget)
    {
        Vector3 pushDir = new Vector3(toPush.transform.position.x - transform.position.x,0, toPush.transform.position.z - transform.position.z).normalized * frontAmplitude;
        Collider[] hits = Physics.OverlapSphere(toPush.transform.position, frontAmplitude);
        GameObject nearestTrap = null;

        foreach (Collider hit in hits)
        {
            if (findTrap ? hit.gameObject.CompareTag("Trap") : hit.gameObject.CompareTag("Enemy"))
            {
                if (nearestTrap == null)
                {
                    if (Vector3.SignedAngle(pushDir, hit.gameObject.transform.position - toPush.transform.position, Vector3.up) <= (axeTarget? axeConeAngle:coneAngle) && Vector3.SignedAngle(pushDir, hit.gameObject.transform.position - toPush.transform.position, Vector3.up) > (axeTarget ? -axeConeAngle : -coneAngle))
                    {
                        nearestTrap = hit.gameObject;
                    }
                }
                else
                {
                    if(Vector3.SignedAngle(pushDir,hit.gameObject.transform.position - toPush.transform.position,Vector3.up) <= (axeTarget ? axeConeAngle : coneAngle) && Vector3.SignedAngle(pushDir, hit.gameObject.transform.position - toPush.transform.position, Vector3.up) > (axeTarget ? -axeConeAngle : -coneAngle))
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
    
    //void OnDrawGizmos()
    //{
    //    if(isInBulletTime)
    //    {
    //        Vector3 pushDir = (toPush.transform.position - transform.position).normalized;

    //        Vector3 rightAngle = Vector3.RotateTowards(pushDir, new Vector3(pushDir.z, pushDir.y, -pushDir.x), coneAngle,0f);
    //        Vector3 leftAngle = Vector3.RotateTowards(pushDir, -new Vector3(pushDir.z, pushDir.y, -pushDir.x), coneAngle,0f);
    //        //Vector3.SignedAngle(); usado para ver angulos entre vectores

    //        Gizmos.color = Color.yellow;
    //        Gizmos.DrawLine(toPush.transform.position +leftAngle * angleAmplitude + pushDir * frontAmplitude, toPush.transform.position + rightAngle*angleAmplitude + pushDir * frontAmplitude);
    //        Gizmos.DrawLine(toPush.transform.position + rightAngle * angleAmplitude + pushDir * frontAmplitude, toPush.transform.position);
    //        Gizmos.DrawLine(toPush.transform.position, toPush.transform.position + leftAngle * angleAmplitude + pushDir * frontAmplitude);
    //    }
    //}
}
