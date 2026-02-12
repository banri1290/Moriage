using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CharacterFlip : MonoBehaviour
{
    [SerializeField] private SpriteRenderer targetSprite;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private bool isLeft = true;
    [SerializeField] private float deadzone = 0.001f;
    [SerializeField] private bool updateSelf = true;

    private Vector3 previousPosition;
    private Vector3 currentPosition => targetTransform.position;
    private Vector3 moveDirection => currentPosition - previousPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        previousPosition = currentPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (updateSelf) SetFlip();
    }

    public void SetFlip()
    {
        if (moveDirection.magnitude < deadzone) return;
        Vector3 cross = Vector3.Cross(cameraTransform.forward, moveDirection);
        targetSprite.flipX = cross.y >= 0 == isLeft;
        previousPosition = currentPosition;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) return;
        if (targetSprite == null) targetSprite = GetComponent<SpriteRenderer>();
        if (targetTransform == null) targetTransform = transform;
        if (cameraTransform == null) cameraTransform = Camera.main.transform;
    }
#endif
}
