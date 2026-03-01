using UnityEngine;

public class WolfAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Start()
    {
        if(animator == null) animator = GetComponent<Animator>();
    }
 
    public void TriggerWalk()
    {
        animator.SetTrigger("walk");
    }

    public void ResetTriggerWalk()
    {
        animator.ResetTrigger("walk");
    }

    public void TriggerStopWalk()
    {
        animator.SetTrigger("stopWalk");
    }

    public void TriggerAttack()
    {
        animator.SetTrigger("attack");
    }
    public void TriggerStopAttack()
    {
        animator.SetTrigger("stopAttack");
    }
    public void TriggerStepback()
    {
        animator.SetTrigger("stepBack");
    }

    public void TriggerStopStepback()
    {
        animator.SetTrigger("stopStepback");
    }

    public void TriggerHowl()
    {
        animator.SetTrigger("howl");
    }

    public void TriggerStopHowl()
    {
        animator.SetTrigger("stopHowl");
    }
}
