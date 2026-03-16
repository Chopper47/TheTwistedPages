using UnityEngine;
using UnityEngine.SceneManagement;

public class EndTutorialDoor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            PlayerPrefs.SetInt("CompletedTutorial", 1);
            SceneManager.LoadScene("alpha");
        }
    }
}
