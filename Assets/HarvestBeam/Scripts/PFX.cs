using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class PFX : MonoBehaviour
{

    private ParticleSystem pfxSystem;

    private void Start()
    {
        pfxSystem = GetComponent<ParticleSystem>();
    }

    public void UpdatePFX(Vector3 targetPosition, float speed)
    {
        transform.localPosition = HarvestBeamRenderer.ImpactVector;

        if (pfxSystem.isPlaying == false)
        {
            pfxSystem.Play();
        }

        var main = pfxSystem.main;
        main.startSpeed = speed * .5f;
    }

    public void StopPFX()
    {
        if (pfxSystem.isPlaying == true)
        {
            pfxSystem.Stop();
        }
    }
}
