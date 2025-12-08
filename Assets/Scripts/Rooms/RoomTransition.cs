using UnityEngine;
using System.Collections;

/// <summary>
/// Handles player movement between connected rooms.
/// </summary>
public class RoomTransition : MonoBehaviour
{
    private LevelGenerator levelGenerator;
    private Transform player;

    private void Awake()
    {
        levelGenerator = ReferenceManager.Instance.levelGenerator;
        player = ReferenceManager.Instance.player;
    }

    /// <summary>
    /// Moves the player to the room in the given direction if possible.
    /// </summary>
    public IEnumerator TryMoveToAdjacentRoom(RoomController currentRoom, DoorDirection direction)
    {
        yield return RoomTransitionFade.Instance.FadeOut();

        RoomInstance currentInstance = currentRoom.roomInstance;
        
        Vector2Int targetPosition = currentInstance.gridPosition + DirectionToOffset(direction);

        // Try to get the room in that direction.
        RoomInstance nextRoom;
        if (levelGenerator.generatedRooms.ContainsKey(targetPosition))
        {
            nextRoom = levelGenerator.generatedRooms[targetPosition];
        }
        else
        {
            Debug.Log($"No room found to the {direction} of {currentInstance.gridPosition}");
            yield break;
        }

        RoomController nextRoomController = nextRoom.roomPrefab.GetComponent<RoomController>();

        // Teleport player to the opposite door in the next room.
        TeleportPlayerToOppositeDoor(nextRoomController, direction);

        Debug.Log($"Teleporting player from {currentInstance.gridPosition} through its {direction} door to {nextRoom.gridPosition}");

        // Activate the new room.
        nextRoomController.ActivateRoom();

        Vector3 finalPlayerPos = player.position;

        CameraController.Instance.MoveToRoom(nextRoomController, finalPlayerPos);

        yield return RoomTransitionFade.Instance.FadeIn();
    }

    /// <summary>
    /// Teleports the player to the opposite door of the next room.
    /// </summary>
    private void TeleportPlayerToOppositeDoor(RoomController targetRoom, DoorDirection fromDirection)
    {
        DoorDirection toDirection = GetOppositeDirection(fromDirection);

        DoorController exitDoor = null;
        foreach (DoorController door in targetRoom.doors)
        {
            if (door.direction == toDirection)
            {
                exitDoor = door;
                break;
            }
        }

        if (exitDoor == null)
        {
            Debug.LogWarning($"No opposite door ({toDirection}) found in room {targetRoom.name}");
            return;
        }

        Vector3 targetPosition = exitDoor.transform.position + exitDoor.transform.forward * 1.5f;

        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
            player.position = targetPosition - Vector3.up * 0.5f;
            player.forward = -exitDoor.transform.forward;
            controller.enabled = true;
        }
    }

    /// <summary>
    /// Returns the vector offset corresponding to a given door direction.
    /// </summary>
    private Vector2Int DirectionToOffset(DoorDirection direction)
    {
        switch (direction)
        {
            case DoorDirection.North:
                return Vector2Int.up;       // (0, 1).

            case DoorDirection.South:
                return Vector2Int.down;     // (0, -1).

            case DoorDirection.East:
                return Vector2Int.right;    // (1, 0).

            case DoorDirection.West:
                return Vector2Int.left;     // (-1, 0).

            default:
                return Vector2Int.zero;
        }
    }

    /// <summary>
    /// Returns the opposite door direction.
    /// </summary>
    public DoorDirection GetOppositeDirection(DoorDirection direction)
    {
        switch (direction)
        {
            case DoorDirection.North: 
                return DoorDirection.South;

            case DoorDirection.South: 
                return DoorDirection.North;

            case DoorDirection.East: 
                return DoorDirection.West;

            case DoorDirection.West: 
                return DoorDirection.East;

            default: 
                return direction;
        }
    }
}
