using UnityEngine;

public abstract class UpgradeBehaviour : ScriptableObject
{
    public abstract void OnAcquire(PlayerUpgrades player);
    public abstract bool OnUse(PlayerUpgrades player);
    public abstract void OnRemove(PlayerUpgrades player);
}
