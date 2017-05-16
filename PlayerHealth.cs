using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : BaseHealth
{
    public int initialPlayerHealth = 3;

    private BasePlayerController _playerController;
    private bool _isInvincible;

    private static Color32 playerDamageColor = new Color32(0, 125, 125, 0);
    private const int damageCooldown = 3;

    protected override void Start()
    {
        base.Start();

        _playerController = GetComponent<BasePlayerController>();
        _isInvincible = false;

        ResetTargetHealth(initialPlayerHealth);
    }

    void OnEnable()
    {
        BasePlayerController.EventActivatePowerMode += MakePlayerInvincible;
        BasePlayerController.EventCancelPowerMode += CancelPlayerInvincibility;
    }

    void OnDisable()
    {
        BasePlayerController.EventActivatePowerMode -= MakePlayerInvincible;
        BasePlayerController.EventCancelPowerMode -= CancelPlayerInvincibility;
    }

    /// <summary>
    /// Deals damage to player object.
    /// </summary>
    /// <param name="damageAmount">Damage to inflict on player.</param>
    public override void ApplyDamageToTarget(int damageAmount = 1)
    {
        if(!_isInvincible)
        {
            base.ApplyDamageToTarget();

            _playerController.ResetPlayerCatchCombo();
        }
    }

    /// <summary>
    /// Kill player and restart the game.
    /// </summary>
    protected override void TargetDeath()
    {
        base.TargetDeath();

        CameraShake.Instance.ShakeTheCamera(.1f, .1f, false);
        objectSprite.enabled = false;

        Invoke("RestartGameAfterDeath", 2f);
    }

    /// <summary>
    /// Change player color (red) to indicate damage/invulnerability timer.
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator DamageColorChange()
    {
        objectSprite.color -= playerDamageColor;
        yield return new WaitForSeconds(DamageCooldownLength);
        objectSprite.color += playerDamageColor;
    }

    private void RestartGameAfterDeath()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void MakePlayerInvincible()
    {
        _isInvincible = true;
    }

    private void CancelPlayerInvincibility()
    {
        _isInvincible = false;
    }
}
