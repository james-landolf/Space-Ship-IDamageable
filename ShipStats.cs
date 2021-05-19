using System.Collections;
using UnityEngine;

/// <summary>
/// Class containing stats for a ship in game. 
/// If a ship's integrity is reduced to 0, the associated ship is destroyed. 
/// </summary>
public class ShipStats : MonoBehaviour, IDamageable
{
    // The amount all sources of damage are reduced by.
    public float shipArmor;
    // The maximum amount of power.
    public float shipPowerMax;
    // The maximum amount of integrity.
    public float shipIntegrityMax;
    // The amount of time it takes the ship to detonate after destruction is triggered.
    public float detonationTime;
    // The effect displayed upon the ship's destruction.
    public GameObject explosionEffect;

    // The current amount of power.
    private float shipPowerCurrent;
    // The current amount of integrity
    private float shipIntegrityCurrent;
    // If cool down is required before using more power.
    private bool coolingDown;
    // The percentage of power regenerated each frame.
    private float powerRegenPercent = .1f;
    // If the ship has been destroyed.
    private bool destroyed;

    // Use this for initialization
    void Start()
    {
        shipPowerCurrent = shipPowerMax;
        shipIntegrityCurrent = shipIntegrityMax;
        coolingDown = false;
        destroyed = false;
        DisplayIntegrity();
    }

    // Update is called once per frame
    void Update()
    {
        if (shipPowerCurrent < shipPowerMax)
        {
            if (shipPowerCurrent > shipPowerMax)
            {
                shipPowerCurrent = shipPowerMax;
            }
            if (coolingDown && shipPowerCurrent > shipPowerMax * powerRegenPercent)
            {
                coolingDown = false;
            }
        }
    }

    /// <summary>
    /// Displays the ship's inegrity on the GUI.
    /// </summary>
    private void DisplayIntegrity()
    {
        // Not Yet Implemented
    }

    /// <summary>
    /// Reduces the ship's current integrity by an amount reduced by armor.
    /// </summary>
    /// <param name="amount">The amount of damage taken.</param>
    public void TakeDamage(float amount)
    {
        if (amount - shipArmor > 0)
        {
            shipIntegrityCurrent -= amount - shipArmor;
            if (shipIntegrityCurrent <= 0)
            {
                destroyed = true;
                shipIntegrityCurrent = 0;
                StartCoroutine(Explode());
            }
            DisplayIntegrity();
        }
    }

    /// <summary>
    /// Increases the ship's current integrity not exceeding it's maximum integrity.
    /// </summary>
    /// <param name="amount">The amount of damage repaired.</param>
    public void RepairDamage(float amount)
    {
        shipIntegrityCurrent += amount;
        if (shipIntegrityCurrent > shipIntegrityMax)
        {
            shipIntegrityCurrent = shipIntegrityMax;
        }
        DisplayIntegrity();
    }

    /// <summary>
    /// Uses an amount of the ship's current power.
    /// </summary>
    /// <param name="amount">The amount of power used.</param>
    public void UsePower(float amount)
    {
        shipPowerCurrent -= amount;
        if (shipPowerCurrent < 0)
        {
            shipPowerCurrent = 0;
        }
    }

    /// <summary>
    /// Restores an amount of the ship's current power not exceeding it's maximum power.
    /// </summary>
    /// <param name="amount">The amount of power restored.</param>
    public void RestorePower(float amount)
    {
        shipPowerCurrent += amount;
        if (shipPowerCurrent > shipPowerMax)
        {
            shipPowerCurrent = shipPowerMax;
        }
    }

    /// <summary>
    /// Determines if the ship has enough power for a given function and is not currently cooling down.
    /// </summary>
    /// <param name="powerNeeded">The amount of power needed for a given function.</param>
    /// <returns>If the ship has the amount of power needed.</returns>
    public bool HasPower(float powerNeeded)
    {
        bool hasPower = false;
        if (powerNeeded <= shipPowerCurrent && !coolingDown)
        {
            hasPower = true;
        }
        else
        {
            hasPower = false;
            coolingDown = true;
        }
        return hasPower;
    }

    /// <summary>
    /// Determines if the ship has been destroyed.
    /// </summary>
    /// <returns>If the ship has been destroyed.</returns>
    public bool IsDestroyed()
    {
        return destroyed;
    }

    /// <summary>
    /// Instantiates the explosion effect for the ship and destroys the gameobject.
    /// </summary>
    public void DoExplosion()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    /// <summary>
    /// IEnumerator to count down the ship's destruction.
    /// </summary>
    IEnumerator Explode()
    {
        float detonationCounter = 0;
        GetComponent<ShipMovement>().Interrupt(true);

        while (detonationCounter < detonationTime)
        {
            detonationCounter += Time.deltaTime;
            yield return null;
        }

        DoExplosion();
    }
}
