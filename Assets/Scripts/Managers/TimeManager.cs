using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    [SerializeField] [Range(0f, 1.0f)] float slowTime;

    [SerializeField] static float _slowTime;

    private void Awake()
    {
        _slowTime = slowTime;
    }

    public static void SetTimeScale(float value)
    {
        _slowTime = Mathf.Clamp(value, 0f, 1.0f);
    }

    public static void SlowTime()
    {
        Debug.Log("Slowed time");
        Time.timeScale = _slowTime;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    public static void ReturnTime()
    {
        Debug.Log("Time back to normal");
        Time.timeScale = 1.0f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }
}
