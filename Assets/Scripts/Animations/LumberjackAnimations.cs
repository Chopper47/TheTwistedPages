using System.Collections;
using UnityEngine;

public class LumberjackAnimations : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Lumberjack boss;
    [SerializeField] private GameObject visualAxe;
    private int swipeVariation = 0;

    [Header("Particle Systems")]
    [SerializeField] private GameObject AOE_Effect;
    [SerializeField] private Transform AOE_Effect_SpawnPoint;
    [SerializeField] private GameObject Straight_Effect;
    [SerializeField] private Transform Straight_Effect_SpawnPoint;
    private void Awake()
    {
        if (boss == null)
        {
            boss = FindAnyObjectByType<Lumberjack>();
        }
    }

    public void MovingTrigger()
    {
        animator.SetTrigger("moving");
        animator.SetBool("walk", true);
    }

    public void ResetMovingTrigger()
    {
        animator.ResetTrigger("moving");
    }

    public void StopMovingTrigger()
    {

        animator.SetBool("walk", false);
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
        if (swipeVariation == 0)
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

    public void ExplosionParticle()
    {
        GameObject instance = Instantiate(AOE_Effect, AOE_Effect_SpawnPoint.position, AOE_Effect_SpawnPoint.rotation);
        StartCoroutine(DestroyParticle(instance, 1.5f));
    }

    public void StraightParticle()
    {
        GameObject instance = Instantiate(Straight_Effect, Straight_Effect_SpawnPoint.position, Straight_Effect_SpawnPoint.rotation);
        StartCoroutine(DestroyParticle(instance, 1f));
    }

    IEnumerator DestroyParticle(GameObject particle, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(particle);
    }
}
