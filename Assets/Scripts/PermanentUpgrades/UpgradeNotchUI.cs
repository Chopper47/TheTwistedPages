using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UpgradeNotchUI : MonoBehaviour
{
    [SerializeField] private List<Image> notches;
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite inactiveSprite;

    public void SetLevel(int level)
    {
        for (int i = 0; i < notches.Count; i++)
        {
            if (i < level)
            {
                notches[i].sprite = activeSprite;
            }
            else
            {
                notches[i].sprite = inactiveSprite;
            }
        }
    }
}
