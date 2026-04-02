using UnityEngine;

public class TutorialEnemy : MonoBehaviour
{
    [SerializeField] TutorialDoor doorToOpen;
    private void OnDestroy()
    {
        doorToOpen.OpenDoor();
    }
}
