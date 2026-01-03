using UnityEngine;
using UnityEngine.Rendering.Universal;

public class InteractableLight : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private float minInnerRadius = 0.2f;
    [SerializeField] private float maxInnerRadius = 1.5f;
    [SerializeField] private float oscillationDuration = 1f;
    [SerializeField] private LeanTweenType easeType = LeanTweenType.easeInOutSine;
    
    private Light2D spotLight;
    private LTDescr currentTween;

    void Start()
    {
        spotLight = GetComponent<Light2D>();
        
        if (spotLight == null)
        {
            Debug.LogError("No Light2D component found on " + gameObject.name);
            enabled = false;
            return;
        }
        
        if (spotLight.lightType != Light2D.LightType.Point)
        {
            Debug.LogWarning("Light type should be Point or Spot for inner radius control");
        }
        
        // Start the oscillation
        StartOscillation();
    }

    void StartOscillation()
    {
        // Tween to max radius
        currentTween = LeanTween.value(gameObject, minInnerRadius, maxInnerRadius, oscillationDuration)
            .setEase(easeType)
            .setOnUpdate((float val) => {
                //spotLight.pointLightInnerRadius = val;
                spotLight.pointLightOuterRadius = val;
            })
            .setOnComplete(() => {
                // Tween back to min radius
                currentTween = LeanTween.value(gameObject, maxInnerRadius, minInnerRadius, oscillationDuration)
                    .setEase(easeType)
                    .setOnUpdate((float val) => {
                        //spotLight.pointLightInnerRadius = val;
                        spotLight.pointLightOuterRadius = val;
                    })
                    .setOnComplete(StartOscillation); // Loop back
            });
    }

    void OnDestroy()
    {
        // Clean up tween when object is destroyed
        if (currentTween != null)
        {
            LeanTween.cancel(gameObject);
        }
    }
}
