using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraEffector : MonoBehaviour
{
    [SerializeField] private CinemachinePositionComposer cam;

    [SerializeField] private float maxDamp;
    [SerializeField] private float minDamp;
    [SerializeField] private float lerpTime;
    
    private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.linearVelocityY > 0)
        {
            cam.Damping.y = maxDamp;
        }

        if (rb.linearVelocityY < 0)
        {
            cam.Damping.y = Mathf.Lerp(cam.Damping.y, minDamp, Time.deltaTime * lerpTime);
        }
    }
}
