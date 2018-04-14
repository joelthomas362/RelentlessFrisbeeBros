using UnityEngine;

public class EnemySpawnPoint : MonoBehaviour
{
    [Header("Enemy Prefab")]
    public BaseEnemy enemyToSpawn;
    [Header("Spawner Attributes")]
    public float timeBetweenSpawns;
    public int amountOfEnemiesToSpawn;
    public bool isSpawning;

    private float _timer;
    private int _totalEnemiesSpawned;

	void Start ()
    {
        _timer = 0;
        _totalEnemiesSpawned = 0;
	}

	void Update ()
    {
        if (isSpawning)
        {
            SpawnEnemyOnTimer();
        }
	}

    void SpawnEnemyOnTimer()
    {
        _timer += Time.deltaTime;

        if (_timer >= timeBetweenSpawns)
        {
            SpawnEnemy();
            _timer = 0f;
        }
    }

    void SpawnEnemy()
    {
        Instantiate(enemyToSpawn, transform.position, Quaternion.identity);
        _totalEnemiesSpawned++;

        // If amountOfEnemiesToSpawn is 0, spawn enemies until further notice.
        if (amountOfEnemiesToSpawn != 0 && _totalEnemiesSpawned >= amountOfEnemiesToSpawn)
        {
            Destroy(gameObject);
        }
    }

    // Activate enemy spawner.
    void OnTriggerEnter2D(Collider2D trigger)
    {
        if(!isSpawning && trigger.GetComponentInParent<BasePlayerController>())
        {
            isSpawning = true;
        }
    }
}
