using UnityEngine;

public class Axe : MonoBehaviour
{
    [SerializeField] int damage = 1;
    Vector3 dir;

    private void Update()
    {
        transform.Translate(dir * Time.deltaTime);
    }

    public void SetDir(Vector3 newDir)
    {
        dir = newDir;
    }


    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

        if(playerHealth != null )
        {
            playerHealth.TakeDamage(damage);
        }
    }
}
