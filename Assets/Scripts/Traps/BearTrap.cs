using System.Collections;
using UnityEngine;

public class BearTrap : Trap
{
    [SerializeField] private float destroyDelay = 1.5f;
    [SerializeField] private Animator animator;
    public AudioSource sound;
    public AudioManager AM;
    private void Awake()
    {
        AM= FindFirstObjectByType<AudioManager>();
    }
    protected override void AfterActivation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Activate");
            //sound.Play();
            AM.Play("BearTrap",null);
        }

        StartCoroutine(DestroyCountdown());
    }

    private IEnumerator DestroyCountdown()
    {
        yield return new WaitForSeconds(destroyDelay);
        Destroy(gameObject);
    }
}