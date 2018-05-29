using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class HarvestBeamRenderer : MonoBehaviour {

    public bool isTop;

    private int resolution = 10;
    private float effectSpeed = 1.0f;
    private float noiseStrength = .45f;
    private float noiseWalk = 1f;
    private Mesh mesh;
    private float g;
    private float radianAngle;
    private float startMeshWidth;
    public static Vector3 impactVector;
    private bool beamIsActive;
    private Renderer rend;
    private float velocity = 5f;
    private float angle = 65f;
    private float scale = 0.15f;
    private float meshWidth = 0.5f;
    private float phase;
    private float _effectSpeed;
    private float scrollSpeed;

    public Mesh Mesh
    {
        get
        {
            return mesh;
        }
    }

    /// <summary>
    /// Vector position for the beam impact
    /// </summary>
    public static Vector3 ImpactVector
    {
        get
        {
            return impactVector;
        }
    }

    public float Velocity
    {
        set
        {
            velocity = value;
        }
    }

    public float EffectSpeed
    {
        set
        {
            effectSpeed = value;
        }
    }

    public int Resolution
    {
        set
        {
            resolution = value;
        }
    }

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        g = Mathf.Abs(Physics.gravity.y);

        startMeshWidth = meshWidth;

        rend = GetComponent<Renderer>();

        HarvestBeam.onBeamActivated += activeBeam;
        HarvestBeam.onBeamDeactivated += deactiveBeam;
    }

    void activeBeam()
    {
        beamIsActive = true;
    }

    void deactiveBeam()
    {
        beamIsActive = false;
        scrollSpeed = 0;
    }

    private void OnDisable()
    {
        HarvestBeam.onBeamActivated -= activeBeam;
        HarvestBeam.onBeamActivated -= deactiveBeam;
    }


    private void Update()
    {
        if (mesh != null && beamIsActive)
        {
            MakeArcMesh(CalculateArcArray());

            scrollSpeed += Time.deltaTime * 0.25f;

            rend.material.SetTextureOffset("_MainTex", new Vector2(effectSpeed * -scrollSpeed, effectSpeed * -scrollSpeed));
        }
    
    }

    void MakeArcMesh(Vector3[] arcVerts)
    {

        mesh.Clear();

        if(effectSpeed != _effectSpeed)
        {
            CalcNewSpeed(); // smooth out the speed transition
        }

        Vector3[] vertices = new Vector3[(resolution + 1) * 2]; // + 1 because we need an end point, * 2 for both sides 
        int[] triangles = new int[resolution * 6 * 2]; // each quad is 2 triangle (6 verts)
        Vector2[] uvs = new Vector2[(resolution + 1) * 2];

        meshWidth = startMeshWidth;

        for (int i = 0; i <= resolution; i++)
        {
            //set 2D arch into 3D space
            //set verts
            if (meshWidth > 0.2f)
                meshWidth -= meshWidth * 0.05f;

            if(isTop == true)
            {
                vertices[i * 2] = new Vector3(meshWidth * 0.5f, arcVerts[i].y, arcVerts[i].x); // even side
                vertices[i * 2 + 1] = new Vector3(meshWidth * -0.5f, arcVerts[i].y, arcVerts[i].x); // uneven side
            } else
            {
                vertices[i * 2] = new Vector3(arcVerts[i].z, arcVerts[i].y + meshWidth * 0.5f, arcVerts[i].x); // even side
                vertices[i * 2 + 1] = new Vector3(arcVerts[i].z, arcVerts[i].y + meshWidth * -0.5f, arcVerts[i].x); // uneven side
            }

            if (effectSpeed > 0)
            {

                //Add some "wobble"
                float wobbleEven = Mathf.Sin(Time.time * _effectSpeed + phase + vertices[i * 2].x + vertices[i * 2].y + vertices[i * 2].z) * scale;
                float wobbleOdd = Mathf.Sin(Time.time * _effectSpeed + phase + vertices[i * 2 + 1].x + vertices[i * 2 + 1].y + vertices[i * 2 + 1].z) * scale;
                vertices[i * 2] += Vector3.one * wobbleEven;
                vertices[i * 2 + 1] += Vector3.one * wobbleOdd;


                //Add some perlin noise
                float perlinEven = Mathf.PerlinNoise(vertices[i * 2].x + noiseWalk, vertices[i * 2].y + Mathf.Sin(Time.time * 0.1f)) * noiseStrength;
                float perlinOdd = Mathf.PerlinNoise(vertices[i * 2 + 1].x + noiseWalk, vertices[i * 2 + 1].y + Mathf.Sin(Time.time * 0.1f)) * noiseStrength;
                vertices[i * 2] += Vector3.one * perlinEven;
                vertices[i * 2 + 1] += Vector3.one * perlinOdd;
            }

            //Set the first and last vertices where they should be!
            if (i <= 2 || i >= resolution - 2)
            {
                vertices[i * 2] = new Vector3(meshWidth * 0.25f, arcVerts[i].y, arcVerts[i].x); // even side
                vertices[i * 2 + 1] = new Vector3(meshWidth * -0.25f, arcVerts[i].y, arcVerts[i].x); // uneven side
            }

            //set triangles
            if (i != resolution) // not on the end 
            {
                //top
                triangles[i * 12] = i * 2;
                triangles[i * 12 + 1] = triangles[i * 12 + 4] = i * 2 + 1;
                triangles[i * 12 + 2] = triangles[i * 12 + 3] = (i + 1) * 2;
                triangles[i * 12 + 5] = (i + 1) * 2 + 1;

                ////bottom
                triangles[i * 12 + 6] = i * 2;
                triangles[i * 12 + 7] = triangles[i * 12 + 10] = (i + 1) * 2;
                triangles[i * 12 + 8] = triangles[i * 12 + 9] = i * 2 + 1;
                triangles[i * 12 + 11] = (i + 1) * 2 + 1;
            }
        }

        for (int i = 0; i < uvs.Length; i++)
        {
            if (isTop == true)
            {
                uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
            } else
            {
                uvs[i] = new Vector2(vertices[i].y, vertices[i].z);
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        impactVector = vertices[vertices.Length-1]; // store the impact point (can be used for effect on mining node).
    }

    void CalcNewSpeed()
    {
        float curr = (Time.time * _effectSpeed + phase) % (2.0f * Mathf.PI);
        float next = (Time.time * effectSpeed) % (2.0f * Mathf.PI);
        phase = curr - next;
        _effectSpeed = effectSpeed;
    }

    Vector3[] CalculateArcArray()
    {
        Vector3[] arcArray = new Vector3[resolution + 1];

        radianAngle = Mathf.Deg2Rad * angle;
        float maxDistance = (velocity * velocity * Mathf.Sin(2 * radianAngle)) / g;

        for(int i = 0; i <= resolution; i++)
        {
            float t = (float)i / (float)resolution;
            arcArray[i] = CalculateArcPoint(t, maxDistance);
        }

        return arcArray;
    }

    Vector3 CalculateArcPoint(float t, float maxDistance)
    {
        float x = t * maxDistance;
        float y = x * Mathf.Tan (radianAngle) - ((g * x * x) / (2 * velocity * velocity * Mathf.Cos(radianAngle) * Mathf.Cos(radianAngle)));
        return new Vector3(x, y);
    }
}
