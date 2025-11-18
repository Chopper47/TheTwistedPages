using UnityEngine;

public class ConditionHasApples : ICondition
{
    [SerializeField] private Tree tree;

    private void Awake()
    {
        if (!tree)
        {
            tree = transform.root.GetComponent<Tree>();
        }
    }

    public override bool Check()
    {
        return tree.HasApplesLeft();
    }
}
