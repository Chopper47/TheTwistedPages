using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Slow")]
public class SlowEffect : MysteryEffect
{
    public float slowMultiplier = 0.5f;
    public float duration = 5.0f;

    public override void Apply(GameObject player)
    {
        PlayerMovement movement = player.GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.ApplySlow(duration, slowMultiplier);
        }
    }
}
