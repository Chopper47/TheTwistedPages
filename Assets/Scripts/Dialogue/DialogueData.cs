using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "ScriptableObjects/DialogueData")]
public class DialogueData : ScriptableObject
{
    public string[] lines;
    public string characterName;
    public float textSpeed;
}
