using UnityEngine;
using UnityEngine.SceneManagement;

public class EndTutorialDoor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(SaveManager.instance != null)
            {
                SaveManager.instance.player.tutorialCompleted = true;
                SaveManager.instance.SavePlayerData();
            }
            SceneManager.LoadScene("MainGame");
        }
    }
}
