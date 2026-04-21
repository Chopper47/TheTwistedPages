using UnityEngine;
using UnityEngine.SceneManagement;

public class BossRoomDoor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene("BossRoom");
        }
    }
}
