using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class BossEnemy : BaseEnemy
{
    [SerializeField] private float attackCooldown;
    [SerializeField] private GameObject bossAttack;
    [SerializeField] private GameObject bossDoor;
    [SerializeField] private AudioMixer gameMusic;
    [SerializeField] private int bossAttackAmount;

    private bool _isAttacking;
    private bool _playerOneInZone;
    private bool _playerTwoInZone;


    protected override void Start()
    {
        base.Start();

        _isAttacking = false;
        _playerOneInZone = false;
        _playerTwoInZone = false;

        bossAttackAmount = 3;

        bossDoor.SetActive(false);
    }
	
    IEnumerator BossAttackRoutine()
    {
        if (_isAttacking)
            yield break;

        // Close door to boss arena and queue boss music.
        _isAttacking = true;
        bossDoor.SetActive(true);
        gameMusic.FindSnapshot("BossThemeShot").TransitionTo(1f);

        // Pan camera to boss view.
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraMovement>().SetBossView(new Vector3(128, -8.1f, -10), 22f);

        // Attack players.
        while(true)
        {
            yield return new WaitForSeconds(attackCooldown);

            BossAttack();
        }
    }


    protected override void EnemyMovement()
    {

    }

    void BossAttack()
    {
        for(int i = 0; i < bossAttackAmount; i++)
        {
            Instantiate(bossAttack, transform.position, Quaternion.identity);
        }
    }

    protected override void OnCollisionEnter2D(Collision2D col)
    {
        base.OnCollisionEnter2D(col);

        if(col.gameObject.GetComponent<Frisbee>())
        {
            enemyHealth.ApplyDamageToTarget(Frisbee.FrisbeeDamage);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        // Checks to make sure both players are in trigger zone before activating boss routine.
        if(!_isAttacking)
        {
            if (other.transform.parent.CompareTag("BrotherOne"))
            {
                _playerOneInZone = true;
            }
            else if (other.transform.parent.CompareTag("BrotherTwo"))
            {
                _playerTwoInZone = true;
            }
        }
        
        if(_playerOneInZone && _playerTwoInZone && !_isAttacking)
        {
            StartCoroutine(BossAttackRoutine());
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!_isAttacking)
        {
            if (other.transform.parent.CompareTag("BrotherOne"))
            {
                _playerOneInZone = false;
            }
            if (other.transform.parent.CompareTag("BrotherTwo"))
            {
                _playerTwoInZone = false;
            }
        }
    }
}
