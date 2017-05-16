using UnityEngine;

public class MeleeEnemy : BaseEnemy
{
    [Header("Melee Attributes")]
    [SerializeField] private float meleeMovementSpeed;

    protected SpriteRenderer enemySprite;

    protected override void Start()
    {
        base.Start();

        enemySprite = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update ()
    {
        EnemyMovement();
	}

    protected override void EnemyMovement()
    {
        if(attackTarget)
        {
            Vector3 attackDirection = attackTarget.position - transform.position;
            transform.Translate(attackDirection.normalized * Time.deltaTime * meleeMovementSpeed);
        }
    }
}