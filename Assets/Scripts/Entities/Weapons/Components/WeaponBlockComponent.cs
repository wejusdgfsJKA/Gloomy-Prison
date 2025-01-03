using System.Collections;
using UnityEngine;

public class WeaponBlockComponent : MonoBehaviour
{
    //manages the blockign part of a weapon
    [SerializeField] protected WeaponBlockData data;
    [SerializeField] protected Transform blockPoint;
    protected Coroutine blockCoroutine;
    public StaminaComponent StaminaComp { protected get; set; }
    public BlockResult Block(DmgInfo _dmgInfo)
    {
        //we need to calculate the angle at which the attack hit us
        float _strikeangle = Mathf.Acos(Vector3.Dot(blockPoint.forward,
             (_dmgInfo.ContactPoint - blockPoint.position).normalized)) * Mathf.Rad2Deg;
        //Debug.Log(strikeangle);
        if (_strikeangle <= data.BlockAngle)
        {
            //the attack is within our block angle
            return BlockResult.Success;
        }
        return BlockResult.Failure;
    }
    protected void Block(bool block)
    {
        if (block)
        {
            blockCoroutine = StartCoroutine(ExecuteBlock());
        }
        else
        {
            if (blockCoroutine != null)
            {
                StopCoroutine(blockCoroutine);
            }
        }
    }
    protected IEnumerator ExecuteBlock()
    {
        while (true)
        {
            StaminaComp.DrainStamina(data.PeriodicStaminaDrain);
        }
    }
}
