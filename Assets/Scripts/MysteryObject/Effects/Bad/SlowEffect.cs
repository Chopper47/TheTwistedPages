using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Slow")]
public class SlowEffect : MysteryEffect
{
    public float slowMultiplier = 0.5f;
    public int durationInRooms = 3;

    public override void Apply(GameObject player)
    {
        MonoBehaviour runner = player.GetComponent<MonoBehaviour>();
        if (!runner)
        {
            return;
        }

        runner.StartCoroutine(Run(player));
    }

    private IEnumerator Run(GameObject player)
    {
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        RoomProgressTracker tracker = player.GetComponent<RoomProgressTracker>();

        if (movement == null || tracker == null)
        {
            yield break;
        }

        int startRooms = tracker.roomsCleared;

        movement.SetSlowMultiplier(slowMultiplier);

        while (tracker.roomsCleared < startRooms + durationInRooms)
        {
            yield return null;
        }

        movement.SetSlowMultiplier(1.0f);
    }
}
