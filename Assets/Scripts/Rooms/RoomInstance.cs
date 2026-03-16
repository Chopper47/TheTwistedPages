using UnityEngine;

public enum RoomType
{
    Normal,
    Start,
    Upgrade,
    Health,
    FloorChange,
    Boss
}

[System.Serializable]
public class RoomInstance
{
    public Vector2Int gridPosition;
    public RoomType roomType;
    public RoomPreset roomPreset;
    public GameObject roomPrefab;

    public bool visited = false;
    public bool cleared = false;

    public RoomInstance(Vector2Int position, RoomType type, RoomPreset preset)
    {
        gridPosition = position;
        roomType = type;
        roomPreset = preset;
    }
}