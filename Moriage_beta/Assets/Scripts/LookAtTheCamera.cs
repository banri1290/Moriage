using UnityEngine;

public class LookAtTheCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 rotation;

    private Transform me;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        me = transform;
    }

    // Update is called once per frame
    void Update()
    {
        me.forward = Quaternion.Euler(rotation) * target.forward;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) return;
        if (target == null) target = Camera.main.transform;
    }
#endif

}
