using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossRoomFunctionality : MonoBehaviour
{

    [SerializeField] private List<BossRoomColumn> columns = new List<BossRoomColumn>();
    [SerializeField] private int maxHp;
    public int currentHp;
    public List<AudioSource> pillarHit;
    public AudioSource bossDeath;
    public UnityEvent onBossEnd;


    [SerializeField] private EnemyHealth bossHealth;

    private void Start()
    {
        currentHp = maxHp;
    }
    public void LoseHp()
    {
        int hit = Random.Range(0, 2);
        pillarHit[hit].Play();
        currentHp = CheckNumberOfColumns();

        if (bossHealth != null) bossHealth.SetHealth(currentHp);

        if (currentHp <= 0)
        {
            EndBoss();
            bossDeath.Play();
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
        var tempEvent = onBossEnd;
        onBossEnd = new UnityEngine.Events.UnityEvent();
        tempEvent?.Invoke();
    }

}
