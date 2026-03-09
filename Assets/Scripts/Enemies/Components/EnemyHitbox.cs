using System.Collections;
using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    [SerializeField] EnemyHealth enemyHp;
    [SerializeField] EnemyState states;
    [SerializeField] int dmg;
    [SerializeField] bool pushable = true;

    private bool ignorePlayer = false;
    private void Awake()
    {
        if(enemyHp == null) { enemyHp = GetComponent<EnemyHealth>(); }
        if(states == null) { states = GetComponent<EnemyState>(); }
    }

    public int ShowEnemyState()
    {
        if(states == null)
        {
            return 2;                       //returns attack state
        }
        else
        {
            return states.CurrentState();
        }
    }

    public void SetIgnore(bool newIgnorePlayer)
    {
        ignorePlayer = newIgnorePlayer;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(ignorePlayer) 
        {
            return;
        }
        PlayerHealth player = other.GetComponent<PlayerHealth>();

        PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();

        if(playerMovement != null && playerMovement.CanPush(this.gameObject) && pushable)
        {
            Projectile projectile = GetComponent<Projectile>();
            if (projectile != null)
            {
                Pushed(playerMovement.PushTarget(false, false));
                playerMovement.StopBulletTime();
                projectile.Deflect();
                
            }
            else
            {
                Pushed(playerMovement.PushTarget(true, false));
                playerMovement.StopBulletTime();
            }
            SetIgnore(true);

            if(states != null)
            {
                states.PushedState();
            }
        }

        if(player != null && !PlayerMovement.IsInvincible() && !ignorePlayer) 
        {
            CameraShake.Shake(0.5f, 0.5f);
            player.TakeDamage(dmg);
        }
    }

    public void Pushed(Vector3 trapPos)
    {
        StartCoroutine(LerpPosition(new Vector3(trapPos.x, transform.position.y, trapPos.z)));
    }

    IEnumerator LerpPosition(Vector3 position)
    {
        Vector3 startingPos = transform.position;
        float time = 0f;

        while (time < 1f)
        {
            transform.position = Vector3.Lerp(startingPos, position, time/1f);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        SetIgnore(false);
        if (states != null) 
        {
            states.StayState();
        }
        Projectile projectile = GetComponent<Projectile>();
        if (projectile != null) { Destroy(this.gameObject); }
        yield return null;
    }
}
