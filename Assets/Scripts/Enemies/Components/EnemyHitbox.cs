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
        return states.CurrentState();
    }

    public void SetIgnore(bool newIgnorePlayer)
    {
        ignorePlayer = newIgnorePlayer;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(ignorePlayer) { return; }
        PlayerHealth player = other.GetComponent<PlayerHealth>();

        if(player != null && !PlayerMovement.IsInvincible()) 
        {
            CameraShake.Shake(0.5f, 0.5f);
            player.TakeDamage(dmg);
        }

        PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();

        if(playerMovement != null && playerMovement.CanPush(this.gameObject) && pushable)
        {
            Pushed(playerMovement.PushTarget());
            SetIgnore(true);
            states.PushedState();
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
        states.StayState();
        yield return null;
    }
}
