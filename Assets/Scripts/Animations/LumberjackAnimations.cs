using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class LumberjackAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Lumberjack boss;
    [SerializeField] private GameObject visualAxe;
    private int swipeVariation = 0;

    private void Awake()
    {
        if(boss == null)
        {
            boss = FindAnyObjectByType<Lumberjack>();
        }
    }

    public void MovingTrigger()
    {
        animator.SetTrigger("moving");
    }

    public void ResetMovingTrigger()
    {
        animator.ResetTrigger("moving");
    }

    public void StopMovingTrigger()
    {
        animator.SetTrigger("endMoving");
    }

    public void ResetStopMovingTrigger()
    {
        animator.ResetTrigger("endMoving");
    }

    public void StartThrowAttackBTrigger()
    {
        animator.SetInteger("variation", 0);
        animator.SetTrigger("throwAttack");
    }

    public void SetLowerHp(bool newLowerHp)
    {
        animator.SetBool("lowerHp", newLowerHp);
    }

    public void StartThrowAttackATrigger()
    {
        animator.SetInteger("variation", 1);
        animator.SetTrigger("throwAttack");
    }

    public void GetAxe()
    {
        animator.SetTrigger("getAxe");
    }

    public void ResetGetAxe()
    {
        animator.ResetTrigger("getAxe");
    }

    public void EndThrowAttackTrigger()
    {
        animator.ResetTrigger("throwAttack");
    }

    public void JumpingTrigger()
    {
        animator.SetTrigger("jump");
    }

    public void StopJumpingTrigger()
    {
        animator.ResetTrigger("jump");
    }
    
    public void FallingTrigger()
    {
        animator.SetTrigger("fall");
    }

    public void StopFalling()
    {
        animator.SetTrigger("endFall");
    }

    public void StartSwipeAttackTrigger()
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
        animator.SetTrigger("swipe");
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
        animator.ResetTrigger("swipe");
        boss.EndAttack();
    }

    public void CloseToAxeTrigger()
    {
        animator.SetTrigger("getAxe");
    }

    public void EndCloseToAxe()
    {
        animator.ResetTrigger("getAxe");
        boss.EndAttack();
    }

    public void StartAreaAttackATrigger()
    {
        animator.SetInteger("variation", 0);
        animator.SetTrigger("areaAttack");
    }
    public void StartAreaAttackBTrigger()
    {
        animator.SetInteger("variation", 1);
        animator.SetTrigger("areaAttack");
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

    public void EndAreaAttackTrigger()
    {
        animator.SetTrigger("endAreaAttack");
        boss.EndAttack();
    }

    public void DisableVisualAxe()
    {
        visualAxe.SetActive(false);
    }

    public void EnableVisualAxe() 
    {
        if (boss.hasAxe)
        {
            visualAxe.SetActive(true);
        }
    }

    public void EnragedTrigger()
    {
        animator.SetTrigger("enraged");
    }
    public void ResetEndAreaAttack()
    {
        animator.ResetTrigger("endAreaAttack");
    }
    public void ResetEnragedTrigger()
    {
        animator.ResetTrigger("enraged");
    }

    public void EndEnragedTrigger()
    {
        animator.SetTrigger("endEnraged");
    }

    public void StunnedTrigger()
    {
        animator.SetTrigger("stunned");
    }

    public void EndStunnedTrigger()
    {
        animator.SetTrigger("endStunned");
    }

    public void SentToWallTrigger()
    {
        animator.SetTrigger("sentToWall");
    }
    public void CloseInAxeTrigger()
    {
        animator.SetTrigger("closeInToAxe");
    }

    public void DeathTrigger()
    {
        animator.SetTrigger("death");
    }
}
