using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FloorManager : MonoBehaviour
{
    public static FloorManager Instance;

    [Header("Floors")]
    public int totalFloors = 3;
    private int currentFloor = 1;
    private bool isFirstFloor = true;

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
                player.transform.position = position;

                CameraController.Instance.MoveToRoom(roomController, position);

                return;
            }
        }
    }

    private IEnumerator FloorTransition()
    {
        PlayerMovement.canMove = false;
        controller.enabled = false;

        if (!isFirstFloor)
        {
            yield return StartCoroutine(RoomTransitionFade.Instance.FadeOut());
        }
        else
        {
            Color c = RoomTransitionFade.Instance.GetComponentInChildren<Image>().color;
            c.a = 1;
            RoomTransitionFade.Instance.GetComponentInChildren<Image>().color = c;
        }

        foreach (Transform child in levelGenerator.transform)
        {
            Destroy(child.gameObject);
        }

        yield return null;

        GenerateCurrentFloor();

        MovePlayerToStartRoom();

        yield return StartCoroutine(RoomTransitionFade.Instance.FadeIn());

        isFirstFloor = false;

        yield return new WaitForSeconds(0.25f);

        controller.enabled = true;
        PlayerMovement.canMove = true;
    }
}
