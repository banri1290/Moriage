using UnityEngine;

public class Scaler : MonoBehaviour
{
    private Transform m_transform;

    public void SetScale(float x)
    {
        SetScale(x, x);
    }

    public void SetScale(float x, float y)
    {
        SetScale(x, y, 1);
    }

    public void SetScale(float x, float y, float z)
    {
        if (m_transform == null)
        {
            m_transform = transform;
        }
        m_transform.localScale = new Vector3(x, y, z);
    }
}