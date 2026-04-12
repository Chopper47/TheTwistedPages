using UnityEngine;

public class RoomProgressTracker : MonoBehaviour
{
    public int roomsCleared = 0;

    public void RegisterRoomClear()
    {
        roomsCleared++;
    }

    public void ResetRun()
    {
        roomsCleared = 0;
    }
}
