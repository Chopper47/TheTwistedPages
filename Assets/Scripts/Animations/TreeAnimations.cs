using UnityEngine;

public class TreeAnimations : MonoBehaviour
{
    [SerializeField] Animator animator;

    [Header("Particle Systems")]
    [SerializeField] ParticleSystem walk_FR;
    [SerializeField] ParticleSystem walk_FM;
    [SerializeField] ParticleSystem walk_FL;
    [SerializeField] ParticleSystem walk_BR;
    [SerializeField] ParticleSystem walk_BL;

    public void ShakeTrigger()
    {
        animator.SetTrigger("shake");
    }

    public void ResetShakeTrigger()
    {
        animator.ResetTrigger("shake");
    }

    public void EndShakeTrigger()
    {
        animator.SetTrigger("stopShake");
    }

    public void SetWalk()
    {
        animator.SetBool("walk", true);
    }

    public void StopWalk()
    {
        animator.SetBool("walk", false);
    }

    public void DropTrigger()
    {
        animator.SetTrigger("drop");
    }

    public void Play_FrontMiddle()
    {
        walk_FM.Play();
    }

    public void Play_FrontRight()
    {
        walk_FR.Play();
    }

    public void Play_FrontLeft()
    {
        walk_FL.Play();
    }

    public void Play_BackLeft()
    {
        walk_BL.Play();
    }

    public void Play_BackRight()
    {
        walk_BR.Play();
    }
}
