using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ObjectSoundEffects))]
public abstract class BaseHealth : MonoBehaviour
{
    #region Public Attributes
    [Header("BaseHealth Attributes")]
    [SerializeField] protected float DamageCooldownLength = 1f;
    [SerializeField] protected GameObject deathParticles;

    [Header("BaseHealth Damage Audio")]
    [SerializeField] protected AudioClip damageSFX;
    [SerializeField] protected AudioClip deathSFX;
    #endregion

    #region Protected Variables
    protected SpriteRenderer objectSprite;
    protected ObjectSoundEffects soundEffects;
    protected bool canTakeDamage;
    #endregion

    private int _currentHealth = 1;

    protected virtual void Start()
    {
        soundEffects = GetComponent<ObjectSoundEffects>();
        objectSprite = GetComponent<SpriteRenderer>();

        canTakeDamage = true;
    }

    /// <summary>
    /// Deals damage to object - kills if currentHealth is less than 0.
    /// </summary>
    /// <param name="damageAmount">Amount of damage to inflict to target.</param>
    public virtual void ApplyDamageToTarget(int damageAmount = 1)
    {
        if(canTakeDamage && damageAmount > 0)
        {
            _currentHealth -= damageAmount;

            if(_currentHealth <= 0)
            {
                TargetDeath();
            }
            // Damage effects.
            else
            {
                soundEffects.PlayRandomPitchSFX(damageSFX);
                StartCoroutine(CanTakeDamageCoolDown());
                StartCoroutine(DamageColorChange());
            }
        }
        else if(damageAmount <= 0)
        {
            Debug.LogWarning("damageAmount must be greater than 0");
        }
    }

    /// <summary>
    /// Resets object's health to it's initial starting health amount.
    /// </summary>
    /// <param name="initialHealth">Starting health amount.</param>
    protected void ResetTargetHealth(int initialHealth)
    {
        _currentHealth = initialHealth;
    }

    /// <summary>
    /// Grants brief invulnerability until the cooldown is complete.
    /// </summary>
    /// <returns></returns>
    protected IEnumerator CanTakeDamageCoolDown()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(DamageCooldownLength);
        canTakeDamage = true;
    }

    protected virtual void TargetDeath()
    {
        soundEffects.PlayRandomPitchSFX(deathSFX);

        if(deathParticles)
        {
            Destroy(Instantiate(deathParticles, transform.position, Quaternion.identity), 3f);
        }
    }

    protected abstract IEnumerator DamageColorChange();
}