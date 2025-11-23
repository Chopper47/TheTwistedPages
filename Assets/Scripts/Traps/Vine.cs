using UnityEngine;
using System.Collections;

public class VineTrap : MonoBehaviour
{
    public Transform wallA;
    public Transform wallB;

    [Header("Settings")]
    public float waitTime = 5f;
    public float moveSpeed = 3f;

    private Transform currentTarget;

    [Header("Damage")]
    [SerializeField] private int damage = 1;
    [SerializeField] private float damageRate = 1.0f;
    private float damageTimer = 0f;

    [Header("Rotation")]
    public Vector3 rotationOnWall = new Vector3(0, 0, 0);
    public Vector3 rotationOnFloor = new Vector3(90, 0, 0);

    private void Start()
    {
        transform.position = wallA.position;
        transform.rotation = Quaternion.Euler(rotationOnWall);

        currentTarget = wallB;

        StartCoroutine(VineCycle());
    }

    private IEnumerator VineCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTime);

            transform.rotation = Quaternion.Euler(rotationOnFloor);

            yield return StartCoroutine(MoveToTarget(currentTarget));

            transform.rotation = Quaternion.Euler(rotationOnWall);

            if (currentTarget == wallA)
            {
                currentTarget = wallB;
            }
            else
            {
                currentTarget = wallA;
            }
        }
    }

    private IEnumerator MoveToTarget(Transform target)
    {
        while (Vector3.Distance(transform.position, target.position) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

            yield return null;
        }

        transform.position = target.position;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageRate)
            {
                PlayerHealth player = other.GetComponent<PlayerHealth>();
                if (player)
                {
                    player.TakeDamage(damage);
                }
                damageTimer = 0f;
            }
        }
    }
}
