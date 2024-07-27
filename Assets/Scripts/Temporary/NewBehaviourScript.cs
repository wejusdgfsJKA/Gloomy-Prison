using UnityEngine;
using UnityEngine.AI;

public class NewBehaviourScript : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<NavMeshAgent>().SetDestination(transform.position + transform.forward * 10);
    }
}
