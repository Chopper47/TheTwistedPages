using UnityEngine;

public class FloorChangeObject : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FindFirstObjectByType<FloorManager>().GoToNextFloor();
        }
    }
}
