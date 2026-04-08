using UnityEngine;

public abstract class MysteryEffect : ScriptableObject
{
    public string effectName;

    public abstract void Apply(GameObject player);
}
