using UnityEngine;

public class ConditionHasMoreThan3HP : ICondition
{
    [SerializeField] private EnemyHealth enemyHealth;

    private void Awake()
    {
        if (!enemyHealth)
        {
            enemyHealth = transform.root.GetComponent<EnemyHealth>();
        }
    }

    public override bool Check()
    {
        return enemyHealth.health > 3;
    }
}
