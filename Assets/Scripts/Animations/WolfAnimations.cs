using UnityEngine;

public class WolfAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Start()
    {
        if(animator == null) animator = GetComponent<Animator>();
    }

    public void SetWalking(bool value)
    {
        animator.SetBool("walking", value);
    }
    public void SetAttacking(bool value)
    {
        animator.SetBool("attacking", value);
    }
    public void SetStepback(bool value)
    {
        animator.SetBool("stepBack", value);
    }
    public void SetHowling(bool value)
    {
        animator.SetBool("howling", value);
    }
}
