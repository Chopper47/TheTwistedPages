using Unity.VisualScripting;
using UnityEngine;

public class CollisionRefresh : MonoBehaviour
{
    
    /// <summary>
    /// Collision method for every melee enemy
    /// </summary>
    /// <param name="collision">Object collided with</param>
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

    /// <summary>
    /// Collision method for every melee enemy
    /// </summary>
    /// <param name="other">ObjectEnter</param>
    public virtual void OnTriggerEnter(Collider other)
    {
        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if (player)
        {
            OnPlayerHit();
        }

        if (other.CompareTag("Wall"))
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
