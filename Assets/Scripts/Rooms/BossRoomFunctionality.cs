using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomFunctionality : MonoBehaviour
{

    [SerializeField] private List<BossRoomColumn> columns = new List<BossRoomColumn>();
    [SerializeField] private int maxHp = 6;
    public int currentHp;

    private void Start()
    {
        currentHp = maxHp;
    }
    public void LoseHp()
    {
        currentHp--;
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
