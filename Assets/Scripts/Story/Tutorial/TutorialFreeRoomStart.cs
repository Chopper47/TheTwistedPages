using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialFreeRoomStart : MonoBehaviour
{
    [SerializeField] private GameObject enemyToEnable;
    [SerializeField] private PlayerMovement player;

    private void Awake()
    {
        enemyToEnable.SetActive(false);
        player = FindFirstObjectByType<PlayerMovement>();
    }

    
    private void OnTriggerEnter(Collider other)
    {

        if(other.CompareTag("Player"))
        {
            Debug.Log("Player entered tutorial free room start trigger");
            StartCoroutine(EnableMovementAndEnemy());
        }
    }
    IEnumerator EnableMovementAndEnemy()
    {
        yield return new WaitForSeconds(0.4f);
        if (enemyToEnable)
        {
            enemyToEnable.SetActive(true);
        }
        PlayerMovement.canMove = true;
        player.canRoll = true;
        Destroy(gameObject);

    }


}
