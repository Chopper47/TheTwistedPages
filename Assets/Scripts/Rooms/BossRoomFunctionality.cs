using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomFunctionality : MonoBehaviour
{

    [SerializeField] private List<BossRoomColumn> columns = new List<BossRoomColumn>();
    [SerializeField] private int maxHp;
    public int currentHp;

    [SerializeField] private EnemyHealth bossHealth;

    private void Start()
    {
        currentHp = maxHp;
    }
    public void LoseHp()
    {
        currentHp--;

        if(bossHealth != null) bossHealth.TakeDamage(1);

        if (currentHp <= 0)
        {
            EndBoss();
        }
    }

    public void EndBoss()
    {
        //TODO Boss death, fade out, end game
    }

}
