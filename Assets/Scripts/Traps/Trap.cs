using System.Collections;
using UnityEngine;

public class Trap : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] protected int damage = 1;
    [SerializeField] protected bool damageOnEnter = true;
    [SerializeField] protected bool damageOverTime = false;
    [SerializeField] protected float damageRate = 1.0f;

    protected float damageTimer = 0f;

    protected virtual void AfterActivation() 
    {

    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Projectile"))
        {
            return;
        }

        if (damageOnEnter)
        {
            TryApplyDamage(other);
        }

        AfterActivation();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!damageOverTime)
        {
            return;
        }

        damageTimer += Time.deltaTime;

        if (damageTimer >= damageRate)
        {
            TryApplyDamage(other);
            damageTimer = 0f;
        }
    }

    private void TryApplyDamage(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<PlayerHealth>().enabled)
            {
                other.GetComponent<PlayerHealth>().TakeDamage(damage);
            }
        }

        else if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemy = other.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
