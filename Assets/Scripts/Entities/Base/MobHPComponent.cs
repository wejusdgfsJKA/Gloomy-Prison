/// <summary>
/// Handles the HP of an entity which will die when its HP reaches 0.
/// </summary>
public class MobHPComponent : HPComponent
{
    protected void Awake()
    {
        TakeDamage = (int damage) =>
        {
            CurrentHP -= damage;
            if (CurrentHP <= 0)
            {
                Die();
            }
        };
    }
    /// <summary>
    /// Fires when the entity dies.
    /// </summary>
    protected void Die()
    {
        gameObject.SetActive(false);
    }
}
