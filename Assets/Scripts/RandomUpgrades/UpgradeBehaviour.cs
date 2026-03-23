using UnityEngine;

public abstract class UpgradeBehaviour : ScriptableObject
{
    public abstract void OnAcquire(PlayerUpgrades player);
    public abstract void OnUse(PlayerUpgrades player);
    public abstract void OnRemove(PlayerUpgrades player);
}
