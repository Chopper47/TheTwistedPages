using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LumberjackAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Lumberjack boss;
    private int swipeVariation = 0;

    private void Awake()
    {
        if(boss == null)
        {
            boss = FindAnyObjectByType<Lumberjack>();
        }
    }

    public void Moving()
    {
        animator.SetBool("running", true);
    }

    public void StopMoving()
    {
        animator.SetBool("running", false);
    }

    public void StartThrowAttackB()
    {
        animator.SetInteger("variation", 0);
        animator.SetBool("throwAttack", true);
    }

    public void SetLowerHp(bool newLowerHp)
    {
        animator.SetBool("lowerHp", newLowerHp);
    }

    public void StartThrowAttackA()
    {
        animator.SetInteger("variation", 1);
        animator.SetBool("throwAttack", true);
    }

    public void GainAxe()
    {
        animator.SetBool("gainAxe", true);
    }

    public void EndGainAxe()
    {
        animator.SetBool("gainAxe", false);
    }

    public void EndThrowAttack()
    {
        animator.SetBool("throwAttack", false);
    }

    public void Jumping()
    {
        animator.SetBool("jumping", true);
    }

    public void StopJumping()
    {
        animator.SetBool("jumping", false);
    }
    
    public void Falling()
    {
        animator.SetBool("falling", true);
    }

    public void StopFalling()
    {
        animator.SetBool("falling", false);
    }

    public void StartSwipeAttack()
    {
        if(swipeVariation == 0)
        {
            swipeVariation = 1;
        }
        else
        {
            swipeVariation = 0;
        }
        animator.SetInteger("variation", swipeVariation);
        animator.SetBool("basicAttack", true);
    }

    public void SwipeHitbox()
    {
        boss.EnableSwipeHitbox();
    }

    public void EndSwipeHitbox()
    {
        boss.DisableSwipeHitbox();
    }

    public void EndSwipeAttack()
    {
        animator.SetBool("basicAttack", false);
        boss.EndAttack();
    }

    public void CloseToAxe()
    {
        animator.SetBool("closeToAxe", true);
    }

    public void EndCloseToAxe()
    {
        animator.SetBool("closeToAxe", false);
    }

    public void StartAreaAttackA()
    {
        animator.SetInteger("variation", 0);
        animator.SetBool("areaAttack", true);
    }
    public void StartAreaAttackB()
    {
        animator.SetInteger("variation", 1);
        animator.SetBool("areaAttack", true);
    }

    public void AoeHitbox()
    {
        boss.EnableAoeHitbox();
    }

    public void EndAoeHitbox()
    {
        boss.DisableAoeHitbox();
    }

    public void ExpansiveAoeHitbox()
    {
        boss.EnableAoeExpansion();
    }

    public void EndAreaAttack()
    {
        animator.SetBool("areaAttack", false);
        boss.EndAttack();
    }
}
