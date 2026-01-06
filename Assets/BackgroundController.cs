using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private float startPos;
    private GameObject cam;
    public float parallaxEffect;

    void Start()
    {
        startPos = transform.position.x;
        cam = GameObject.Find("Main Camera");
    }

    void FixedUpdate()
    {
        transform.position = new Vector3(cam.transform.position.x + parallaxEffect, cam.transform.position.y, transform.position.z);
    }
}
