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

    public override void Apply(GameObject player)
    {
        MonoBehaviour runner = player.GetComponent<MonoBehaviour>();

        Volume volume = FindFirstObjectByType<Volume>();

        if (volume == null)
        {
            return;
        }

        if (!volume.profile.TryGet(out Vignette vignette))
        {
            return;
        }

        runner.StartCoroutine(VignetteRoutine(vignette));
    }

    private IEnumerator VignetteRoutine(Vignette vignette)
    {
        float startIntensity = vignette.intensity.value;
        float startSmoothness = vignette.smoothness.value;

        float timer = 0f;

        while (timer < 0.5f)
        {
            vignette.intensity.value = Mathf.Lerp(startIntensity, targetIntensity, timer / 0.5f);
            vignette.smoothness.value = Mathf.Lerp(startSmoothness, targetSmoothness, timer / 0.5f);

            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(duration);

        timer = 0f;

        while (timer < 0.5f)
        {
            vignette.intensity.value = Mathf.Lerp(targetIntensity, 0.45f, timer / 0.5f);
            vignette.smoothness.value = Mathf.Lerp(targetSmoothness, 0.2f, timer / 0.5f);

            timer += Time.deltaTime;
            yield return null;
        }
    }
}
