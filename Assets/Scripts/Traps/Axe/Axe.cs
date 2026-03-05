using System.Collections;
using UnityEngine;

public class Axe : Trap
{
    [Header("Timing & Movement")]
    [SerializeField] private float showTime = 0.4f;
    [SerializeField] private LayerMask wall;

    private Vector3 direction = Vector3.zero;
    private Vector3 startingPoint = Vector3.zero;
    private Vector3 oppositeWallPoint = Vector3.zero;

    private bool move = false;
    private bool grabbed = false;
    private bool thrown = false;
    private bool ignorePlayer = false;

    [Header("Visuals")]
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private GameObject trajectory;

    [Header("SFX")]
    [SerializeField] private AudioManager AM;
    public AudioSource Main;

    private void Awake()
    {
        transform.Rotate(Vector3.forward, -90.0f);
        Main = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (move)
        {
            transform.position += direction * Time.deltaTime;
        }
    }

    public void SetDirection(Vector3 newDirection)
    {
        direction = new Vector3(newDirection.x, 0f, newDirection.z);

        trajectory.transform.parent = null;
        startingPoint = transform.position;

        if (Physics.Raycast(transform.position - Vector3.up * 0.7f, direction, out RaycastHit hit, 100.0f, wall))
        {
            oppositeWallPoint = hit.point;
        }

        Vector3 halfOffset = (oppositeWallPoint - startingPoint) / 2;
        trajectory.transform.position = transform.position + halfOffset;
        trajectory.transform.LookAt(oppositeWallPoint);
        trajectory.transform.localScale = new Vector3(
            trajectory.transform.localScale.x, 
            trajectory.transform.localScale.y, 
            halfOffset.magnitude * 2.25f);

        StartCoroutine(StartMoving());
    }

    #region Damage
    private void TryApplyDamage(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null && !PlayerMovement.IsInvincible() && !ignorePlayer && !thrown)
            {
                player.TakeDamage(damage);
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null && (!grabbed || (grabbed && thrown)))
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (damageOnEnter)
        {
            TryApplyDamage(other);
        }

        AfterActivation();
    }
    #endregion

    #region Movement Control
    private IEnumerator StartMoving()
    {
        GetComponent<MeshRenderer>().enabled = false;
        trailRenderer.enabled = false;

        yield return new WaitForSeconds(showTime);

        GetComponent<MeshRenderer>().enabled = true;
        trailRenderer.enabled = true;

        Destroy(trajectory);
        move = true;
    }

    public void StopMoving()
    {
        trailRenderer.enabled = false;
        move = false;
        grabbed = true;
    }

    public void ActivateTrail()
    {
        trailRenderer.enabled = true;
    }

    public void NewDirection(Vector3 newDir)
    {
        move = true;
        thrown = true;
        direction = newDir;
    }
    #endregion
}
