using UnityEngine;

public class CollisionRefresh : MonoBehaviour
{
    public virtual void OnCollisionEnter(Collision collision)
    {
        PlayerHealth player = collision.gameObject.GetComponent<PlayerHealth>();

        if(player)
        {
            OnPlayerHit();
        }

        if(collision.gameObject.CompareTag("Wall"))
        {
            OnWallHit();
        }
    }

    public virtual void OnPlayerHit()
    {
        //Whatever happens when it hits the player
    }

    public virtual void OnWallHit()
    {
        //Whatever happens when it hits a wall
    }

}
