using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "ScriptablecObjects/DialogueData")]
public class DialogueData : ScriptableObject
{
    [TextArea] public string[] lines;
    public string characterName;
    public float textDelay;
}
