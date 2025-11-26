using System;
using System.Collections;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;

    [Header("Flip Rotation Stats")] 
    [SerializeField] private float lerpTime = 0.5f;

    private bool flipping = false;
    private float elapsedTime = 0f;
}
