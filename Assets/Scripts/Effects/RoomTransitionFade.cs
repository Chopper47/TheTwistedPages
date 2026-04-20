using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoomTransitionFade : MonoBehaviour
{
    public static RoomTransitionFade Instance;

    [SerializeField] private Image fadeImage;
    public float fadeDuration = 0.4f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        fadeImage = GetComponentInChildren<Image>();
    }

    public IEnumerator FadeOut(float duration)
    {
        float timer = 0f;
        Color colour = fadeImage.color;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            colour.a = timer / duration;
            fadeImage.color = colour;
            yield return null;
        }

        colour.a = 1;
        fadeImage.color = colour;
    }

    public IEnumerator FadeIn(float duration)
    {
        float timer = duration;
        Color colour = fadeImage.color;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            colour.a = timer / duration;
            fadeImage.color = colour;
            yield return null;
        }

        colour.a = 0;
        fadeImage.color = colour;
    }
}
