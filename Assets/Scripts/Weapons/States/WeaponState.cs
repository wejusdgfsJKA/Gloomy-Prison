public abstract class WeaponState
{
    /// <summary>
    /// True if we can attack from this state.
    /// </summary>
    public bool CanAttack { get; protected set; } = true;
    /// <summary>
    /// True if we can block from this state.
    /// </summary>
    public bool CanBlock { get; protected set; } = true;
    protected Weapon weapon;
    /// <summary>
    /// Fires when entering this state.
    /// </summary>
    public abstract void OnEnter();
    /// <summary>
    /// Fires when exiting this state.
    /// </summary>
    public virtual void OnExit() { }
}