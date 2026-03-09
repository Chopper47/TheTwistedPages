using UnityEngine;

public class BeeSwarm : Trap
{
    private Transform target;
    private float duration;
    private float timer = 0f;
    [SerializeField] private AudioSource swarm;
    [Header("Movement")]
    [SerializeField] private float followSpeed = 6.0f;
    [SerializeField] private float hoverHeight = 1.5f;

    private Vector3 velocity;
    private ParticleSystem beeSwarm;
    private bool isDying = false;

    private void Awake()
    {
        beeSwarm = GetComponentInChildren<ParticleSystem>();
    }

    private void Update()
    {
        if (isDying)
        {
            return;
        }

        if (!target)
        {
            StopSwarm();
            return;
        }

        timer += Time.deltaTime;
        if (timer >= duration)
        {
            StopSwarm();
            return;
        }

        MoveSwarm();
    }

    private void MoveSwarm()
    {
        Vector3 targetPosition = target.position + Vector3.up * hoverHeight;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 0.2f, followSpeed);
    }

    private void StopSwarm()
    {
        isDying = true;

        beeSwarm.Stop();

        Destroy(gameObject, beeSwarm.main.startLifetime.constantMax);
    }

    public void Initialize(Transform target, float duration)
    {
        this.target = target;
        this.duration = duration;
        swarm.Play();
        timer = 0f;
    }
}