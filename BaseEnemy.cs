using UnityEngine;

public abstract class BaseEnemy : MonoBehaviour
{
    #region Public Attributes
    [Header("Base Enemy Attributes")]
    [SerializeField] private int enemyAttackStrength;
    #endregion

    #region Protected Variables
    protected ObjectSoundEffects enemySoundEffects;
    protected Transform attackTarget;
    protected EnemyHealth enemyHealth;
    #endregion

    private static bool _isAttackingPlayerOne = true;

    protected virtual void Start ()
    {
        GetAttackTargetTransform();

        enemySoundEffects = GetComponent<ObjectSoundEffects>();
        enemyHealth = GetComponent<EnemyHealth>();
	}

    protected abstract void EnemyMovement();

    // Enemy Collides with player, deals damage.
    protected virtual void OnCollisionEnter2D(Collision2D col)
    {
        PlayerHealth player = col.gameObject.GetComponent<PlayerHealth>();

        if (player)
        {
            player.ApplyDamageToTarget(enemyAttackStrength);
        }
    }

    // Enemy is destroyed with any contact with the frisbee.
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Frisbee>())
        {
            enemyHealth.ApplyDamageToTarget(Frisbee.FrisbeeDamage);
        }
    }

    /// <summary>
    /// Permanently selects player1 or player2 as a target for attack.
    /// </summary>
    private void GetAttackTargetTransform()
    {
        attackTarget = (_isAttackingPlayerOne) ? GameObject.FindGameObjectWithTag("BrotherOne").GetComponent<Transform>() : GameObject.FindGameObjectWithTag("BrotherTwo").GetComponent<Transform>();

        _isAttackingPlayerOne = !_isAttackingPlayerOne;
    }
}