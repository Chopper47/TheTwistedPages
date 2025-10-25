using UnityEngine;

public class BulletTimeTrigger : MonoBehaviour
{
    private bool canDodge = false;

    private void OnTriggerEnter(Collider other)
    {
        EnemyHitbox enemyHb = other.GetComponent<EnemyHitbox>();

        if(enemyHb != null)
        {
            if(enemyHb.ShowEnemyState() == 2)
            {
                canDodge = true;
            }
        }
    }

    public void OnTriggerStay(Collider other)
    {
        EnemyHitbox enemyHb = other.GetComponent<EnemyHitbox>();

        if (enemyHb != null)
        {
            if (enemyHb.ShowEnemyState() != 2 && canDodge)
            {
                canDodge = false;
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        EnemyHitbox enemyHb = other.GetComponent<EnemyHitbox>();

        if (enemyHb != null)
        {
            canDodge = false;
        }
    }

    public bool CanDodge()
    { 
        return canDodge; 
    }
}
