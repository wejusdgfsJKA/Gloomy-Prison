using UnityEngine;

public class WeaponBlockComponent : MonoBehaviour
{
    //manages the blockign part of a weapon
    [SerializeField]
    protected WeaponBlockData data;
    [SerializeField]
    protected Transform blockPoint;
    public BlockResult Block(DmgInfo dmgInfo)
    {
        //we need to calculate the angle at which the attack hit us
        float strikeangle = Mathf.Acos(Vector3.Dot(blockPoint.forward,
             (dmgInfo.ContactPoint - blockPoint.position).normalized)) * Mathf.Rad2Deg;
        //Debug.Log(strikeangle);
        if (strikeangle <= data.BlockAngle)
        {
            //we blocked this attack
            if (dmgInfo.Attack.AttackStrength == Attack.Strength.Heavy &&
                !data.Shield)
            {
                //we blocked a heavy attack without a shield
                return BlockResult.Partial;
            }
            return BlockResult.Success;
        }
        return BlockResult.Failure;
    }
}
