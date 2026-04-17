using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/SlowerEnemies")]
public class SlowerEnemiesEffect : MysteryEffect
{
    public float slowValue = 0.8f;
    public int durationInRooms = 3;

    public override void Apply(GameObject player)
    {
        player.GetComponent<MonoBehaviour>().StartCoroutine(Run(player));
    }

    private IEnumerator Run(GameObject player)
    {
        RoomProgressTracker tracker = player.GetComponent<RoomProgressTracker>();
        if (tracker == null)
        {
            yield break;
        }

        int startRooms = tracker.roomsCleared;

        TimeManager.SetTimeScale(slowValue);
        TimeManager.SlowTime();

        while (tracker.roomsCleared < startRooms + durationInRooms)
        {
            yield return null;
        }

        TimeManager.ReturnTime();
    }
}
