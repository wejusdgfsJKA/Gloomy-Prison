using UnityEngine;
public class Receiver : MonoBehaviour
{
    protected System.Action mainAction;
    protected void Awake()
    {
        enabled = false;
    }
    protected void OnEnable()
    {
        //this script will be enabled, execute its action and disable itself again
        mainAction();
        enabled = false;
    }
    public void AddAction(System.Action action)
    {
        //add an action to this delegate
        mainAction += action;
    }
}
