using UnityEngine;

public class BossAttack : MonoBehaviour
{
    public float attackForce;

    private Rigidbody2D _rigidBody;
    private int _hitCount;

	void Start ()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _hitCount = 0;

        _rigidBody.AddForce(Random.insideUnitCircle * attackForce, ForceMode2D.Impulse);

        Destroy(gameObject, 5f);
	}	

    void OnCollisionEnter2D(Collision2D col)
    {
        _hitCount++;

        if(_hitCount >= 5)
        {
            Destroy(gameObject);
        }
    }
}
