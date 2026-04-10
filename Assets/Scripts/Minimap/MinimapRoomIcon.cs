using UnityEngine;
using UnityEngine.UI;

public class MinimapRoomIcon : MonoBehaviour
{
    public Image iconImage;

    private bool visited = false;
    private RoomInstance roomInstance;

    [Header("Sprites")]
    public Sprite normalSprite;
    public Sprite startSprite;
    public Sprite upgradeSprite;
    public Sprite healthSprite;
    public Sprite bossSprite;
    public Sprite floorChangeSprite;

    [Header("Connections")]
    [SerializeField] private GameObject northConnection;
    [SerializeField] private GameObject southConnection;
    [SerializeField] private GameObject eastConnection;
    [SerializeField] private GameObject westConnection;

    private bool northActive, southActive, eastActive, westActive;

    [SerializeField] private float notVisitedOpacity = 0.3f;

    public void SetRoomSprite(RoomInstance room)
    {
        roomInstance = room;

        switch (room.roomType)
        {
            case RoomType.Start:
                iconImage.sprite = startSprite;
                break;
            case RoomType.Upgrade:
                iconImage.sprite = upgradeSprite;
                break;
            case RoomType.Health:
                iconImage.sprite = healthSprite;
                break;
            case RoomType.Boss:
                iconImage.sprite = bossSprite;
                break;
            case RoomType.FloorChange:
                iconImage.sprite = floorChangeSprite;
                break;
            default:
                iconImage.sprite = normalSprite;
                break;
        }

        iconImage.color = Color.white;

        SetOpacity(0f);
    }

    public void UpdateConnections()
    {
        if (roomInstance == null)
        {
            return;
        }

        Vector2Int pos = roomInstance.gridPosition;

        northActive = roomInstance.connectedRooms.Contains(pos + Vector2Int.up);
        southActive = roomInstance.connectedRooms.Contains(pos + Vector2Int.down);
        eastActive = roomInstance.connectedRooms.Contains(pos + Vector2Int.right);
        westActive = roomInstance.connectedRooms.Contains(pos + Vector2Int.left);

        northConnection.SetActive(northActive);
        southConnection.SetActive(southActive);
        eastConnection.SetActive(eastActive);
        westConnection.SetActive(westActive);
    }

    public void SetVisited()
    {
        if (!visited)
        {
            visited = true;
            SetOpacity(notVisitedOpacity);
        }
    }

    public void SetAsCurrent(bool isCurrent)
    {
        if (isCurrent)
        {
            iconImage.color = Color.white;
            SetOpacity(1.0f);
        }
        else if (visited)
        {
            SetOpacity(0.6f);
        }
    }

    private void SetOpacity(float value)
    {
        Color colour = iconImage.color;
        colour.a = value;
        iconImage.color = colour;
    }

    public void SetVisible(bool visible)
    {
        iconImage.enabled = visible;

        northConnection.SetActive(visible && northActive);
        southConnection.SetActive(visible && southActive);
        eastConnection.SetActive(visible && eastActive);
        westConnection.SetActive(visible && westActive);
    }

    public bool IsVisited()
    {
        return visited;
    }
}
