using UnityEngine;

public class BossRoomDoor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FindFirstObjectByType<FloorManager>().GoToNextFloor();
            //SceneManager.LoadScene("Scene_06_BossRoomTest");
        }
    }
}
