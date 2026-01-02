using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ParticleLight2D : MonoBehaviour
{
    public GameObject light2DPrefab;
    public float lightRatio = 0.3f; // Percentage of particles that get lights
    
    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;
    private GameObject[] lights;
    
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        int maxLights = Mathf.CeilToInt(ps.main.maxParticles * lightRatio);
        particles = new ParticleSystem.Particle[ps.main.maxParticles];
        lights = new GameObject[maxLights];
        
        // Pre-instantiate lights
        for (int i = 0; i < maxLights; i++)
        {
            lights[i] = Instantiate(light2DPrefab, transform);
            lights[i].SetActive(false);
        }
    }
    
    void LateUpdate()
    {
        int numParticles = ps.GetParticles(particles);
        int lightsToUse = Mathf.Min(Mathf.CeilToInt(numParticles * lightRatio), lights.Length);
        
        for (int i = 0; i < lights.Length; i++)
        {
            if (i < lightsToUse && i < numParticles)
            {
                lights[i].SetActive(true);
                lights[i].transform.position = new Vector3(particles[i].position.x,  particles[i].position.y, 0);
            }
            else
            {
                lights[i].SetActive(false);
            }
        }
    }
}