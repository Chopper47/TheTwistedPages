using System.Collections;
using UnityEngine;

public class Flower : MonoBehaviour
{
    [Header("Burrow variables")]
    [SerializeField] private float maxBurrowedTime = 7f;
    [SerializeField] private int comeoutChance = 10;
    private bool passedMaxTime = false;
    private Coroutine timer;

    [Header("Non burrow variables")]
    [SerializeField] private GameObject projectile;
    [SerializeField] private float shootCD = 1.5f;
    [SerializeField] private float burrowRange = 2f;
    [SerializeField] private float shootRange = 7f;
    [SerializeField] private int burrowChance = 10;

    private bool burrowed = false;
    [Header("References")]
    [SerializeField] ChaseAgent chase;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsBurrowed()
    {
        return burrowed;
    }

    public void SetBurrow(bool newBurrowed)
    {
        chase.StopChase();

        burrowed = newBurrowed;

        if(burrowed)
        {
            StartCoroutine(BurrowTimer());
        }
    }
    
    IEnumerator BurrowTimer()
    {
        yield return new WaitForSeconds(maxBurrowedTime);

        passedMaxTime = true;
    }
    public bool TimePassed()
    {
        return passedMaxTime;
    }
    public bool ChanceBurrow()
    {
        int i = Random.Range(1, 101);

        if(i <= burrowChance)
        {
            return true;
        }
        return false;
    }

    public bool ChanceComeOut()
    {
        int i = Random.Range(1, 101);

        if (i <= comeoutChance)
        {
            return true;
        }
        return false;
    }

    public float ShootRange()
    {
        return shootRange;
    }

    public float BurrowRange()
    {
        return burrowRange;
    }

    public void Shoot()
    {
        Debug.Log("Shoot");
    }
}
