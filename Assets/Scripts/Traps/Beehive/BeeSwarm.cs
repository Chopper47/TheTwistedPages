using UnityEngine;

public class BeeSwarm : Trap
{
    private Transform target;
    private float duration;
    private float timer = 0f;

    [Header("Movement")]
    [SerializeField] private float followSpeed = 6.0f;
    [SerializeField] private float hoverHeight = 1.5f;

    private void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }

        timer += Time.deltaTime;
        if (timer >= duration)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 targetPosition = target.position + Vector3.up * hoverHeight;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }

    public void Initialize(Transform target, float duration)
    {
        this.target = target;
        this.duration = duration;
    }
}
