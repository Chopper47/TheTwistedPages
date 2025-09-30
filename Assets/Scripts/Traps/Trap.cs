using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private int damage;

    public int GetDamage()
    {
        return damage;
    }
}
