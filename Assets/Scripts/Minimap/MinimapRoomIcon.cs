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
    }

    public bool IsVisited()
    {
        return visited;
    }
}
