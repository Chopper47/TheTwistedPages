using UnityEngine;

public class WolfAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem walk_FR;
    [SerializeField] private ParticleSystem walk_FL;
    [SerializeField] private ParticleSystem walk_BR;
    [SerializeField] private ParticleSystem walk_BL;

    private void Start()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    public void SetWalk()
    {
        animator.SetBool("walk", true);
    }

    public void StopWalk()
    {
        animator.SetBool("walk", false);
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
        animator.SetTrigger("stepback");
    }

    public void ResetTriggerStepback()
    {
        animator.ResetTrigger("stepback");
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

    public void Play_FrontRight()
    {
        walk_FR.Play();
    }

    public void Play_FrontLeft()
    {
        walk_FL.Play();
    }

    public void Play_BackRight()
    {
        walk_BR.Play();
    }

    public void Play_BackLeft()
    {
        walk_BL.Play();
    }
}
