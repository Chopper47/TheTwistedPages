using UnityEngine;

public class BossRoomColumn : MonoBehaviour
{
    [SerializeField] private BossRoomFunctionality bossRoom;
    private int maxHp;
    private int currentHp;

    private void Start()
    {
        bossRoom = transform.GetComponentInParent<BossRoomFunctionality>();

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "BossHitbox")
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
