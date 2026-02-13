using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapManager : MonoBehaviour
{
    [Header("References")]
    public RectTransform minimapContainer;
    public GameObject roomIconPrefab;
    public float cellSizeX = 75.0f;
    public float cellSizeY = 50.0f;

    private Dictionary<Vector2Int, MinimapRoomIcon> icons = new Dictionary<Vector2Int, MinimapRoomIcon>();

    private Dictionary<Vector2Int, RoomInstance> rooms;

    private Vector2Int currentRoom;

    public void InitializeMinimap(Dictionary<Vector2Int, RoomInstance> generatedRooms)
    {
        foreach (Transform child in minimapContainer)
        {
            Destroy(child.gameObject);
        }

        icons.Clear();

        rooms = generatedRooms;

        foreach (KeyValuePair<Vector2Int, RoomInstance> pair in generatedRooms)
        {
            Vector2Int gridPosition = pair.Key;
            RoomInstance room = pair.Value;

            GameObject iconObject = Instantiate(roomIconPrefab, minimapContainer);
            MinimapRoomIcon icon = iconObject.GetComponent<MinimapRoomIcon>();

            icon.SetRoomColour(room);

            icons.Add(gridPosition, icon);
        }

        currentRoom = Vector2Int.zero;
    }

    public void SetCurrentRoom(Vector2Int newRoomPosition)
    {
        currentRoom = newRoomPosition;

        foreach (KeyValuePair<Vector2Int, MinimapRoomIcon> pair in icons)
        {
            Vector2Int roomPosition = pair.Key;
            MinimapRoomIcon icon = pair.Value;

            Vector2Int offset = new Vector2Int(roomPosition.x - currentRoom.x, roomPosition.y - currentRoom.y);

            icon.transform.localPosition = new Vector3(offset.x * cellSizeX, offset.y * cellSizeY, 0f);

            icon.SetVisited();

            if (roomPosition.x == currentRoom.x && roomPosition.y == currentRoom.y)
            {
                icon.SetAsCurrent(true);
            }
            else
            {
                icon.SetAsCurrent(false);
            }
        }
    }
}
