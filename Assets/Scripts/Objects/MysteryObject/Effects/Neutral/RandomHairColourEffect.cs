using UnityEngine;

[CreateAssetMenu(menuName = "Effects/RandomHairColour")]
public class RandomHairColourEffect : MysteryEffect
{
    public Material baseMaterial;

    public override void Apply(GameObject player)
    {
        GameObject hair = GameObject.FindGameObjectWithTag("Hair");
        SkinnedMeshRenderer skinnedMesh = hair.GetComponent<SkinnedMeshRenderer>();

        if (hair == null || skinnedMesh == null || baseMaterial == null)
        {
            return;
        }

        skinnedMesh.material = baseMaterial;

        Color randomColor = Random.ColorHSV(0f, 1.0f, 0.6f, 1.0f, 0.6f, 1.0f);

        skinnedMesh.material.color = randomColor;
    }
}
