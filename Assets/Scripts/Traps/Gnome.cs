using UnityEngine;
using UnityEngine.Splines;

public class Gnome : MonoBehaviour
{
    [SerializeField] private float speed;

    private SplineAnimate spline;

    private RoomController roomController;

    private void Awake()
    {
        spline = GetComponent<SplineAnimate>();

        if (spline != null)
        {
            spline.MaxSpeed = speed;

            if (spline.Container == null)
            {
                Debug.LogWarning("Path missing");
            }
        }
        else
        {
            Debug.LogWarning("Spline Animate component missing");
        }

        roomController = GetComponentInParent<RoomController>();
    }

    private void Start()
    {
        StartMoving();
    }

    private void Update()
    {
        if (roomController.isCleared == true) 
        { 
            RoomCleared(); 
        }
    }

    private void RoomCleared()
    {
        if (spline != null) 
        { 
            spline.Pause();
            Debug.Log("Spline paused.");
        }
    }

    private void StartMoving()
    {
        if (spline != null) 
        { 
            spline.Play(); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyHitbox enemy = other.gameObject.GetComponent<EnemyHitbox>();

        if (enemy)
        {
            Destroy(gameObject);
        }
    }
}
