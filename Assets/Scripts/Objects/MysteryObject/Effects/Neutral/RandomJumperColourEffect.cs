using UnityEngine;

[CreateAssetMenu(menuName = "Effects/RandomJumperColour")]
public class RandomJumperColourEffect : MysteryEffect
{
    public Material baseMaterial;

    public override void Apply(GameObject player)
    {
        GameObject jumper = GameObject.FindGameObjectWithTag("Jumper");
        SkinnedMeshRenderer skinnedMesh = jumper.GetComponent<SkinnedMeshRenderer>();

        if (jumper == null || skinnedMesh == null || baseMaterial == null)
        {
            return;
        }

        skinnedMesh.material = baseMaterial;

        Color randomColor = Random.ColorHSV(0f, 1.0f,0.6f, 1.0f,0.6f, 1.0f);

        skinnedMesh.material.color = randomColor;
    }
}
