using System.Collections;
using UnityEngine;

public class AxeThrow : MonoBehaviour
{
    [Header("Throw Settings")]
    [SerializeField] float timeBetweenThrows = 5.0f;
    [SerializeField] int throwChance = 30;
    [SerializeField] private float throwForce = 40.0f;
    [SerializeField] private LayerMask layerMask;

    [Header("References")]
    [SerializeField] private GameObject axePrefab;
    [SerializeField] private CameraController cameraController;

    public bool testingCheck = true;

    private void Awake()
    {
        if (!cameraController)
        {
            cameraController = FindAnyObjectByType<CameraController>();
        }
    }

    private void Start()
    {
        StartCoroutine(TryThrowAxe());
    }

    private IEnumerator TryThrowAxe()
    {
        yield return new WaitForSeconds(timeBetweenThrows);

        int random = Random.Range(1, 101);

        if (random <= throwChance)
        {
            if(testingCheck)
            {
                ThrowAxe();
            }
            else
            {
                if (cameraController && !cameraController.IsCurrentRoomFinished())
                {
                    ThrowAxe();
                }
            }
        }

        StartCoroutine(TryThrowAxe());
    }

    private void ThrowAxe()
    {
        Vector3 randomDir = new Vector3(Random.Range(-1.0f, 1.0f),0, Random.Range(-1.0f, 1.0f)).normalized;

        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.7f, randomDir, out hit, 100.0f, layerMask))
        {
            GameObject axeGO = Instantiate(axePrefab, hit.point, Quaternion.identity, null);

            axeGO.GetComponent<Axe>().SetDirection((transform.position - hit.point).normalized * throwForce);
        }
    }
}
