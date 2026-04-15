using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Script que controla la reproducción de sonidos en el juego.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource defaultSource;

    [Header("Sound List")]
    public List<AudioData> sounds;

    private Dictionary<string, AudioData> soundDictionary;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Se llena el diccionario con los sonidos.
        soundDictionary = new Dictionary<string, AudioData>();
        foreach (AudioData sound in sounds)
        {
            if (!soundDictionary.ContainsKey(sound.name))
            {
                soundDictionary.Add(sound.name, sound);
            }
        }
    }

    private void Start()
    {
        int volume = PlayerPrefs.GetInt("AudioVolume", 1);
        Instance.defaultSource.volume = volume;
    }
    /// <summary>
    /// Método que reproduce un sonido por su nombre, pudiéndole pasar un Audiosource.
    /// </summary>
    public void Play(string name, AudioSource source)
    {
        // Se verifica que el sonido exista.
        if (!soundDictionary.ContainsKey(name))
        {
            Debug.LogWarning("Sonido no encontrado: " + name);
            return;
        }

        AudioData data = soundDictionary[name];

        // Se selecciona un clip aleatorio del array y se genera un pitch aleatorio.
        AudioClip clip = data.clips[Random.Range(0, data.clips.Length)];
        //float pitch = Random.Range(data.pitchRange.x, data.pitchRange.y);

        if (source == null)
        {
            source = defaultSource;
        }

       // source.pitch = pitch;
        source.PlayOneShot(clip, data.volume);
    }

    /// <summary>
    /// Método que reproduce un sonido en cualquier lugar del mundo que se le especifique, creando un AudioSource temporal.
    /// </summary>
    public void PlayAtPosition(string name, Vector3 position)
    {
        // Se verifica que el sonido exista.
        if (!soundDictionary.ContainsKey(name))
        {
            return;
        }

        AudioData data = soundDictionary[name];

        AudioClip clip = data.clips[Random.Range(0, data.clips.Length)];
        float pitch = Random.Range(data.pitchRange.x, data.pitchRange.y);

        // Se crea un objeto temporal con un AudioSource.
        GameObject temporaryAudioSource = new GameObject($"TemporaryAudioSource{name}");
        temporaryAudioSource.transform.position = position;

        AudioSource audioSource = temporaryAudioSource.AddComponent<AudioSource>();
        audioSource.spatialBlend = 1.0f;
        audioSource.pitch = pitch;
        audioSource.PlayOneShot(clip, data.volume);

        // Se destruye el objeto una vez que termina el clip.
        Destroy(temporaryAudioSource, clip.length / pitch);
    }
}
