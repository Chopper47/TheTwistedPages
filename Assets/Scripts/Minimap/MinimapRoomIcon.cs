using UnityEngine;
using UnityEngine.UI;

public class MinimapRoomIcon : MonoBehaviour
{
    public Image iconImage;

    private bool visited = false;
    private RoomInstance roomInstance;

    [Header("Colours")]
    public Color normalColor = Color.grey;
    public Color startColor = Color.green;
    public Color healthColor = Color.cyan;
    public Color bossColor = Color.red;
    public Color floorChangeColor = Color.yellow;
    public Color currentRoomColor = Color.white;
    [SerializeField] private float notVisitedOpacity = 0.3f;

    public void SetRoomColour(RoomInstance room)
    {
        roomInstance = room;

        switch (room.roomType)
        {
            case RoomType.Start: 
                iconImage.color = startColor; 
                break;
            case RoomType.Health: 
                iconImage.color = healthColor; 
                break;
            case RoomType.Boss: 
                iconImage.color = bossColor; 
                break;
            case RoomType.FloorChange: 
                iconImage.color = floorChangeColor; 
                break;
            default: 
                iconImage.color = normalColor; 
                break;
        }

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
            iconImage.color = currentRoomColor;
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
}
