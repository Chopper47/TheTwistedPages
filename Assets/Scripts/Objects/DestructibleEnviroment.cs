using UnityEngine;

public class DestructibleEnviroment : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag == "DestructionCheck")
        {
            Destroy(gameObject);
        }
    }
}
