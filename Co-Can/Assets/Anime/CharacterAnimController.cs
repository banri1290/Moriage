using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CharacterAnimController : MonoBehaviour
{
    [Tooltip("Animator付きのSprite")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private string isWalkingParam = "IsFacingRight";
    [Tooltip("本体")]
    [SerializeField] private Transform observedTransform;
    [Tooltip("カメラ")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float deadzone = 0.001f;
    [SerializeField] private bool flipX;

    private Transform spriteTransform;
    private Vector3 lastPosition;
    private bool isMoving;

    void Start()
    {
        if (observedTransform == null) observedTransform = transform;
        if (spriteTransform == null) spriteTransform = spriteRenderer.transform;
        if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;
        if (animator == null && spriteTransform != null) animator = spriteTransform.GetComponent<Animator>();

        lastPosition = observedTransform.position;
    }

    void Update()
    {
        Vector3 currentPosition = observedTransform.position;
        Vector3 deltaPosition = currentPosition - lastPosition;
        bool _isMoving = deltaPosition.magnitude > deadzone;
        if (_isMoving != isMoving)
        {
            isMoving = _isMoving;
            if (animator != null)
            {
                animator.SetBool(isWalkingParam, isMoving);
            }
        }
        if (isMoving)
        {
            OnPositionChanged(deltaPosition);
        }
    }

    void LateUpdate()
    {
        if (spriteTransform != null)
        {
            spriteTransform.forward = cameraTransform.forward;
        }
    }

    void OnPositionChanged(Vector3 deltaPosition)
    {
        Vector3 crossProduct = Vector3.Cross(cameraTransform.forward, deltaPosition);

        if (Mathf.Abs(crossProduct.y) > deadzone)
        {
            spriteRenderer.flipX = (crossProduct.y < 0f) != flipX;
        }

        lastPosition += deltaPosition;
    }
}
