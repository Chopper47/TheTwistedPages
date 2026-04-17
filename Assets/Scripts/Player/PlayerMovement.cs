using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Rendering;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    private CharacterController controller;
    [SerializeField] private Animator animator;
    private CameraController cameraController;
    [SerializeField] private Volume blackAndWhiteFilter;
    [SerializeField] private BulletTimeTrigger bulletTimeTrigger;
    [SerializeField] private UpgradeStats upgradeStatsScriptable;
    private PlayerUpgrades playerUpgrades;

    [Header("Movement")]
    [SerializeField] private float baseMoveSpeed = 10.0f;
    [SerializeField] private float rotationSpeed = 10.0f;

    private float speedMultiplier = 1.0f;

    private Vector3 direction;
    private Vector2 moveAmount;

    public static bool canMove = true;

    [Header("Roll")]
    [SerializeField] private float rollSpeed = 20.0f;
    [SerializeField] private float rollDuration = 0.5f;
    [SerializeField] private float rollCooldown = 1.0f;
    [SerializeField] private GameObject destructionCheck;

    public bool isRolling = false;
    public bool canRoll = true;
    public static bool isInvincible = false;

    private Vector3 rollDirection;

    [Header("Bullet Time")]
    [SerializeField] private float bulletTimeDuration = 1.2f;

    [SerializeField] private float coneAngle = 35.0f;
    [SerializeField] private float axeConeAngle = 35.0f;

    [SerializeField] private float frontAmplitude = 5.0f;
    [SerializeField] private float angleAmplitude = 2.0f;
    [SerializeField] private float maxPushDistance = 9.0f;

    private bool isInBulletTime = false;
    public bool canTriggerBulletTime = true;
    private bool firstDash = false;

    private Coroutine bulletTimeCoroutine;
    private Coroutine btUICoroutine;

    [Header("Axe")]
    [SerializeField] private Transform axeHold;
    [SerializeField] private float axeRotationSpeed = 1.0f;
    [SerializeField] private float axeThrowSpeed = 15.0f;
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
    private InputAction useUpgradeAction;

    [Header("UI")]
    [SerializeField] private Image bulletTimeTimer;

    [Header("SFX")]
    private AudioManager AM;
    private AudioSource Main;

    [Header("VFX")]
    [SerializeField] private ParticleSystem walkParticles;
    [SerializeField] private ParticleSystem dashParticles;

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
        playerUpgrades = GetComponent<PlayerUpgrades>();

        playerMap = inputActions.FindActionMap("Player");

        moveAction = playerMap.FindAction("Move");
        rollAction = playerMap.FindAction("Roll");
        throwAxeAction = playerMap.FindAction("ThrowAxe");
        useUpgradeAction = playerMap.FindAction("UseUpgrade");

        SetupRangeRenderers();
    }

    private void Start()
    {
        baseMoveSpeed += upgradeStatsScriptable.speed;

        cameraController = FindAnyObjectByType<CameraController>();
        AM = FindFirstObjectByType<AudioManager>();
        Main = GetComponentInChildren<AudioSource>();

        if(blackAndWhiteFilter == null)
        {
            Debug.LogWarning("Black and white filter not assigned.");
        }

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
        HandleUpgradeInput();
    }

    #region Input
    private void HandleMovement()
    {
        if (axeHolding)
        {
            return;
        }

        moveAmount = moveAction.ReadValue<Vector2>();

        direction = new Vector3(moveAmount.x, 0f, moveAmount.y);

        if (direction.sqrMagnitude > 0.01f && canMove)
        {
            direction.Normalize();

            RotatePlayer(direction);

            controller.Move(direction * baseMoveSpeed * speedMultiplier * Time.unscaledDeltaTime);

            animator.SetBool("isRunning", true);

            if (walkParticles != null && !walkParticles.isPlaying)
            {
                walkParticles.Play();
            }
        }
        else
        {
            animator.SetBool("isRunning", false);

            if (walkParticles != null && walkParticles.isPlaying)
            {
                walkParticles.Stop();
            }
        }
    }

    private void HandleRollInput()
    {
        if (!rollAction.triggered || !canRoll)
        {
            return;
        }

        if (playerUpgrades.seedBasketActive)
        {
            return;
        }

        TryTriggerBulletTime();

        HandleRollState();
    }

    private void HandleUpgradeInput()
    {
        if (!useUpgradeAction.triggered)
        {
            return;
        }

        if (isRolling || isInBulletTime || axeHolding)
        {
            return;
        }

        playerUpgrades.UseActiveUpgrade();
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
            Time.unscaledDeltaTime * rotationSpeed);
    }

    public void SetSlowMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;

        if (animator != null)
        {
            animator.speed = multiplier;
        }
    }
    #endregion

    #region Roll
    private void HandleRollState()
    {
        if (!axeHolding && !isInBulletTime && !throwAxe)
        {
            firstDash = false;
            StartCoroutine(Roll());
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
                StartCoroutine(Roll());
            }
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

        if (dashParticles != null)
        {
            dashParticles.Play();
        }

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

        if (playerUpgrades.activeUpgrade != null && playerUpgrades.activeUpgrade.behaviour is DashExplosionUpgrade dashExplosion)
        {
            dashExplosion.TriggerExplosion(transform.position, playerUpgrades);
        }

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

        //Debug.LogError("Can Dodge? " + bulletTimeTrigger.CanDodge() + " | Can Trigger? " + canTriggerBulletTime + " | First Harm? " + bulletTimeTrigger.FirstHarm());

        bulletTimeTrigger.DeactivateDodge();

        PlayAxeHoldingAnimation();

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

            Animator axeAnimator = toPush.GetComponent<Animator>();
            axeAnimator.SetBool("InPlayerHands", true);

            toPush.transform.parent = axeHold;
            toPush.transform.localPosition = new Vector3(-0.05f, 0f, 0.3f);
            Quaternion toPushRotation = Quaternion.identity;
            toPushRotation = Quaternion.Euler(0, 90.0f, 90.0f);
            toPush.transform.localRotation = toPushRotation;

            rotationCoroutine = StartCoroutine(AxeRotation());
        }

        if (toPush.GetComponent<Projectile>() != null)
        {
            toPush.GetComponent<Projectile>().IgnorePlayer();
            float distance = Vector3.Distance(transform.position, new Vector3(toPush.transform.position.x, transform.position.y, toPush.transform.position.z));
            SphereCollider proyectile = toPush.GetComponent<SphereCollider>();
            if (distance > proyectile.radius * 2.5f)
            {
                Debug.LogError("max radius");
                proyectile.radius *= 2.5f;
            }
            else
            {
                if (distance <= 0.25f)
                {
                    Debug.LogError("reduced radius minimal");
                    proyectile.radius = 0.25f;
                }
                else
                {
                    Debug.LogError("reduced radius scalar");
                    proyectile.radius = (distance / 2) - .1f;
                }
            }
        }
        else
        {
            if(toPush.GetComponent<EnemyHitbox>() != null)
            {
                toPush.GetComponent<EnemyHitbox>().SetCurrentlyPushing(true);
            }
        }

            TimeManager.SlowTime();

        if (bulletTimeTimer != null)
        {
            //Debug.LogWarning("Start bullet time");
            btUICoroutine = StartCoroutine(UpdateBulletTimeUI());
        }

        bulletTimeCoroutine = StartCoroutine(WaitForNormalTime());

        if(blackAndWhiteFilter != null)
        {
            blackAndWhiteFilter.enabled = true;
        }

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
        //Debug.Log("Bullet Time stopped.");
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
        if(blackAndWhiteFilter != null)
        {
            blackAndWhiteFilter.enabled = false;
        }
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

        if(toPush && toPush.GetComponent<EnemyHitbox>() != null)
        {
            toPush.GetComponent<EnemyHitbox>().SetCurrentlyPushing(true);
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

        bulletTimeCoroutine = null;
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

        if(blackAndWhiteFilter != null)
        {
            blackAndWhiteFilter.enabled = false;
        }

        if (leftRangeRender && rightRangeRender)
        {
            ShowCone(false);
        }
    }

    public void ReleaseAxe()
    {
        StopAxeHoldingAnimation();
        Animator animator = toPush.GetComponent<Animator>();
        animator.SetBool("InPlayerHands", false);

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
    public Vector3 GetDirection()
    {
        return direction;
    }

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
        //Debug.Log("Can push " + enemyChecking + "? - " + isRolling + "|" + isInBulletTime + "|" + (enemyChecking == toPush));
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

    #region Animation
    public void PlayPlaceAnimation()
    {
        canMove = false;
        canRoll = false;

        animator.SetTrigger("Place");

        StartCoroutine(UnlockMovementAfterPlace());
    }

    private IEnumerator UnlockMovementAfterPlace()
    {
        yield return new WaitForSeconds(0.8f);

        canMove = true;
        canRoll = true;
    }

    public void PlayThrowingSeedsAnimation()
    {
        animator.SetBool("IsThrowingSeeds", true);
    }

    public void StopThrowingSeedsAnimation()
    {
        animator.SetBool("IsThrowingSeeds", false);
    }

    public void PlayAxeHoldingAnimation()
    {
        animator.SetBool("HasAxe", true);
    }

    public void StopAxeHoldingAnimation()
    {
        animator.SetBool("HasAxe", false);
    }
    #endregion
}
