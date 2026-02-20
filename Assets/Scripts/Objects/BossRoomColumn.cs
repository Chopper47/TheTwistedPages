using UnityEngine;

public class BossRoomColumn : MonoBehaviour
{
    [SerializeField] private BossRoomFunctionality bossRoom;
    [SerializeField] private int maxHp;
    [SerializeField] private int currentHp;

    private void Start()
    {
        bossRoom = transform.GetComponentInParent<BossRoomFunctionality>();
        currentHp = maxHp;

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "BossHitbox")
        {
            LoseHp(other.gameObject.GetComponent<Hitbox>().damage);
        }
    }

    private void LoseHp(int dmgAmount)
    {
        currentHp -= dmgAmount;
        if (currentHp <= 0)
        {
            bossRoom.LoseHp();
            DestroySelf();
        }   
    }
    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
