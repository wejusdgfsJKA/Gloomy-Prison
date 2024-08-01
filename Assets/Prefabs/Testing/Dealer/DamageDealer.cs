using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [SerializeField]
    DealerAnimHandler animHandler;
    public bool b1;
    void Update()
    {
        if (b1)
        {
            b1 = false;
            //attack
            animHandler.Swing();
        }
    }
    public void gey()
    {

    }
}
