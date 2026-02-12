using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(RectTransform))]
public class DragTarger : MonoBehaviour
{
    [SerializeField] private Dragger[] draggers;
    [SerializeField] private float fixDistance;
    [SerializeField] private bool setOneOnly = false;
    [SerializeField] private UnityEvent onDraggerEnter;
    [SerializeField] private UnityEvent onDraggerExit;

    [SerializeField, HideInInspector] private RectTransform[] draggerTransform = { };
    [SerializeField, HideInInspector] private RectTransform rectTransform;

    private bool[] draggerSet;
    private const int NO_DRAGGER_SET = -1;
    private UnityAction<int> onDraggerEnterAction=null;
    private UnityAction<int> onDraggerExitAction=null;

    private int currentDraggerIndex
    {
        get
        {
            for (int i = 0; i < draggerSet.Length; i++)
            {
                if (draggerSet[i]) return i;
            }
            return NO_DRAGGER_SET;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        draggerSet = new bool[draggers.Length];
        for (int i = 0; i < draggers.Length; i++)
        {
            draggerSet[i] = false;
        }
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (setOneOnly && currentDraggerIndex != NO_DRAGGER_SET)
        {
            FixDragger(currentDraggerIndex);
        }
        else
        {
            for (int i = 0; i < draggers.Length; i++)
            {
                FixDragger(i);
            }
        }
    }

    private void FixDragger(int index)
    {
        if (draggerTransform[index] == null) return;
        Vector3 draggerDifference = draggers[index].UpdatedPosition - rectTransform.position;
        bool nearX = Mathf.Abs(draggerDifference.x) < fixDistance;
        bool nearY = Mathf.Abs(draggerDifference.y) < fixDistance;
        bool near = nearX && nearY;
        if (near)
        {
            draggerTransform[index].position = rectTransform.position;
        }
        if (near && !draggerSet[index])
        {
            DraggerEnter(index);
        }
        else if (!near && draggerSet[index])
        {
            DraggerExit(index);
        }
    }

    private void DraggerEnter(int index)
    {
        draggerSet[index] = true;
        draggers[index].freezeX = true;
        draggers[index].freezeY = true;
        onDraggerEnter?.Invoke();
        onDraggerEnterAction?.Invoke(index);
    }

    private void DraggerExit(int index)
    {
        draggerSet[index] = false;
        draggers[index].freezeX = false;
        draggers[index].freezeY = false;
        onDraggerExit?.Invoke();
        onDraggerExitAction?.Invoke(index);
    }

    public void SetAction(UnityAction<int> _onDraggerEnter, UnityAction<int> _onDraggerExit)
    {
        onDraggerEnterAction = _onDraggerEnter;
        onDraggerExitAction = _onDraggerExit;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode) return;
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        if (draggers == null) return;
        for (int i = 0; i < draggers.Length; i++)
        {
            if (draggers[i] == null) return;
        }
        if (draggers.Length == 0) return;
        SetDraggerTransform();
    }

    private void SetDraggerTransform()
    {
        bool needToSet = false;
        if (draggerTransform.Length != draggers.Length) needToSet = true;
        else
        {
            for (int i = 0; i < draggerTransform.Length; i++)
            {
                if (draggerTransform[i] == null)
                {
                    needToSet = true;
                    break;
                }
                if (draggerTransform[i].gameObject != draggers[i].gameObject)
                {
                    needToSet = true;
                    break;
                }
            }
        }

        if (!needToSet) return;
        draggerTransform = new RectTransform[draggers.Length];
        for (int i = 0; i < draggerTransform.Length; i++)
        {
            draggerTransform[i] = draggers[i].GetComponent<RectTransform>();
        }
    }
#endif
}
