using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    private CharacterController controller;
    [SerializeField] private Animator animator;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private Volume blackAndWhiteFilter;
    [SerializeField] private BulletTimeTrigger bulletTimeTrigger;
    [SerializeField] private UpgradeStats upgradeStatsScriptable;

    [Header("Movement")]
    [SerializeField] private float baseMoveSpeed = 10.0f;
    [SerializeField] private float rotationSpeed = 10.0f;

    private Vector3 direction;
    private Vector2 moveAmount;

    public static bool canMove = true;

    [Header("Roll")]
    [SerializeField] private float rollSpeed = 20.0f;
    [SerializeField] private float rollDuration = 0.5f;
    [SerializeField] private float rollCooldown = 1.0f;
    [SerializeField] private GameObject destructionCheck;

    private bool isRolling = false;
    public bool canRoll = true;
    public static bool isInvincible = false;

    private Vector3 rollDirection;
    private Coroutine rollCoroutine;

    [Header("Bullet Time")]
    [SerializeField] private float bulletTimeDuration = 1.2f;

    [SerializeField] private float coneAngle = 35f;
    [SerializeField] private float axeConeAngle = 35f;

    [SerializeField] private float frontAmplitude = 5f;
    [SerializeField] private float angleAmplitude = 2f;
    [SerializeField] private float maxPushDistance = 9f;

    private bool isInBulletTime = false;
    public bool canTriggerBulletTime = true;
    private bool firstDash = false;

    private Coroutine bulletTimeCoroutine;
    private Coroutine btUICoroutine;

    [Header("Axe")]
    [SerializeField] private Transform axeHold;
    [SerializeField] private float axeRotationSpeed = 1f;
    [SerializeField] private float axeThrowSpeed = 15f;
    [SerializeField] private float bossAxeThrowSpeed = 0.2f;

    private bool axeHolding = false;
    private bool throwAxe = false;

    private GameObject toPush;
    private Coroutine rotationCoroutine;

    [Header("Range Visual")]
    [SerializeField] private GameObject LeftRangeGO;
    [SerializeField] private GameObject RightRangeGO;

    private LineRenderer leftRangeRender;
    private LineRenderer rightRangeRender;

    [Header("Input System")]
    [SerializeField] public InputActionAsset inputActions;
    private InputActionMap playerMap;

    private InputAction moveAction;
    private InputAction rollAction;
    private InputAction throwAxeAction;

    [Header("UI")]
    [SerializeField] private Image bulletTimeTimer;

    [Header("SFX")]
    [SerializeField] private AudioManager AM;
    [SerializeField] private AudioSource Main;

    private void OnEnable()
    {
        playerMap.Enable();
    }

    private void OnDisable()
    {
        playerMap.Disable();
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        playerMap = inputActions.FindActionMap("Player");

        moveAction = playerMap.FindAction("Move");
        rollAction = playerMap.FindAction("Roll");
        throwAxeAction = playerMap.FindAction("ThrowAxe");

        SetupRangeRenderers();
    }

    private void Start()
    {
        baseMoveSpeed += upgradeStatsScriptable.speed;

        cameraController = FindAnyObjectByType<CameraController>();
        AM = FindFirstObjectByType<AudioManager>();
        Main = GetComponentInChildren<AudioSource>();

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        canMove = true;
    }

    private void Update()
    {
        if (!controller.enabled)
        {
            return;
        }

        if (isRolling)
        {
            HandleRollingMovement();
            return;
        }

        HandleMovement();
        HandleAxeInput();
        HandleRollInput();
    }

    #region Input
    private void HandleMovement()
    {
        if (axeHolding) return;

        moveAmount = moveAction.ReadValue<Vector2>();

        direction = new Vector3(moveAmount.x, 0f, moveAmount.y);

        if (direction.sqrMagnitude > 0.01f && canMove)
        {
            direction.Normalize();

            RotatePlayer(direction);

            controller.Move(direction * baseMoveSpeed * Time.unscaledDeltaTime);

            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }

    private void HandleRollInput()
    {
        if (!rollAction.triggered || !canRoll)
        {
            return;
        }

        TryTriggerBulletTime();

        HandleRollState();
    }

    private void HandleAxeInput()
    {
        if (!axeHolding)
        {
            return;
        }

        if (throwAxeAction.triggered)
        {
            ThrowAxeInput();
        }
    }
    #endregion

    #region Movement
    private void HandleRollingMovement()
    {
        if (!firstDash)
        {
            controller.Move(rollDirection * rollSpeed * Time.unscaledDeltaTime);
        }
    }

    private void RotatePlayer(Vector3 dir)
    {
        Quaternion targetRotation = Quaternion.LookRotation(dir);

        controller.transform.rotation = Quaternion.Slerp(
            controller.transform.rotation,
            targetRotation,
            Time.unscaledDeltaTime * rotationSpeed
        );
    }
    #endregion

    #region Roll
    private void HandleRollState()
    {
        if (!axeHolding && !isInBulletTime && !throwAxe)
        {
            firstDash = false;
            rollCoroutine = StartCoroutine(Roll());
            Debug.Log("start rolling");
        }
        else if (!axeHolding && isInBulletTime && !throwAxe)
        {
            if (!firstDash)
            {
                firstDash = true;
            }
            else
            {
                firstDash = false;
                rollCoroutine = StartCoroutine(Roll());
            }

            Debug.Log("start rolling on bullet time");
        }

        if (throwAxe)
        {
            throwAxe = false;
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

        if (inputDirection.sqrMagnitude > 0.01f)
        {
            rollDirection = inputDirection.normalized;
        }
        else
        {
            rollDirection = transform.forward;
        }

        transform.rotation = Quaternion.LookRotation(rollDirection);

        animator.SetBool("isRolling", true);

        CameraShake.Shake(0.1f, 0.1f, inputDirection);
        CameraShake.DoDashZoom(-0.5f, 0.5f);

        yield return new WaitForSecondsRealtime(rollDuration);

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
    #endregion

    #region Bullet Time
    private void TryTriggerBulletTime()
    {
        if (!bulletTimeTrigger.CanDodge() || !canTriggerBulletTime || bulletTimeTrigger.FirstHarm() == null)
        {
            return;
        }

        canTriggerBulletTime = false;
        isInBulletTime = true;

        toPush = bulletTimeTrigger.FirstHarm();

        bulletTimeTrigger.gameObject.SetActive(false);

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

        if (toPush.GetComponent<Projectile>() != null)
        {
            toPush.GetComponent<SphereCollider>().radius *= 2f;
        }

        TimeManager.SlowTime();

        if (bulletTimeTimer != null)
        {
            btUICoroutine = StartCoroutine(UpdateBulletTimeUI());
        }

        bulletTimeCoroutine = StartCoroutine(WaitForNormalTime());

        blackAndWhiteFilter.enabled = true;

        if (leftRangeRender && rightRangeRender)
        {
            ShowCone(true);
        }
    }

    /// <summary>
    /// Method used for getting lerp direction of enemy pushing, projectiles and axe throws
    /// </summary>
    /// <param name="findTrap">Is the method finding the direction of a trap</param>
    /// <param name="axeTarget">Is the method using the max angle for an axe throw</param>
    /// <returns>Returns max position of the target</returns>
    public Vector3 PushTarget(bool findTrap, bool axeTarget)
    {
        Vector3 origin = toPush.transform.position;

        Vector3 pushDir = FlatDirection(transform.position, origin) * frontAmplitude;

        float maxAngle = axeTarget ? axeConeAngle : coneAngle;

        Collider[] hits = Physics.OverlapSphere(origin, frontAmplitude);

        GameObject bestTarget = null;
        float bestDistance = float.MaxValue;

        foreach (Collider hit in hits)
        {
            if (!IsValidTarget(hit.gameObject, findTrap))
            {
                continue;
            }

            Vector3 dirToTarget = hit.transform.position - origin;
            dirToTarget.y = 0;

            float angle = Vector3.SignedAngle(pushDir, dirToTarget, Vector3.up);

            if (angle < -maxAngle || angle > maxAngle)
            {
                continue;
            }

            float dist = dirToTarget.sqrMagnitude;

            if (dist > maxPushDistance * maxPushDistance)
            {
                continue;
            }

            if (dist < bestDistance)
            {
                bestDistance = dist;
                bestTarget = hit.gameObject;
            }
        }

        if (bestTarget != null)
        {
            Vector3 dir = (bestTarget.transform.position - origin).normalized;
            return origin + dir * maxPushDistance;
        }

        return origin + pushDir.normalized * maxPushDistance;
    }

    public void StopBulletTime()
    {
        Debug.Log("Bullet Time stopped.");
        if (bulletTimeCoroutine != null)
        {
            StopCoroutine(bulletTimeCoroutine);
        }

        if (btUICoroutine != null)
        {
            StopCoroutine(btUICoroutine);
        }

        if (leftRangeRender && rightRangeRender)
        {
            ShowCone(false);
        }

        toPush = null;
        EndBulletTime();
    }

    public void EndBulletTime()
    {
        blackAndWhiteFilter.enabled = false;
        bulletTimeTrigger.gameObject.SetActive(true);
        TimeManager.ReturnTime();
        axeHolding = false;
        canTriggerBulletTime = true;
        isInBulletTime = false;
    }

    private void FinishBulletTime()
    {
        if (toPush && (toPush.GetComponent<Axe>() || toPush.GetComponent<BossAxe>()))
        {
            ReleaseAxe();
            throwAxe = false;
        }

        if (leftRangeRender && rightRangeRender)
        {
            ShowCone(false);
        }

        EndBulletTime();
    }

    public IEnumerator WaitForNormalTime()
    {
        float timer = 0f;

        while (timer < bulletTimeDuration)
        {
            timer += Time.unscaledDeltaTime;

            UpdateBulletTimeVisuals();

            yield return null;
        }

        FinishBulletTime();
    }

    public IEnumerator UpdateBulletTimeUI()
    {
        float time = 0f;

        if (bulletTimeTimer)
        {
            bulletTimeTimer.fillAmount = 1.0f;
        }

        while (time < bulletTimeDuration)
        {
            bulletTimeTimer.fillAmount = Mathf.Lerp(1.0f, 0f, time / bulletTimeDuration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        if (bulletTimeTimer)
        {
            bulletTimeTimer.fillAmount = 0f;
        }

        yield return null;
    }

    private void UpdateBulletTimeVisuals()
    {
        if (leftRangeRender && rightRangeRender && toPush)
        {
            Vector3 dir = FlatDirection(transform.position, toPush.transform.position);

            DrawRange(toPush.transform.position, dir, maxPushDistance, axeHolding);
        }

        if (cameraController != null)
        {
            cameraController.UpdateLookAhead(0.02f);
            cameraController.UpdateCameraFollow(0.02f);
        }
    }
    #endregion

    #region Axe
    public void ThrowAxeInput()
    {
        if (!axeHolding)
        {
            return;
        }

        StopBulletTimeCoroutines();

        EndBulletTime();

        if (bulletTimeTimer)
        {
            bulletTimeTimer.fillAmount = 0f;
        }

        ReleaseAxe();

        throwAxe = true;

        blackAndWhiteFilter.enabled = false;

        if (leftRangeRender && rightRangeRender)
        {
            ShowCone(false);
        }
    }
    public void ReleaseAxe()
    {
        toPush.transform.parent = null;
        Axe axe = toPush.GetComponent<Axe>();
        BossAxe bossAxe = toPush.GetComponent<BossAxe>();
        Vector3 newDir = new Vector3(toPush.transform.position.x - transform.position.x, 0, toPush.transform.position.z - transform.position.z).normalized;
        if (axe != null)
        {
            axe.NewDirection(PushTarget(false, true).normalized * axeThrowSpeed);
            axe.ActivateTrail();
        }
        else
        {
            Lumberjack boss = FindAnyObjectByType<Lumberjack>();

            if (Vector3.SignedAngle(newDir, boss.gameObject.transform.position - transform.position, Vector3.up) <= axeConeAngle && Vector3.SignedAngle(newDir, boss.gameObject.transform.position - transform.position, Vector3.up) > -axeConeAngle)
            {
                Debug.LogWarning("found boss within range, sent to him");
                bossAxe.NewDirection(transform.position + Vector3.up * 0.1f, new Vector3(boss.gameObject.transform.position.x - transform.position.x, 0f, boss.gameObject.transform.position.z - transform.position.z).normalized, bossAxeThrowSpeed);
            }
            else
            {
                Debug.LogWarning("not even close bruh");
                bossAxe.NewDirection(transform.position + Vector3.up * 0.1f, newDir, bossAxeThrowSpeed);
            }
        }
        toPush = null;
    }

    private IEnumerator AxeRotation()
    {
        float timer = 0f;

        while (timer < bulletTimeDuration)
        {
            transform.Rotate(Vector3.up, Time.unscaledDeltaTime * axeRotationSpeed);
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        yield return null;
    }
    #endregion

    #region Range Visual
    private void SetupRangeRenderers()
    {
        if (LeftRangeGO && RightRangeGO)
        {
            leftRangeRender = CreateLine(LeftRangeGO);
            rightRangeRender = CreateLine(RightRangeGO);
            ShowCone(false);
        }
        else
        {
            Debug.LogWarning("Range renderers not assigned.");
        }
    }

    public LineRenderer CreateLine(GameObject reference)
    {
        LineRenderer lr = reference.GetComponent<LineRenderer>();

        lr.positionCount = 2;
        lr.useWorldSpace = true;

        lr.startWidth = 0.01f;
        lr.endWidth = 0.05f;

        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = Color.coral;
        lr.endColor = Color.red;

        return lr;
    }

    public void ShowCone(bool show)
    {
        leftRangeRender.enabled = show;
        rightRangeRender.enabled = show;
    }

    public void DrawRange(Vector3 origin, Vector3 forward, float length, bool axeHolding)
    {
        Vector3 leftDir = Quaternion.AngleAxis(axeHolding ? -axeConeAngle : -coneAngle, Vector3.up) * forward;
        Vector3 rightDir = Quaternion.AngleAxis(axeHolding ? axeConeAngle : coneAngle, Vector3.up) * forward;

        leftRangeRender.SetPosition(0, origin);
        leftRangeRender.SetPosition(1, origin + leftDir.normalized * length);

        rightRangeRender.SetPosition(0, origin);
        rightRangeRender.SetPosition(1, origin + rightDir.normalized * length);
    }
    #endregion

    #region Utilities
    private Vector3 FlatDirection(Vector3 from, Vector3 to)
    {
        Vector3 direction = to - from;
        direction.y = 0;
        return direction.normalized;
    }

    private bool IsValidTarget(GameObject obj, bool findTrap)
    {
        if (findTrap)
        {
            return obj.CompareTag("Trap");
        }

        return obj.CompareTag("Enemy");
    }

    public bool CanPush(GameObject enemyChecking)
    {
        Debug.Log("Can push " + enemyChecking + "? - " + isRolling + "|" + isInBulletTime + "|" + (enemyChecking == toPush));
        return isRolling && isInBulletTime && enemyChecking == toPush;
    }

    private void StopBulletTimeCoroutines()
    {
        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
        }

        if (btUICoroutine != null)
        {
            StopCoroutine(btUICoroutine);
        }

        if (bulletTimeCoroutine != null)
        {
            StopCoroutine(bulletTimeCoroutine);
        }
    }
    #endregion
}
