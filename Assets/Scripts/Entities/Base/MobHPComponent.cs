public class MobHPComponent : HPComponent
{
    protected void Awake()
    {
        TakeDamage = (int _damage) =>
        {
            CurrentHP -= _damage;
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
