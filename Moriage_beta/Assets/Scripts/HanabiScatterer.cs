using System.Linq;
using UnityEngine;

public abstract class HanabiScatterer : MonoBehaviour
{
    [SerializeField] private HanabiParticle particlePrefab;
    [SerializeField] private int particleCount;
    [SerializeField] private float time;
    [SerializeField] private float minDestance;
    [SerializeField] private float maxDestance;
    [SerializeField] private float minMultiply;
    [SerializeField] private float maxMultiply;
    [SerializeField] private float defaultSpeed;
    [SerializeField] private float directionVariance;
    [SerializeField] private Texture2D colorMap;
    [SerializeField] private bool playOnAwake = false;

    // Start is called before the first frame update
    void Start()
    {
        if (playOnAwake)
        {
            Scatter();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Scatter()
    {
        float baseDirection = Random.Range(0, 360);
        float[] multiply = ClearRandom(minMultiply, maxMultiply, particleCount);
        for (int i = 0; i < particleCount; i++)
        {
            float distance = minDestance + multiply[i] * (maxDestance - minDestance);
            float direction
                = (baseDirection
                + Random.Range(-directionVariance, directionVariance)
                + i * 360f / particleCount
                );
            Vector2 normal = new(Mathf.Cos(direction * Mathf.Deg2Rad), Mathf.Sin(direction * Mathf.Deg2Rad));

            HanabiParticle particle = Instantiate(particlePrefab, transform);
            particle.transform.rotation = Quaternion.Euler(0f, 0f, direction);
            particle.transform.localPosition = normal * distance;
            SetParameter(particle, normal * defaultSpeed, multiply[i]);
            particle.Init(time);

            int u = Mathf.FloorToInt(colorMap.width * Random.Range(0f, 1f));
            int v = Mathf.FloorToInt(colorMap.height * Random.Range(0f, 1f));
            Color color = colorMap.GetPixel(u, v);
            particle.SetColor(color);
        }
    }

    float[] ClearRandom(float min, float max, int count)
    {
        float[] result;
        if (count <= 1)
        {
            result = new float[1];
            result[0] = Random.Range(min, max);
            return result;
        }
        result = new float[count];
        for (int i = 0; i < count; i++)
        {
            result[i] = Random.Range(0f, 1f);
        }
        float rMin = result.Min();
        float rMax = result.Max();
        for (int i = 0; i < count; i++)
        {
            float r = 0f;
            if (rMin != rMax)
            {
                r = (result[i] - rMin) / (rMax - rMin);
            }
            else
            {
                r = i * 1f / (count - 1);
            }
            result[i] = r * (max - min) + min;
        }

        return result;
    }

    public abstract void SetParameter(HanabiParticle particle, Vector2 velocity, float multiply);
}
