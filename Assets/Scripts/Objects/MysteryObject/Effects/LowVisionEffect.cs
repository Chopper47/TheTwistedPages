using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(menuName = "Effects/LowVision")]
public class LowVisionEffect : MysteryEffect
{
    public float duration = 30.0f;

    public float targetIntensity = 1.0f;
    public float targetSmoothness = 1.0f;

    public float transitionTime = 0.5f;

    public float targetPriority = 100.0f;

    public override void Apply(GameObject player)
    {
        MonoBehaviour runner = player.GetComponent<MonoBehaviour>();
        if (!runner)
        {
            return;
        }

        runner.StartCoroutine(Run(player.transform));
    }

    private IEnumerator Run(Transform player)
    {
        Camera camera = CameraController.Instance.GetComponentInChildren<Camera>();

        GameObject go = new GameObject("LowVisionVolume");
        Volume volume = go.AddComponent<Volume>();

        volume.isGlobal = true;
        volume.priority = targetPriority;
        volume.weight = 1.0f;

        volume.profile = CreateInstance<VolumeProfile>();

        var vignette = volume.profile.Add<Vignette>(true);

        Vector2 initialCenter = GetScreenCenter(player, camera);

        vignette.intensity.Override(0f);
        vignette.smoothness.Override(0.2f);
        vignette.center.Override(initialCenter);

        float t = 0f;

        while (t < transitionTime)
        {
            float lerp = t / transitionTime;

            vignette.intensity.value = Mathf.Lerp(0f, targetIntensity, lerp);
            vignette.smoothness.value = Mathf.Lerp(0.2f, targetSmoothness, lerp);

            t += Time.deltaTime;
            yield return null;
        }

        float timer = 0f;

        while (timer < duration)
        {
            Vector2 target = GetScreenCenter(player, camera);

            vignette.center.value = Vector2.Lerp(vignette.center.value, target, Time.deltaTime * 6.0f);

            timer += Time.deltaTime;
            yield return null;
        }

        t = 0f;

        Vector2 finalCenter = vignette.center.value;
        Vector2 screenCenter = new Vector2(0.5f, 0.5f);

        while (t < transitionTime)
        {
            float lerp = t / transitionTime;

            vignette.intensity.value = Mathf.Lerp(targetIntensity, 0f, lerp);
            vignette.smoothness.value = Mathf.Lerp(targetSmoothness, 0.2f, lerp);
            vignette.center.value = Vector2.Lerp(finalCenter, screenCenter, lerp);

            t += Time.deltaTime;
            yield return null;
        }

        Destroy(go);
    }

    private Vector2 GetScreenCenter(Transform player, Camera cam)
    {
        Vector3 screenPos = cam.WorldToScreenPoint(player.position);

        return new Vector2(screenPos.x / Screen.width, screenPos.y / Screen.height);
    }
}
