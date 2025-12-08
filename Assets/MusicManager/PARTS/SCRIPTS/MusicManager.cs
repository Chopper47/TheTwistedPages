using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Lista de Stagers (canciones o variaciones)")]
    public List<Stager> stages = new List<Stager>();

    [Header("Configuración")]
    public int currentStage = 0;
    public float fadeTime = 1f;
    public float volume;
    public static MusicManager instance;
    private Stack<int> stageHistory = new Stack<int>();

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        if (stages.Count == 0)
        {
            Debug.LogWarning("⚠️ No hay stagers configurados en MusicManager.");
            return;
        }

        instance.stages[currentStage].Play(0f, volume);
    }

    // --- CAMBIO SINCRONIZADO (misma canción, diferente variación) ---
    public void SwitchStageSynced(int newIndex)
    {
        if (newIndex == currentStage || newIndex >= stages.Count) return;

        Stager oldStage = stages[currentStage];
        Stager newStage = stages[newIndex];

        if (oldStage.instruments.Count == 0) return;
        AudioSource refSource = oldStage.instruments[0];
        if (refSource == null || refSource.clip == null) return;

        float progress = refSource.time / refSource.clip.length;

        newStage.Play(progress);
        newStage.SetVolume(0f);

        StopAllCoroutines();
        StartCoroutine(FadeBetweenStages(oldStage, newStage));

        currentStage = newIndex;
    }
    public void SwitchToNewSong(int newIndex)
    {
        SwitchToNewSong(newIndex, true);
    }

    public void SwitchToNewSongNoSave(int newIndex)
    {
        SwitchToNewSong(newIndex, false);
    }
    // --- CAMBIO A OTRA CANCIÓN COMPLETA (GameOver, Boss, etc.) ---
    public void SwitchToNewSong(int newIndex, bool savePrevious)
    {
        if (newIndex == currentStage || newIndex >= stages.Count) return;

        if (savePrevious)
            stageHistory.Push(currentStage);

        Stager oldStage = stages[currentStage];
        Stager newStage = stages[newIndex];

        StopAllCoroutines();
        StartCoroutine(FadeOutIn(oldStage, newStage));

        currentStage = newIndex;
    }

    // --- VOLVER A LA CANCIÓN ANTERIOR ---
    public void ReturnToPreviousSong()
    {
        if (stageHistory.Count == 0)
        {
            Debug.Log("No hay canción anterior en el historial.");
            return;
        }

        int previous = stageHistory.Pop();
        SwitchToNewSong(previous, false);
    }

    // --- EFECTOS DE TRANSICIÓN ---
    private IEnumerator FadeBetweenStages(Stager from, Stager to)
    {
        float timer = 0f;
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            float t = timer / fadeTime;
            from.SetVolume(Mathf.Lerp(1f * volume, 0f, t));
            to.SetVolume(Mathf.Lerp(0f, 1f * volume, t));
            yield return null;
        }
        from.SetVolume(0f);
        from.Stop();
        to.SetVolume(1f * volume);
    }

    private IEnumerator FadeOutIn(Stager from, Stager to)
    {
        // Fade out la canción actual
        float timer = 0f;
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            float t = timer / fadeTime;
            from.SetVolume(Mathf.Lerp(1f * volume, 0f, t));
            yield return null;
        }
        from.Stop();

        // Fade in la nueva
        to.Play(0f, volume);
        to.SetVolume(volume);

        timer = 0f;
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            float t = timer / fadeTime;
            to.SetVolume(Mathf.Lerp(0f, 1f * volume, t));
            yield return null;
        }
        to.SetVolume(1f * volume);
    }

    public void SetVolumeOfStagers(float volume)
    {
        foreach(Stager stage in stages)
        {
            stage.SetVolume(volume);
        }
    }
}
