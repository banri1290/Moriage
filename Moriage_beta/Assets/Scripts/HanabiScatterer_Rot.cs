using UnityEngine;

public class HanabiScatterer_Rot : HanabiScatterer
{
    [SerializeField] private float angularSpeed;
    [SerializeField] private float defaultSize;

    public override void SetParameter(HanabiParticle particle, Vector2 velocity, float multiply)
    {
        particle.SetMovement(velocity * multiply, Vector2.zero, angularSpeed);
        float rot=Random.Range(0f,360f);
        particle.transform.rotation=Quaternion.Euler(0f,0f,rot);
        particle.SetSize(defaultSize*multiply, 0f);
        particle.SetFadeout(0f);
    }
}
