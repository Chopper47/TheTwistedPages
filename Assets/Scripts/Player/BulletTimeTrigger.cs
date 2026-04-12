using UnityEngine;

public class BulletTimeTrigger : MonoBehaviour
{
    private bool canDodge = false;

    private GameObject axeGO;
    private GameObject harmOrigin;

    private void OnTriggerEnter(Collider other)
    {

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


        if(axe != null)
        {
            axeGO = null;
            canDodge = false;
            harmOrigin = null;
        }

        if (bossAxe != null)
        {
            axeGO = null;
            canDodge = false;
            harmOrigin = null;
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

    public void DeactivateDodge()
    {
        Debug.Log("Deactivating dodge");
        canDodge = false;
    }
}
