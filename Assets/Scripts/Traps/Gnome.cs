using UnityEngine;
using UnityEngine.Splines;

public class Gnome : MonoBehaviour
{
    [SerializeField] float speed;

    SplineAnimate spline;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        spline = GetComponent<SplineAnimate>();

        if(spline != null )
        {
            spline.MaxSpeed = speed;

            if(spline.Container == null)
            {
                Debug.LogWarning("Path missing");
            }
        }
        else
        {
            Debug.LogWarning("Spline Animate component missing");
        }
    }

    private void Start()
    {
        StartMoving();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.J)) { RoomCleared(); }
    }

    void RoomCleared()
    {
        if(spline != null) { spline.Pause(); }
    }

    void StartMoving()
    {
        if (spline != null) { spline.Play(); }
    }

    private void OnTriggerEnter(Collider other)
    {
        EnemyHitbox enemy = other.gameObject.GetComponent<EnemyHitbox>();

        if(enemy)
        {
            Destroy(gameObject);
        }
    }
}
