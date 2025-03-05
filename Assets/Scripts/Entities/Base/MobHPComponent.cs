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
    protected void Die()
    {
        gameObject.SetActive(false);
    }
}
