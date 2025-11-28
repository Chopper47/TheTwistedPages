using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class Stager : MonoBehaviour
{
    [Header("Nombre de la etapa musical")]
    public string stageName = "Default";

    [Tooltip("Cada instrumento o capa que compone esta versión de la canción")]
    public List<AudioSource> instruments = new List<AudioSource>();

    public void Play(float startTimeNormalized = 0f, float volume = 1f)
    {
        foreach (var src in instruments)
        {
            if (src == null || src.clip == null) continue;

            // Salta al punto correspondiente
            float startTime = Mathf.Clamp(startTimeNormalized * src.clip.length, 0f, src.clip.length - 0.05f);
            src.time = startTime;
            src.volume = volume;
            src.Play();
        }
    }

    public void Stop()
    {
        foreach (var src in instruments)
        {
            if (src == null) continue;
            src.Stop();
        }
    }

    public void SetVolume(float volume)
    {
        foreach (var src in instruments)
        {
            if (src == null) continue;
            src.volume = volume;
        }
    }
}

