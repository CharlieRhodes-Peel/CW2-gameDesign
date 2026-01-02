using UnityEngine;

public class NpcCollider : MonoBehaviour
{
    private BoxCollider2D boxCollider;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = true;
    }
}
