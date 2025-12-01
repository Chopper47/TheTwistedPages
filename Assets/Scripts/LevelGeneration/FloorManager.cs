using System.Collections;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    public static FloorManager Instance;

    [Header("Floors")]
    public int totalFloors = 3;
    private int currentFloor = 1;

    [Header("References")]
    private LevelGenerator levelGenerator;
    private Transform player;
    private CharacterController controller;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        levelGenerator = FindFirstObjectByType<LevelGenerator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        controller = player.GetComponent<CharacterController>();
    }

    private void Start()
    {
        StartCoroutine(FloorTransition());
    }

    private void GenerateCurrentFloor()
    {
        if (currentFloor == totalFloors)
        {
            levelGenerator.isLastFloor = true;
        }
        else
        {
            levelGenerator.isLastFloor = false;
        }

        Debug.Log($"Generating floor {currentFloor}/{totalFloors}.");

        levelGenerator.GenerateLevel();
    }

    public void GoToNextFloor()
    {
        if (currentFloor >= totalFloors)
        {
            return;
        }

        currentFloor++;
        StartCoroutine(FloorTransition());
    }

    private void MovePlayerToStartRoom()
    {
        foreach (RoomInstance room in levelGenerator.generatedRooms.Values)
        {
            if (room.roomType == RoomType.Start)
            {
                RoomController roomController = room.roomPrefab.GetComponent<RoomController>();

                Vector3 position = room.roomPrefab.transform.position;
                player.transform.position = position + Vector3.up * 1.5f;

                CameraController.Instance.MoveToRoom(roomController);

                return;
            }
        }
    }

    private IEnumerator FloorTransition()
    {
        PlayerMovement.canMove = false;
        controller.enabled = false;

        yield return new WaitForSeconds(0.25f);

        foreach (Transform child in levelGenerator.transform)
        {
            Destroy(child.gameObject);
        }

        yield return new WaitForSeconds(0.25f);

        GenerateCurrentFloor();

        yield return new WaitForSeconds(0.25f);

        MovePlayerToStartRoom();

        yield return new WaitForSeconds(0.25f);

        controller.enabled = true;
        PlayerMovement.canMove = true;
    }
}
