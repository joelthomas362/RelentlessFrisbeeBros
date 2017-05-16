using UnityEngine;

public class ArmoredEnemy : MeleeEnemy
{
    [Header("Armored Enemy Attributes")]
    [Range(0,5)]
    [SerializeField] private int initialEnemyArmor;

    private int _enemyArmor;
    private GameObject _armorCollider;

    protected override void Start()
    {
        _enemyArmor = initialEnemyArmor;
        _armorCollider = transform.GetChild(1).gameObject;

        base.Start();
    }

    protected override void OnCollisionEnter2D(Collision2D col)
    {
        if(col.gameObject.GetComponent<Frisbee>())
        {
            DamageArmor(Frisbee.FrisbeeDamage);
        }

        // Check for player and deal damage.
        base.OnCollisionEnter2D(col);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if(_enemyArmor <= 0)
        {
            // Take damage if frisbee touches enemy trigger.
            base.OnTriggerEnter2D(other);
        }
    }

    /// <summary>
    /// Frisbee hits enemy, bounces off subtracting from armor amount.
    /// </summary>
    /// <param name="damageToEnemyArmor">Amount of damage to deal to armor.</param>
    private void DamageArmor(int damageToEnemyArmor)
    {
        if(damageToEnemyArmor > 0)
        {
            _enemyArmor -= damageToEnemyArmor;
            enemySprite.color += new Color(.2f, .2f, 0f, 0f);

            if (_enemyArmor <= 0)
            {
                BreakArmor();
            }
        }
        else
        {
            Debug.LogWarning("Damage must be greater than 0.");
        }
    }

    /// <summary>
    /// Remove armor collider and leave enemy vulnerable.
    /// </summary>
    private void BreakArmor()
    {
        _armorCollider.layer = 8;
        enemySprite.color = Color.white;
    }
}
