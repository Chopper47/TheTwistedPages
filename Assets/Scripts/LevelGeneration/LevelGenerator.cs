using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class LevelGenerator : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private Vector2Int gridDimensions = new Vector2Int(9, 9);
    [SerializeField] private Vector3 roomWorldSize = new Vector3(20f, 0f, 20f);
    [SerializeField] private int maxRooms = 12;
    private Vector2Int startingPosition;
    private Stack<Vector2Int> roomStack;

    [Header("Room Templates")]
    [SerializeField] private RoomPreset[] roomPresets;

    [Header("Room Locations")]
    public Dictionary<Vector2Int, RoomInstance> generatedRooms = new Dictionary<Vector2Int, RoomInstance>();

    [HideInInspector] public bool isLastFloor = false;

    /// <summary>
    /// Generates a connected set of rooms in the grid using the DFS algorithm.
    /// </summary>
    public void GenerateLevel()
    {
        generatedRooms.Clear();

        startingPosition = new Vector2Int(gridDimensions.x / 2, gridDimensions.y / 2);
        roomStack = new Stack<Vector2Int>();

        // Create starting room.
        RoomPreset startingRoomPreset = GetRandomRoomPreset(RoomType.Start, startingPosition);
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
                RoomPreset nextRoomPreset = GetRandomRoomPreset(RoomType.Normal, nextPosition);
                RoomInstance nextRoom = new RoomInstance(nextPosition, RoomType.Normal, nextRoomPreset);

                generatedRooms.Add(nextPosition, nextRoom);
                roomStack.Push(nextPosition);
            }
            else
            {
                roomStack.Pop();
            }
        }

        // Make the farthest room from the start the floor-ending room (Boss or FloorChange).
        Vector2Int farthestRoomPosition = GetFarthestRoom(startingPosition);
        RoomInstance farthestRoom = generatedRooms[farthestRoomPosition];

        if (isLastFloor)
        {
            farthestRoom.roomType = RoomType.Boss;
            farthestRoom.roomPreset = GetRandomRoomPreset(RoomType.Boss, farthestRoomPosition);
        }
        else
        {
            farthestRoom.roomType = RoomType.FloorChange;
            farthestRoom.roomPreset = GetRandomRoomPreset(RoomType.FloorChange, farthestRoomPosition);
        }

        // Assign the Health and Shop rooms.
        AssignSpecialRooms();

        // Instantiate rooms in the 3D world.
        foreach (RoomInstance room in generatedRooms.Values)
        {
            InstantiateRoom(room);
        }

        foreach (RoomInstance room in generatedRooms.Values)
        {
            if (room.roomPrefab == null)
            {
                continue;
            }

            RoomController roomController = room.roomPrefab.GetComponent<RoomController>();
            if (roomController == null)
            {
                continue;
            }

            foreach (DoorController door in roomController.doors)
            {
                Vector2Int neighbourPosition = room.gridPosition;

                switch (door.direction)
                {
                    case DoorDirection.North:
                        neighbourPosition += Vector2Int.up;
                        break;
                    case DoorDirection.South:
                        neighbourPosition += Vector2Int.down;
                        break;
                    case DoorDirection.East:
                        neighbourPosition += Vector2Int.right;
                        break;
                    case DoorDirection.West:
                        neighbourPosition += Vector2Int.left;
                        break;
                }

                if (!generatedRooms.ContainsKey(neighbourPosition))
                {
                    door.gameObject.SetActive(false);
                }
            }
        }
    }

    private Dictionary<Vector2Int, int> BFS(Vector2Int origin)
    {
        // Dictionary storing visited rooms and their distance in steps from the origin.
        Dictionary<Vector2Int, int> distances = new Dictionary<Vector2Int, int>();
        
        Queue<Vector2Int> queue = new Queue<Vector2Int>();

        // Initialize BFS with the origin room.
        distances[origin] = 0;
        queue.Enqueue(origin);

        // Cardinal directions used to explore the grid neighbours.
        Vector2Int[] directions = new Vector2Int[]
        { 
            Vector2Int.up, 
            Vector2Int.down, 
            Vector2Int.left, 
            Vector2Int.right 
        };

        while (queue.Count > 0)
        {
            // Get the next room to process.
            Vector2Int current = queue.Dequeue();

            // Read its stored BFS distance.
            int currentDistance = distances[current];

            // Check all four neighbours.
            for (int i = 0; i < directions.Length; i++)
            {
                Vector2Int neighbour = current + directions[i];

                // The neighbour must exist in the generated level.
                if (!generatedRooms.ContainsKey(neighbour))
                {
                    continue;
                }

                // The neighbour must not have been visited yet.
                if (distances.ContainsKey(neighbour))
                {
                    continue;
                }

                distances[neighbour] = currentDistance + 1;
                queue.Enqueue(neighbour);
            }
        }
        return distances;
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
    /// Returns the room that is farthest from the origin using BFS (measured in number of room-to-room steps).
    /// </summary>
    /// <param name="origin">The position to measure distance from.</param>
    /// <returns>The grid position of the farthest room.</returns>
    private Vector2Int GetFarthestRoom(Vector2Int origin)
    {
        Dictionary<Vector2Int, int> distances = BFS(origin);
        int maxDistance = -1;
        Vector2Int farthest = origin;

        foreach (Vector2Int position in distances.Keys)
        {
            int distance = distances[position];

            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthest = position;
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

        if (normalRooms.Count <= 2)
        {
            return;
        }

        // Shop room.
        //RoomInstance shopRoom = normalRooms[Random.Range(0, normalRooms.Count)];
        //shopRoom.roomType = RoomType.Shop;
        //shopRoom.roomPreset = GetRandomRoomPreset(RoomType.Shop, shopRoom.gridPosition);

        // Health room (cannot be generated at a distance less than three depthwise from the starting room).
        Dictionary<Vector2Int, int> distances = BFS(startingPosition);

        List<RoomInstance> healthCandidates = new List<RoomInstance>();

        foreach (RoomInstance room in normalRooms)
        {
            //if (room == shopRoom)
            //{
            //    continue;
            //}

            if (distances.ContainsKey(room.gridPosition) && distances[room.gridPosition] >= 3)
            {
                healthCandidates.Add(room);
            }
        }

        // Fallback in case there are not at least three depth levels in the generated grid.
        if (healthCandidates.Count == 0)
        {
            healthCandidates.AddRange(normalRooms);
        }

        RoomInstance healthRoom = healthCandidates[Random.Range(0, healthCandidates.Count)];
        healthRoom.roomType = RoomType.Health;
        healthRoom.roomPreset = GetRandomRoomPreset(RoomType.Health, healthRoom.gridPosition);
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
    private RoomPreset GetRandomRoomPreset(RoomType roomType, Vector2Int position)
    {
        // Collect all presets that match the requested room type.
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
            //else if (roomType == RoomType.Shop && preset.canBeShop)
            //{
            //    validPresets.Add(preset);
            //}
            else if (roomType == RoomType.Health && preset.canBeHealth)
            {
                validPresets.Add(preset);
            }
            else if (roomType == RoomType.FloorChange && preset.canBeFloorChange)
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
            Debug.LogWarning($"No valid presets were found for type {roomType}.");
            return roomPresets[Random.Range(0, roomPresets.Length)];            // Fallback in case no matching presets are found.
        }

        // Collect presets used by neighbouring rooms.
        Vector2Int[] directions = new Vector2Int[] 
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        List<RoomPreset> neighbourPresets = new List<RoomPreset>();

        for (int i = 0; i < directions.Length; i++)
        {
            Vector2Int neighbourPosition = position + directions[i];

            // If a room already exists at this neighbour, save its preset.
            if (generatedRooms.ContainsKey(neighbourPosition))
            {
                neighbourPresets.Add(generatedRooms[neighbourPosition].roomPreset);
            }
        }

        // Filter out presets that match any neighbouring preset.
        List<RoomPreset> filteredPresets = new List<RoomPreset>();

        for (int i = 0; i < validPresets.Count; i++)
        {
            RoomPreset candidate = validPresets[i];
            bool matchesNeighbour = false;

            for (int j = 0; j < neighbourPresets.Count; j++)
            {
                if (candidate == neighbourPresets[j])
                {
                    matchesNeighbour = true;
                    break;
                }
            }

            // Keep only presets that are not equal to any neighbour.
            if (!matchesNeighbour)
            {
                filteredPresets.Add(candidate);
            }
        }

        // Fallback to all valid presets in case all presets were filtered out.
        if (filteredPresets.Count == 0)
        {
            Debug.LogWarning("Filtered presets is empty.");
            filteredPresets = validPresets;
        }

        List<RoomPreset> difficultyFilteredPresets = new List<RoomPreset>();

        if (roomStack != null && roomStack.Count >= 2)
        {
            Vector2Int[] stackArray = roomStack.ToArray();

            Vector2Int last = stackArray[0];                // Top of the stack.
            Vector2Int secondToLast = stackArray[1];        // Second from the top.

            int lastDifficulty = generatedRooms[last].roomPreset.difficultyRating;
            int secondDifficulty = generatedRooms[secondToLast].roomPreset.difficultyRating;

            if (lastDifficulty == secondDifficulty)
            {
                int forbidden = lastDifficulty;

                foreach (RoomPreset candidate in filteredPresets)
                {
                    if (candidate.difficultyRating != forbidden)
                    {
                        difficultyFilteredPresets.Add(candidate);
                    }
                }
            }
        }

        if (difficultyFilteredPresets.Count == 0)
        {
            difficultyFilteredPresets = filteredPresets;
        }

        // Return a random preset from the final list of filtered presets.
        int randomIndex = Random.Range(0, difficultyFilteredPresets.Count);
        return difficultyFilteredPresets[randomIndex];
    }
}
