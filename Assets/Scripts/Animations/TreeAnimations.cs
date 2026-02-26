using UnityEngine;

public class TreeAnimations : MonoBehaviour
{
    [SerializeField] Animator animator;

    public void ShakeTrigger()
    {
        animator.SetTrigger("shake");
    }

    public void EndShakeTrigger()
    {
        animator.SetTrigger("stopShake");
    }

    public void WalkTrigger()
    {
        animator.SetTrigger("walk");
    }
    
    public void ResetWalkTrigger()
    {
        animator.ResetTrigger("walk");
    }

    public void StopWalkTrigger()
    {
        animator.SetTrigger("stopWalking");
    }

    public void DropTrigger()
    {
        animator.SetTrigger("drop");
    }
}
