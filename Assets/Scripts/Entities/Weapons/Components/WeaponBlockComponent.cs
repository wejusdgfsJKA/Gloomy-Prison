using UnityEngine;

public class WeaponBlockComponent : MonoBehaviour
{
    //manages the blockign part of a weapon
    [SerializeField] protected WeaponBlockData data;
    [SerializeField] protected Transform blockPoint;
    public BlockResult Block(DmgInfo _dmgInfo)
    {
        //we need to calculate the angle at which the attack hit us
        float _strikeangle = Mathf.Acos(Vector3.Dot(blockPoint.forward,
             (_dmgInfo.ContactPoint - blockPoint.position).normalized)) * Mathf.Rad2Deg;
        //Debug.Log(strikeangle);
        if (_strikeangle <= data.BlockAngle)
        {
            //we blocked this attack
            if (_dmgInfo.Attack.AttackStrength == Attack.Strength.Heavy &&
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
