/// <summary>
/// Interface to implement if a gameobject should be damagable.
/// </summary>
public interface IDamageable {

    /// <summary>
    /// Reduces the gameobject's integrity by a given amount.
    /// </summary>
    /// <param name="damage">The amount of damage to be taken.</param>
    void TakeDamage(float damage);
}
