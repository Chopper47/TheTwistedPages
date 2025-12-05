using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script que guarda los datos y diferentes parámetros de cada grupo de audios del juego.
/// </summary>
[System.Serializable]
public class AudioData
{
    public string name; 
    public AudioClip[] clips;
    public float volume = 1.0f;
    //public Vector2 pitchRange = new Vector2(0.75f, 1.25f);
    public Vector2 pitchRange = new Vector2(1f, 1f);
}
