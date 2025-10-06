using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField][Range(0f, 1f)] float slowTime;
    [SerializeField] static float _slowTime;

    static List<GameObject> enemies = new List<GameObject>();

    public void Awake()
    {
        _slowTime = slowTime;
    }


    public static void SlowTime()
    {
        Debug.Log("Slowed time");
        Time.timeScale = _slowTime;
        Time.fixedDeltaTime = Time.timeScale * .02f;
    }

    public static void ReturnTime()
    {
        Debug.Log("Time back to normal");
        Time.timeScale = 1f;
        Time.fixedDeltaTime = Time.timeScale * .02f;
    }

}
