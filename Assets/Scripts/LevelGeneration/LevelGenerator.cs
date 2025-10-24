using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private Vector2Int gridDimensions = new Vector2Int(9, 9);
    [SerializeField] private Vector3 roomWorldSize = new Vector3(20f, 0f, 20f);
    [SerializeField] private int maxRooms = 12;

    [Header("Room Templates")]
    [SerializeField] private RoomPreset[] roomPresets;

    [Header("Room Locations")]
    public Dictionary<Vector2Int, RoomInstance> generatedRooms = new Dictionary<Vector2Int, RoomInstance>();

    private void Start()
    {
        GenerateLevel();
    }

    /// <summary>
    /// Generates a connected set of rooms in the grid using the DFS algorithm.
    /// </summary>
    private void GenerateLevel()
    {
        generatedRooms.Clear();

        Vector2Int startingPosition = new Vector2Int(gridDimensions.x / 2, gridDimensions.y / 2);
        Stack<Vector2Int> roomStack = new Stack<Vector2Int>();

        // Create starting room.
        RoomPreset startingRoomPreset = GetRandomRoomPreset(RoomType.Start);
        RoomInstance startingRoom = new RoomInstance(startingPosition, RoomType.Start, startingRoomPreset);
        generatedRooms.Add(startingPosition, startingRoom);
        roomStack.Push(startingPosition);

        // DFS.
        while (roomStack.Count > 0 && generatedRooms.Count < maxRooms)
        {
            Vector2Int currentPosition = roomStack.Peek();
            List<Vector2Int> unvisitedNeighbors = GetUnvisitedNeighbours(currentPosition);

            if (unvisitedNeighbors.Count > 0)
            {
                Vector2Int nextPosition = unvisitedNeighbors[Random.Range(0, unvisitedNeighbors.Count)];
                RoomPreset nextRoomPreset = GetRandomRoomPreset(RoomType.Normal);
                RoomInstance nextRoom = new RoomInstance(nextPosition, RoomType.Normal, nextRoomPreset);

                generatedRooms.Add(nextPosition, nextRoom);
                roomStack.Push(nextPosition);
            }
            else
            {
                roomStack.Pop();
            }
        }

        // Make the farthest room from the start the Boss room.
        Vector2Int bossRoomPosition = GetFarthestRoom(startingPosition);
        RoomInstance bossRoom = generatedRooms[bossRoomPosition];
        bossRoom.roomType = RoomType.Boss;
        bossRoom.roomPreset = GetRandomRoomPreset(RoomType.Boss);

        // Assign the Health and Shop rooms.
        AssignSpecialRooms();

        // Instantiate rooms in the 3D world.
        foreach (RoomInstance room in generatedRooms.Values)
        {
            InstantiateRoom(room);
        }
    }

    /// <summary>
    /// Returns a list of all unvisited neighbours (grid positions) of a room.
    /// </summary>
    /// <param name="position">The current room's grid position.</param>
    /// <returns>A list of empty neighbour positions within the grid.</returns>
    private List<Vector2Int> GetUnvisitedNeighbours(Vector2Int position)
    {
        List<Vector2Int> unvisited = new List<Vector2Int>();
        Vector2Int[] directions = 
        {
            Vector2Int.up, 
            Vector2Int.down, 
            Vector2Int.left, 
            Vector2Int.right 
        };

        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighbour = position + direction;

            // Skip positions that fall outside the grid boundaries.
            if (neighbour.x < 0 || neighbour.x >= gridDimensions.x || neighbour.y < 0 || neighbour.y >= gridDimensions.y)
            {
                continue;
            }

            // Add the neighbour if no room exists at that grid position yet.
            if (!generatedRooms.ContainsKey(neighbour))
            {
                unvisited.Add(neighbour);
            }
        }

        return unvisited;
    }

    /// <summary>
    /// Finds the room that's farthest from the given position.
    /// </summary>
    /// <param name="origin">The position to measure distance from.</param>
    /// <returns>The grid position of the farthest room.</returns>
    private Vector2Int GetFarthestRoom(Vector2Int origin)
    {
        Vector2Int farthest = origin;
        int maxDistance = -1;

        foreach (RoomInstance room in generatedRooms.Values)
        {
            int distance = Mathf.Abs(room.gridPosition.x - origin.x) + Mathf.Abs(room.gridPosition.y - origin.y);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthest = room.gridPosition;
            }
        }

        return farthest;
    }

    /// <summary>
    /// Randomly assigns one Shop room and one Health room from the normal rooms.
    /// </summary>
    private void AssignSpecialRooms()
    {
        List<RoomInstance> normalRooms = new List<RoomInstance>();

        foreach (RoomInstance room in generatedRooms.Values)
        {
            if (room.roomType == RoomType.Normal)
            {
                normalRooms.Add(room);
            }
        }

        if (normalRooms.Count > 2)
        {
            // Shop room.
            RoomInstance shopRoom = normalRooms[Random.Range(0, normalRooms.Count)];
            shopRoom.roomType = RoomType.Shop;
            shopRoom.roomPreset = GetRandomRoomPreset(RoomType.Shop);

            // Health room.
            RoomInstance healthRoom;
            do
            {
                healthRoom = normalRooms[Random.Range(0, normalRooms.Count)];
            } while (healthRoom == shopRoom);

            healthRoom.roomType = RoomType.Health;
            healthRoom.roomPreset = GetRandomRoomPreset(RoomType.Health);
        }
    }

    /// <summary>
    /// Instantiates a room in the 3D world at its designed grid position.
    /// </summary>
    private void InstantiateRoom(RoomInstance room)
    {
        Vector3 worldPosition = new Vector3(
            (room.gridPosition.x - gridDimensions.x / 2) * roomWorldSize.x,
            0,
            (room.gridPosition.y - gridDimensions.y / 2) * roomWorldSize.z
        );

        GameObject roomObject = Instantiate(room.roomPreset.roomPrefab, worldPosition, Quaternion.identity, transform);
        roomObject.name = $"room_{room.gridPosition.x}_{room.gridPosition.y}_{room.roomType}";
        room.roomPrefab = roomObject;

        RoomController roomController = roomObject.GetComponent<RoomController>();
        if (roomController != null)
        {
            roomController.roomInstance = room;
            roomController.roomPreset = room.roomPreset;
        }
    }

    /// <summary>
    /// Selects a random room preset from the available presets that matches the specified room type.
    /// </summary>
    /// <param name="roomType">The type of room to select a preset for (Start, Health, Shop, Boss or Normal).</param>
    /// <returns>A randomly selected preset that matches the given room type.</returns>
    private RoomPreset GetRandomRoomPreset(RoomType roomType)
    {
        List<RoomPreset> validPresets = new List<RoomPreset>();

        foreach (RoomPreset preset in roomPresets)
        {
            if (roomType == RoomType.Start && preset.canBeStart)
            {
                validPresets.Add(preset);
            }
            else if (roomType == RoomType.Boss && preset.canBeBoss)
            {
                validPresets.Add(preset);
            }
            else if (roomType == RoomType.Shop && preset.canBeShop)
            {
                validPresets.Add(preset);
            }
            else if (roomType == RoomType.Health && preset.canBeHealth)
            {
                validPresets.Add(preset);
            }
            else if (roomType == RoomType.Normal && !preset.canBeStart && !preset.canBeShop && !preset.canBeBoss && !preset.canBeHealth)
            {
                validPresets.Add(preset);
            }
        }

        if (validPresets.Count == 0)
        {
            validPresets.Add(roomPresets[0]);       // Fallback in case no matching presets are found.
        }

        return validPresets[Random.Range(0, validPresets.Count)];
    }
}
