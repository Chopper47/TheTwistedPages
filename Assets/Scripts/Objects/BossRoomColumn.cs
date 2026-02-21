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
            if (other.gameObject.GetComponent<Hitbox>())
            {
                LoseHp(other.gameObject.GetComponent<Hitbox>().damage);

            }
        }
    }

    private void LoseHp(int dmgAmount)
    {
        currentHp -= dmgAmount;
        if (currentHp <= 0)
        {
            DestroySelf();
        }   
    }
    private void DestroySelf()
    {
        if (gameObject)
        {
            gameObject.SetActive(false);
            bossRoom.LoseHp();

        }
    }
}
