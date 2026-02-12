using UnityEngine;
using UnityEngine.Events;

public class CollisionTrigger : MonoBehaviour
{
    [SerializeField] private Collider target;
    [SerializeField] private UnityEvent onCollisionEnter;
    [SerializeField] private UnityEvent onCollisionExit;

    void OnTriggerEnter(Collider other)
    {
        if (other == target)
            onCollisionEnter.Invoke();
    }

    void OnTriggerExit(Collider other)
    {
        if (other == target)
            onCollisionExit.Invoke();
    }
}
