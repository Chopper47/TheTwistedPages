using UnityEngine;

public class BossRoomColumn : MonoBehaviour
{
    [SerializeField] private BossRoomFunctionality bossRoom;
    [SerializeField] private Lumberjack lumberjack;
    [SerializeField] private GameObject brokenColumn;
    [SerializeField] private Material brokenColumnMaterial;
    [SerializeField] private GameObject brokenColumnParticles;
    [SerializeField] private MeshRenderer columnMesh;

    [SerializeField] private int maxHp;
    [SerializeField] private int currentHp;

    [SerializeField] private float stunDuration = 3.0f;

    private void Start()
    {
        bossRoom = transform.GetComponentInParent<BossRoomFunctionality>();
        columnMesh = transform.GetComponentInChildren<MeshRenderer>();
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
        columnMesh.material = brokenColumnMaterial;

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
            GameObject particles = Instantiate(brokenColumnParticles, transform.position, Quaternion.identity);
            particles.GetComponent<ParticleSystem>().Play();
            Instantiate(brokenColumn, transform.position, transform.rotation);
            bossRoom.LoseHp();
            lumberjack.ApplyStun(stunDuration);
        }
    }


}