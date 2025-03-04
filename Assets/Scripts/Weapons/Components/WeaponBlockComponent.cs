using UnityEngine;

/// <summary>
/// Success means the attack was successfully blocked, Failure means
/// we did not block. Partial means we blocked an attack that was
/// simply too strong, so take full damage as well as stamina damage 
/// as well as regular damage.
/// </summary>
public enum BlockResult { Success = 0, Failure = 1, Partial = 2 }

/// <summary>
/// Manages the blocking part of a weapon.
/// </summary>
public class WeaponBlockComponent : MonoBehaviour
{
    [SerializeField] protected WeaponBlockData data;
    /// <summary>
    /// Where to measure the block angle from
    /// </summary>
    [SerializeField] protected Transform blockPoint;
    protected Coroutine blockCoroutine;
    public StaminaComponent StaminaComp { protected get; set; }
    /// <summary>
    /// Returns whether the weapon is blocking an attack or not.
    /// </summary>
    /// <param name="dmgInfo">The damage package containing the attack.</param>
    /// <returns>A BlockResult datatype.</returns>
    public BlockResult Block(DmgInfo dmgInfo)
    {
        //we need to calculate the angle at which the attack hit us
        float strikeangle = Mathf.Acos(Vector3.Dot(blockPoint.forward,
             (dmgInfo.ContactPoint - blockPoint.position).normalized)) * Mathf.Rad2Deg;
        //Debug.Log(strikeangle);
        if (strikeangle <= data.BlockAngle)
        {
            //the attack is within our block angle
            return BlockResult.Success;
        }
        return BlockResult.Failure;
    }
    /// <summary>
    /// Begin or stop blocking.
    /// </summary>
    /// <param name="block">If true, start blocking. If false, stop blocking.</param>
    protected void Block(bool block)
    {
        if (!block)
        {
            if (blockCoroutine != null)
            {
                StopCoroutine(blockCoroutine);
            }
        }
    }
}
