using UnityEngine;

public class BulletTimeTrigger : MonoBehaviour
{
    private bool canDodge = false;

    private GameObject latestEnemy;
    private GameObject axeGO;
    private GameObject harmOrigin;

    private void OnTriggerEnter(Collider other)
    {
        EnemyHitbox enemyHb = other.GetComponent<EnemyHitbox>();

        if(enemyHb != null)
        {
            if(enemyHb.ShowEnemyState() == 2)
            {
                latestEnemy = enemyHb.gameObject;
                harmOrigin = latestEnemy;
                canDodge = true;
            }
        }

        Axe axe = other.GetComponent<Axe>();

        if(axe != null)
        {
            axeGO = axe.gameObject;   
            if (harmOrigin == null)
            {
                harmOrigin = axeGO;
                canDodge=true;
            }
        }

        BossAxe bossAxe = other.GetComponent<BossAxe>();
        if(bossAxe != null && bossAxe.IsAttacking())
        {
            axeGO = bossAxe.gameObject;
            if(harmOrigin == null)
            {
                harmOrigin = axeGO;
                canDodge=true;
            }
        }
    }

    public void OnTriggerStay(Collider other)
    {
        EnemyHitbox enemyHb = other.GetComponent<EnemyHitbox>();
        if (enemyHb != null)
        {
            if (enemyHb.ShowEnemyState() != 2 && canDodge && axeGO != null)
            {
                harmOrigin = axeGO;
            }
            else if(enemyHb.ShowEnemyState() != 2 && canDodge && axeGO == null)
            {
                harmOrigin = null;
                canDodge = false;
            }
        }

        BossAxe bossAxe = other.GetComponent<BossAxe>();
        if (bossAxe != null)
        {
            if (!bossAxe.IsAttacking() && bossAxe.gameObject == harmOrigin)
            {
                harmOrigin = null;
                canDodge = false;
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        EnemyHitbox enemyHb = other.GetComponent<EnemyHitbox>();
        Axe axe = other.GetComponent<Axe>();
        BossAxe bossAxe = other.GetComponent<BossAxe>();

        if (enemyHb != null)
        {
            latestEnemy = null;
            if (axeGO != null)
            {
                harmOrigin = axeGO;
            }
        }

        if(axe != null)
        {
            axeGO = null;
            if(latestEnemy != null)
            {
                harmOrigin = latestEnemy;
            }
        }

        if (bossAxe != null)
        {
            axeGO = null;
            if (latestEnemy != null)
            {
                harmOrigin = latestEnemy;
            }

            if (latestEnemy == null)
            {
                canDodge = false;
                harmOrigin = null;
            }
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
