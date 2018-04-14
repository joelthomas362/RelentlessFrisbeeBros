using UnityEngine;
using System.Collections;

public class Frisbee : MonoBehaviour
{
    [Header("Frisbee Audio")]
    [SerializeField] private AudioClip frisbeeReset;
    [SerializeField] private AudioClip frisbeePowerUpNoise;
    public const int FrisbeeDamage = 1;
    public bool canBeReset;

    private GameObject _frisbeeSpawnParticles;
    private Rigidbody2D _frisbeeRB;
    private ObjectSoundEffects _frisbeeSFX;
    private float _frisbeeForce;
    private Vector3 _resetPoint;
    private Transform _playerOneTransform;
    private Transform _playerTwoTransform;
    private bool _checkingFrisbee;

    void Start()
    {
        canBeReset = false;
        _checkingFrisbee = false;

        _frisbeeForce = 1;
        _frisbeeRB = GetComponent<Rigidbody2D>();

        _frisbeeSpawnParticles = (GameObject)Resources.Load("P_FrisbeeSpawn") as GameObject;
        _frisbeeSFX = GetComponent<ObjectSoundEffects>();

        _playerOneTransform = GameObject.FindGameObjectWithTag("BrotherOne").transform;
        _playerTwoTransform = GameObject.FindGameObjectWithTag("BrotherTwo").transform;

        MoveToPointBetweenPlayers();

        StartCoroutine(FrisbeeResetCheck());
    }

    void OnEnable()
    {
        BasePlayerController.EventActivatePowerMode += StopFrisbee;
        BasePlayerController.EventCancelPowerMode += ResetFrisbee;
    }

    void OnDisable()
    {
        BasePlayerController.EventActivatePowerMode -= StopFrisbee;
        BasePlayerController.EventCancelPowerMode -= ResetFrisbee;
    }

    /// <summary>
    /// Increases the speed of the frisbee when thrown.
    /// </summary>
    public void IncreaseFrisbeeForce()
    {
        if(_frisbeeForce < 3f)
        {
            _frisbeeForce++;
        }
    }

    public float GetFrisbeeForce()
    {
        return _frisbeeForce;
    }

    /// <summary>
    /// Resets frisbee speed modifier to 1.0f.
    /// </summary>
    public void ResetFrisbeeForce()
    {
        _frisbeeForce = 1f;
    }

    /// <summary>
    /// Control the frisbee with combined inputs from controllers.
    /// </summary>
    /// <param name="horizontalInput">Horizontal input axis.</param>
    /// <param name="verticalInput">Vertical Input axis.</param>
    /// <param name="frisbeeMoveSpeed">Strength at which to move the frisbee.</param>
    public void ControlFrisbeeDirectly(float horizontalInput, float verticalInput, float frisbeeMoveSpeed)
    {
        // Control frisbee via player input.
        Vector3 forceDirection = new Vector3(horizontalInput, verticalInput, 0f);
        transform.position += forceDirection * Time.deltaTime * frisbeeMoveSpeed;

        // Check for valid circle cast on frisbee each frame frisbee is controlled...
        RaycastHit2D hitInfo = Physics2D.CircleCast(transform.position, .27f, Vector2.zero);
        if(hitInfo)
        {
            // ...if we hit a valid enemy -- Destroy them
            EnemyHealth hitEnemy = hitInfo.transform.GetComponent <EnemyHealth>();
            if(hitEnemy && hitEnemy.gameObject.layer != 11)
            {
                hitEnemy.ApplyDamageToTarget(1000);
            }
        }
    }

    /// <summary>
    /// Returns a number based on the how similar player horizontal and vertical input axes.
    /// </summary>
    /// <returns></returns>
    private float GetSyncedControlsSuccess()
    {
        float p1Horz = Input.GetAxis("P1Horizontal");
        float p2Horz = Input.GetAxis("P2Horizontal");
        float p1Vert = Input.GetAxis("P1Vertical");
        float p2Vert = Input.GetAxis("P2Vertical");

        float horizontalSuccess = 0;
        float verticalSuccess = 0;

        // get synchronized values from player horizontal movement
        if(p1Horz >= 0 && p2Horz >= 0)
        {
            horizontalSuccess = p1Horz + p2Horz;
        }
        else if(p1Horz < 0 && p2Horz < 0)
        {
            horizontalSuccess = -p1Horz + -p2Horz;
        }

        // get synchronized values from player vertical movement
        if (p1Vert >= 0 && p2Vert >= 0)
        {
            verticalSuccess = p1Vert + p2Vert;
        }
        else if (p1Vert < 0 && p2Vert < 0)
        {
            verticalSuccess = -p1Vert + -p2Vert;
        }

        // return a value lerped between 0->1
        return (horizontalSuccess + verticalSuccess) * .33f;
    }

    /// <summary>
    /// Halt Fribee's physical inertia applied to RigidBody2D.
    /// </summary>
    private void StopFrisbee()
    {
        if (_frisbeeRB)
        {
            _frisbeeRB.velocity = Vector2.zero;
            _frisbeeRB.inertia = 0f;
            transform.SetParent(null);
        }

        _frisbeeSFX.PlayRandomPitchSFX(frisbeePowerUpNoise, .2f, .8f, 1.2f);
    }

    /// <summary>
    /// Send a pulse of energy to Frisbee in a random direction.
    /// </summary>
    private void ResetFrisbee()
    {
        canBeReset = false;

        MoveToPointBetweenPlayers();
        StartCoroutine(JoltFrisbee());
    }

    private void MoveToPointBetweenPlayers()
    {
        _resetPoint = _playerOneTransform.position + .5f * (_playerTwoTransform.position - _playerOneTransform.position);
        transform.position = _resetPoint;
    }

    private IEnumerator FrisbeeResetCheck()
    {
        if (_checkingFrisbee)
            yield break;

        _checkingFrisbee = true;

        while (true)
        {
            yield return new WaitForSeconds(3f);

            if (!_frisbeeRB.simulated || canBeReset == false)
            {
                continue;
            }

            if(_frisbeeRB.velocity.magnitude <= 9f)
            {
                ResetFrisbee();
            }
        }
    }

    // Called by JoltFrisbee.
    private IEnumerator JoltFrisbee()
    {
        // Waits until next frame to ensure Frisbee returns to position between brothers before adding the pulse of energy.
        yield return new WaitForEndOfFrame();

        // Add energy pulse.
        if (_frisbeeRB)
        {
            _frisbeeRB.simulated = true;
            _frisbeeRB.AddForce(Random.insideUnitCircle * 15f, ForceMode2D.Impulse);
        }
        else
        {
            Debug.LogAssertion("The RigidBody2D on the Frisbee is missing");
        }

        if(_frisbeeSpawnParticles)
        {
            Destroy(Instantiate(_frisbeeSpawnParticles, transform.position, Quaternion.identity), 3f);
        }

        _frisbeeSFX.PlayRandomPitchSFX(frisbeeReset);
    }
}
