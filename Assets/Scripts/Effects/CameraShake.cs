using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;
public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    public float defaultDuration;
    public float defaultStrength;


    private Tween zoomTween;
    private void Awake()
    {
        instance = this;
    }

    private void OnDashZoom(float zoomAmount, float zoomDuration)
    {
        Camera cam = transform.GetComponentInChildren<Camera>();

        zoomTween = DOTween.Sequence();

        float originalFOV = cam.fieldOfView;
        float targetFOV = originalFOV - zoomAmount;

        zoomTween = DOTween.Sequence()
        .Append(cam.DOFieldOfView(targetFOV, zoomDuration * 0.5f)) // zoom in fast
        .Append(cam.DOFieldOfView(originalFOV, zoomDuration * 0.5f)) // return quickly
        .SetEase(Ease.OutQuad);
    }

    public static void DoDashZoom(float zoomAmount, float zoomDuration)
    {
        instance.OnDashZoom(zoomAmount, zoomDuration);
    }

    private void OnShake(float duration, float strength)
    {
        transform.DOKill();
        Vector3 shakeVector = new Vector3(strength, strength, 0);
        transform.DOShakePosition(duration, shakeVector);
        transform.DOShakeRotation(duration, shakeVector);
    }

    private void OnShake(float duration, float strength, Vector3 dashDirection)
    {
        transform.DOKill();
        Vector3 shakeVector = dashDirection.normalized * strength;

        transform.DOShakePosition(duration, shakeVector);

        transform.DOShakeRotation(duration, shakeVector * 0.5f);
    }

    public static void Shake(float duration, float strength)
    {
        instance.OnShake(duration, strength);
    }

    public static void Shake(float duration, float strength, Vector3 dashDirection)
    {
        instance.OnShake(duration, strength, dashDirection);
    }
    public void StopShake()
    {
        // Kills all tweens on this transform (position and rotation shakes)
        transform.DOKill();
    }

    // Static version for convenience
    public static void Stop()
    {
        instance.StopShake();
    }
}
