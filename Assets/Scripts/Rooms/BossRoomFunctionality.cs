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
        
        currentHp = CheckNumberOfColumns();

        if (bossHealth != null) bossHealth.SetHealth(currentHp);

        if (currentHp <= 0)
        {
            EndBoss();
        }
    }

    int CheckNumberOfColumns()
    {
        int numberOfColumnsActive = 0;
        foreach (BossRoomColumn column in columns)
        {
            if (column.gameObject.activeSelf)
            {
                numberOfColumnsActive++;
            }
        }
        return numberOfColumnsActive;
    }

    public void EndBoss()
    {
        //TODO Boss death, fade out, end game
    }

}
