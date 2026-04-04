using System.Collections;
using UnityEngine;

public class RootSpike : Trap
{
    [Header("Growth")]
    [SerializeField] private float growTime = 0.5f;
    [SerializeField] private float startY = -2.0f;
    [SerializeField] private float endY = 0f;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 360.0f;

    private bool isActive = false;

    protected override void AfterActivation()
    {
        
    }

    private void Start()
    {
        Vector3 rotation = transform.eulerAngles;
        rotation.y = Random.Range(0f, 360.0f);
        transform.eulerAngles = rotation;

        Vector3 position = transform.position;
        position.y += startY;
        transform.position = position;

        StartCoroutine(Grow());
    }

    private IEnumerator Grow()
    {
        float t = 0f;

        Vector3 startPosition = transform.position;
        Vector3 endPosition = new Vector3(startPosition.x, startPosition.y - startY + endY, startPosition.z);

        while (t < growTime)
        {
            t += Time.deltaTime;
            float normalized = t / growTime;

            float eased = normalized * normalized;

            transform.position = Vector3.Lerp(startPosition, endPosition, eased);

            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

            yield return null;
        }

        transform.position = endPosition;

        isActive = true;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (!isActive)
        {
            return;
        }

        base.OnTriggerEnter(other);
    }
}
