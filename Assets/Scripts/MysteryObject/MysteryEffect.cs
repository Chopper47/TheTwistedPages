using UnityEngine;

public abstract class MysteryEffect : ScriptableObject
{
    public string effectMessage;

    public abstract void Apply(GameObject player);
}
