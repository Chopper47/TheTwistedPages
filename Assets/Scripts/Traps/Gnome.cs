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
        if (roomController.isCleared == true && spline.IsPlaying) 
        { 
            RoomCleared(); 
        }
    }

    private void RoomCleared()
    {
        if (spline != null) 
        { 
            spline.Pause();
            CurrencyManager.instance.AddSingleCoin();
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
        if (other.GetComponent<EnemyHitbox>())
        {
            Destroy(gameObject);
            return;
        }

        if (other.GetComponent<FlowerProjectile>())
        {
            Destroy(gameObject);
            return;
        }

        if (other.GetComponent<AppleGrenade>())
        {
            Destroy(gameObject);
            return;
        }
    }
}
