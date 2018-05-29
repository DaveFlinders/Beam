using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class HarvestBeam : MonoBehaviour {

    [Range(0.1f, 20f)]
    public float effectSpeed;
    public int meshResolution = 40;

    private HarvestBeamRenderer[] harvestBeamRenderers;
    private ParticleSystem harvestBeamParticleSystem;
    private ParticleSystem.ShapeModule shapeModule;
    private bool isActive;
    private Transform target;
    private PFX pfx;

    public Transform Target
    {
        get
        {
            return target;
        }
    }

    public delegate void OnBeamActivated();
    public static event OnBeamActivated onBeamActivated;

    public delegate void OnBeamDeactivated();
    public static event OnBeamActivated onBeamDeactivated;

    void Start ()
    {
        harvestBeamRenderers = GetComponentsInChildren<HarvestBeamRenderer>();
        harvestBeamParticleSystem = GetComponent<ParticleSystem>();
        shapeModule = harvestBeamParticleSystem.shape;
        shapeModule.shapeType = ParticleSystemShapeType.Mesh;
        shapeModule.enabled = true;

        if (pfx == null)
        {
            pfx = FindObjectOfType<PFX>();
        }
    }

    /// <summary>
    /// this function must be called once a frame.
    /// responsible for updating the beam mesh data and particle systems.
    /// </summary>
    /// <param name="_target"></param>
    public void UpdateHarvestBeam(Transform _target)
    {
        if(isActive == false)
        {
            isActive = true;

            target = _target;


            if (harvestBeamParticleSystem.isPlaying == false)
                harvestBeamParticleSystem.Play();

            if (onBeamActivated != null)
            {
                onBeamActivated();
            }
        }

        pfx.UpdatePFX(Target.position, effectSpeed);

        float distanceFromNode = Mathf.Abs(Vector3.Distance(transform.position, Target.position));

        transform.LookAt(Target);

        for(int i = 0; i< harvestBeamRenderers.Length; i++)
        {
            harvestBeamRenderers[i].GetComponent<MeshRenderer>().enabled = true;
            harvestBeamRenderers[i].Velocity = distanceFromNode + 3f; // allow for player and target height offsets!
            harvestBeamRenderers[i].EffectSpeed = effectSpeed;
            harvestBeamRenderers[i].Resolution = meshResolution;
        }

        shapeModule.mesh = harvestBeamRenderers[0].Mesh;
    }

    public void DeactivateHarvestBeam()
    {
        isActive = false;

        for (int i = 0; i < harvestBeamRenderers.Length; i++)
        {
            harvestBeamRenderers[i].GetComponent<MeshRenderer>().enabled = false;
        }

        pfx.StopPFX();

        effectSpeed = 0.1f;

        if (harvestBeamParticleSystem.isPlaying == true)
            harvestBeamParticleSystem.Stop();

        if (onBeamDeactivated != null)
        {
            onBeamDeactivated(); //callback for when beam is deactivated.
        }
    }
}
