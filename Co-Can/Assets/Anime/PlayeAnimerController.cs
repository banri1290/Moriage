using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerAnimeController : MonoBehaviour
{
    [Tooltip("Animator付きのSprite")]
    public Transform spriteTransform;
    [Tooltip("本体")]
    public Transform observedTransform;
    public float deadzone = 0.001f;

    public string paramFacing = "IsFacingRight";

    private Animator animator;
    private float lastX;
    private Quaternion initialRotation; 

    void Start()
    {
        if (observedTransform == null) observedTransform = transform;
        if (spriteTransform == null) spriteTransform = transform;

        animator = spriteTransform.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogWarning($"[PlayerAnimeController] Animator 未找到：{spriteTransform.name}");
        }

        lastX = observedTransform.position.x;

        initialRotation = spriteTransform.rotation;
    }

    void Update()
    {
        spriteTransform.rotation = initialRotation;

        float currentX = observedTransform.position.x;
        float deltaX = currentX - lastX;

        if (Mathf.Abs(deltaX) > deadzone)
        {
            bool facingRight = deltaX > 0f;

            if (animator != null)
            {
                //animator.SetBool(paramFacing, facingRight);
            }

            Vector3 scale = spriteTransform.localScale;
            scale.x = facingRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            spriteTransform.localScale = scale;

        }

        lastX = currentX;
    }


    void LateUpdate()
    {
        if (spriteTransform != null && Camera.main != null)
        {
            spriteTransform.forward = Camera.main.transform.forward;
        }
    }

}
