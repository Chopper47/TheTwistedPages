using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/FasterEnemies")]
public class FasterEnemiesEffect : MysteryEffect
{
    public float fastValue = 1.2f;
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

        TimeManager.SetTimeScale(fastValue);
        TimeManager.SlowTime();

        while (tracker.roomsCleared < startRooms + durationInRooms)
        {
            yield return null;
        }

        TimeManager.ReturnTime();
    }
}
