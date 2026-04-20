using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CheckDummyRoom : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject enemy;
    [SerializeField] private Vector3 enemySpawnpoint;
    [SerializeField] private GameObject dummy;
    [SerializeField] private PlayerUpgrades playerUpgrades;
    [SerializeField] private Transform playerSpawnPoint;
    [SerializeField] private TextMeshProUGUI useUpgradeText;
    [SerializeField] private DialogueData restartDialogue;
    [SerializeField] private InputActionReference interactAction;


    [SerializeField] private RandomUpgrade dummyUpgrade;
    [SerializeField] private bool checkingDummyDeath;

    private void Start()
    {
        enemySpawnpoint = enemy.transform.position;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            string binding = interactAction.action.GetBindingDisplayString(1);
            useUpgradeText.text = "Press " + binding + " to use your Active Upgrade";
            useUpgradeText.gameObject.SetActive(true);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(dummy == null)
        {
            if (FindFirstObjectByType<Dummy>() != null)
            {
                dummy = FindFirstObjectByType<Dummy>().gameObject;
                checkingDummyDeath = true;
            }
        }

        if (checkingDummyDeath && dummy == null)
        {
            if(enemy != null)
            {
                checkingDummyDeath = false;
                enemy.GetComponent<NavMeshAgent>().enabled = false;
                enemy.GetComponent<ChaseAgent>().enabled = false;
                enemy.GetComponent<DecisionTreeAgent>().enabled = false;
                DialogueManager.instance.currentDialogue = restartDialogue;
                DialogueManager.instance.onDialogueEnd.AddListener(RestartRoom);
                DialogueManager.instance.StartDialogue();
            }
        }
    }

    void RestartRoom()
    {
        playerUpgrades.activeUpgrade = dummyUpgrade;
        enemy.transform.position = enemySpawnpoint;
        enemy.GetComponent<NavMeshAgent>().enabled = true;
        enemy.GetComponent<ChaseAgent>().enabled = true;
        enemy.GetComponent<DecisionTreeAgent>().enabled = true;
        playerUpgrades.gameObject.transform.position = playerSpawnPoint.position;
    }
}
