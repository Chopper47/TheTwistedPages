using UnityEngine;
using UnityEngine.SceneManagement;

public class EndTutorialDoor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            SceneManager.LoadScene("alpha");
        }
    }
}
