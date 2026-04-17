using UnityEngine;

[CreateAssetMenu(menuName = "Effects/Heal")]
public class HealEffect : MysteryEffect
{
    public int healAmount;

    public override void Apply(GameObject player)
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.Heal(healAmount);
        }
    }
}
