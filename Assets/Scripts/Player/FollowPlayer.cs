using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform pos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = pos.position;
    }
}
