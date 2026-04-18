using System.Collections.Generic;
using UnityEngine;

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

    [Header("Upgrade Room")]
    [SerializeField] private RandomUpgradeList upgradeList;
    [SerializeField] private UpgradeStats permanentUpgradeStats;

    [Header("Room Locations")]
    public Dictionary<Vector2Int, RoomInstance> generatedRooms = new Dictionary<Vector2Int, RoomInstance>();

    [HideInInspector] public bool isLastFloor = false;


    public void TryGenerateLevel()
    {
        int maxAttempts = 10000;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            if (GenerateLevel())
            {
                return;
            }

            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        Debug.LogError("Level generation failed after many attempts.");
    }

    /// <summary>
    /// Generates a connected set of rooms in the grid using the DFS algorithm.
    /// </summary>
    public bool GenerateLevel()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

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

                generatedRooms[currentPosition].connectedRooms.Add(nextPosition);
                nextRoom.connectedRooms.Add(currentPosition);

                roomStack.Push(nextPosition);
            }
            else
            {
                roomStack.Pop();
            }
        }

        // Make the farthest room from the start the floor-ending room (Boss or FloorChange).
        Vector2Int farthestRoomPosition = GetFarthestRoom(startingPosition);

        Vector2Int finalRoomPosition = farthestRoomPosition + Vector2Int.up;

        bool insideGrid = finalRoomPosition.x >= 0 && finalRoomPosition.x < gridDimensions.x && finalRoomPosition.y >= 0 && finalRoomPosition.y < gridDimensions.y;

        bool isEmpty = !generatedRooms.ContainsKey(finalRoomPosition);

        if (!insideGrid || !isEmpty)
        {
            Debug.LogWarning("Boss position invalid or occupied. Regenerating level...");
            return false;
        }

        // Determine the special room type, whether it's a Boss or FloorChange room.
        RoomType roomType;

        if (isLastFloor)
        {
            roomType = RoomType.Boss;
        }
        else
        {
            roomType = RoomType.FloorChange;
        }

        // Select, create and add the room's preset.
        RoomPreset finalRoomPreset = GetRandomRoomPreset(roomType, finalRoomPosition);

        RoomInstance finalRoom = new RoomInstance(finalRoomPosition, roomType, finalRoomPreset);
        generatedRooms.Add(finalRoomPosition, finalRoom);

        generatedRooms[farthestRoomPosition].connectedRooms.Add(finalRoomPosition);
        finalRoom.connectedRooms.Add(farthestRoomPosition);

        // Assign the Health and Shop rooms.
        if (!AssignSpecialRooms())
        {
            return false;
        }

        if (IsTooLinear())
        {
            Debug.Log("Level too linear, regenerating...");
            return false;
        }

        AddExtraConnections(0.2f);

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

                RoomInstance currentRoom = room;

                if (!currentRoom.connectedRooms.Contains(neighbourPosition))
                {
                    door.gameObject.SetActive(false);
                    continue;
                }

                RoomType neighbourType = generatedRooms[neighbourPosition].roomType;

                if (neighbourType == RoomType.Boss || neighbourType == RoomType.FloorChange)
                {
                    if (!(door.direction == DoorDirection.North && neighbourPosition == room.gridPosition + Vector2Int.up))
                    {
                        door.gameObject.SetActive(false);
                        continue;
                    }
                }
            }
        }

        MinimapManager minimapManager = ReferenceManager.Instance.minimapManager;
        if (minimapManager != null)
        {
            minimapManager.InitializeMinimap(generatedRooms);
        }

        return true;
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

    private bool IsTooLinear()
    {
        int corridorRooms = 0;

        foreach (RoomInstance room in generatedRooms.Values)
        {
            int connections = room.connectedRooms.Count;

            if (connections == 2)
            {
                corridorRooms++;
            }
        }

        float ratio = (float)corridorRooms / generatedRooms.Count;

        return ratio > 0.6f;
    }

    /// <summary>
    /// Randomly assigns one Shop room and one Health room from the normal rooms.
    /// </summary>
    private bool AssignSpecialRooms()
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
            return true;
        }

        Dictionary<Vector2Int, int> distances = BFS(startingPosition);

        // Upgrade room(s) (cannot be generated at a distance less than two depthwise from the starting room).
        int upgradeRoomCount;
        if(Random.value <= 0.5f + (permanentUpgradeStats.roomChance * 0.1f))
        {
            upgradeRoomCount = 2;
        }
        else
        {
            upgradeRoomCount = 1;
        }

        List<RoomInstance> upgradeCandidates = new List<RoomInstance>();

        foreach (RoomInstance room in normalRooms)
        { 
            if (distances.ContainsKey(room.gridPosition) && distances[room.gridPosition] >= 2)
            {
                upgradeCandidates.Add(room);
            }
        }

        List<RoomInstance> selectedUpgradeRooms = new List<RoomInstance>();

        for (int i = 0; i < upgradeRoomCount; i++)
        {
            List<RoomInstance> validCandidates = new List<RoomInstance>();

            foreach (RoomInstance candidate in upgradeCandidates)
            {
                bool valid = true;

                Dictionary<Vector2Int, int> distancesFromCandidate = BFS(candidate.gridPosition);

                foreach (RoomInstance selected in selectedUpgradeRooms)
                {
                    if (distancesFromCandidate.TryGetValue(selected.gridPosition, out int distance))
                    {
                        if (distance < 3)
                        {
                            valid = false;
                            break;
                        }
                    }
                }

                if (!valid)
                {
                    continue;
                }

                foreach (RoomInstance room in generatedRooms.Values)
                {
                    if (room.roomType != RoomType.Boss &&
                        room.roomType != RoomType.FloorChange &&
                        room.roomType != RoomType.Start)
                    {
                        continue;
                    }

                    if (distancesFromCandidate.TryGetValue(room.gridPosition, out int dist))
                    {
                        if (dist < 3)
                        {
                            valid = false;
                            break;
                        }
                    }
                }

                if (valid)
                {
                    validCandidates.Add(candidate);
                }
            }

            if (validCandidates.Count == 0 && upgradeCandidates.Count > 0)
            {
                validCandidates.AddRange(upgradeCandidates);
            }

            if (validCandidates.Count == 0)
            {
                return false;
            }

            RoomInstance upgradeRoom = validCandidates[Random.Range(0, validCandidates.Count)];

            upgradeRoom.roomType = RoomType.Upgrade;
            upgradeRoom.roomPreset = GetRandomRoomPreset(RoomType.Upgrade, upgradeRoom.gridPosition);

            selectedUpgradeRooms.Add(upgradeRoom);
            normalRooms.Remove(upgradeRoom);
            upgradeCandidates.Remove(upgradeRoom);
        }

        // Health room (cannot be generated at a distance less than three depthwise from the starting room).
        List<RoomInstance> healthCandidates = new List<RoomInstance>();

        foreach (RoomInstance room in normalRooms)
        {
            if (distances.ContainsKey(room.gridPosition) && distances[room.gridPosition] >= 3)
            {
                healthCandidates.Add(room);
            }
        }

        // Fallback in case there are not at least three depth levels in the generated grid.
        if (healthCandidates.Count == 0)
        {
            Debug.LogWarning("There are not at least three depth levels in the generated level. Regenerating level...");
            return false;
        }

        RoomInstance healthRoom = healthCandidates[Random.Range(0, healthCandidates.Count)];
        healthRoom.roomType = RoomType.Health;
        healthRoom.roomPreset = GetRandomRoomPreset(RoomType.Health, healthRoom.gridPosition);

        return true;
    }

    private void AddExtraConnections(float chance = 0.2f)
    {
        Vector2Int[] directions =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (RoomInstance room in generatedRooms.Values)
        {
            foreach (Vector2Int direction in directions)
            {
                Vector2Int neighbourPosition = room.gridPosition + direction;

                // Check that a neighbour exists in that direction.
                if (!generatedRooms.ContainsKey(neighbourPosition))
                {
                    continue;
                }

                RoomInstance neighbour = generatedRooms[neighbourPosition];

                // If they are already connected, skip.
                if (room.connectedRooms.Contains(neighbourPosition))
                {
                    continue;
                }

                // Probability of making an extra connection.
                if (Random.value > chance)
                {
                    continue;
                }

                // Avoid Upgrade, Boss and FloorChange rooms.
                if (room.roomType == RoomType.Upgrade || neighbour.roomType == RoomType.Upgrade)
                {
                    continue;
                }

                if (room.roomType == RoomType.Boss || neighbour.roomType == RoomType.Boss)
                {
                    continue;
                }

                if (room.roomType == RoomType.FloorChange || neighbour.roomType == RoomType.FloorChange)
                {
                    continue;
                }

                room.connectedRooms.Add(neighbourPosition);
                neighbour.connectedRooms.Add(room.gridPosition);
            }
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

        // Upgrade room object initialization.
        if (room.roomType == RoomType.Upgrade)
        {
            UpgradeObject upgradeObject = roomObject.GetComponentInChildren<UpgradeObject>();
            if (upgradeObject != null && upgradeList != null)
            {
                upgradeObject.Initialize(upgradeList.GetRandomUpgrade());
            }
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
            else if (roomType == RoomType.Upgrade && preset.canBeUpgrade)
            {
                validPresets.Add(preset);
            }
            else if (roomType == RoomType.Health && preset.canBeHealth)
            {
                validPresets.Add(preset);
            }
            else if (roomType == RoomType.FloorChange && preset.canBeFloorChange)
            {
                validPresets.Add(preset);
            }
            else if (roomType == RoomType.Normal && preset.canBeNormal)
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

        if (roomType == RoomType.Normal && roomStack.Count > 0)
        {
            Vector2Int lastPos = roomStack.Peek();
            int lastDifficulty = generatedRooms[lastPos].roomPreset.difficultyRating;

            // Count how many times this difficulty repeats in a row.
            int streak = 1;

            if (roomStack.Count > 1)
            {
                Vector2Int[] stackArray = roomStack.ToArray();
                Vector2Int previousPosition = stackArray[1];
                int previousDifficulty = generatedRooms[previousPosition].roomPreset.difficultyRating;

                if (previousDifficulty == lastDifficulty)
                {
                    streak++;
                }
            }

            // If two consecutive difficulties are the same, forbid a third.
            if (streak >= 2)
            {
                foreach (RoomPreset candidate in filteredPresets)
                {
                    if (candidate.difficultyRating != lastDifficulty)
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
