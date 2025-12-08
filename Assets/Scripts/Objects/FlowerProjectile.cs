using UnityEngine;

public class FlowerProjectile : MonoBehaviour
{
    [SerializeField] float speed;
    Vector3 direction = Vector3.zero;
    

    // Update is called once per frame
    void Update()
    {
        if (direction != Vector3.zero)
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    public void SetDirection(Vector3 newDir)
    {
        direction = newDir.normalized; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
