using UnityEngine;
using UnityEngine.Splines;

public class Gnome : MonoBehaviour
{
    [SerializeField] private float speed;

    private SplineAnimate spline;

    private RoomController roomController;
    public AudioManager AM;
    public AudioSource Main;

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
        AM=FindFirstObjectByType<AudioManager>();
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
            //AM.Play("Laugh", Main);
        }
    }

    public void Laugh()
    {
        AM.Play("Laugh", Main);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<EnemyHitbox>())
        {
            AM.Play("Die", null);
            Destroy(gameObject);
            return;
        }

        if (other.GetComponent<Projectile>())
        {
            AM.Play("Die", null);
            Destroy(gameObject);
            return;
        }

        if (other.GetComponent<AppleGrenade>())
        {
            AM.Play("Die", null);
            Destroy(gameObject);
            return;
        }
    }
}
