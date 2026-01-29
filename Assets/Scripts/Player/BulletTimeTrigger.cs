using UnityEngine;

public class BulletTimeTrigger : MonoBehaviour
{
    private bool canDodge = false;

    private GameObject harmOrigin;

    private void OnTriggerEnter(Collider other)
    {
        EnemyHitbox enemyHb = other.GetComponent<EnemyHitbox>();

        if(enemyHb != null)
        {
            if(enemyHb.ShowEnemyState() == 2)
            {
                Debug.Log("close attack");
                harmOrigin = enemyHb.gameObject;
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
                harmOrigin = null;
                canDodge = false;
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        EnemyHitbox enemyHb = other.GetComponent<EnemyHitbox>();

        if (enemyHb != null)
        {
            harmOrigin = null;
            canDodge = false;
        }
    }

    public bool CanDodge()
    { 
        return canDodge; 
    }

    public GameObject FirstHarm()
    {
        return harmOrigin;
    }
}
