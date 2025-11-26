using Unity.Cinemachine;
using UnityEngine;

public class CameraEffector : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Rigidbody2D rb;
    
    [Header("Falling Positional")]
    [SerializeField] private float maxDamp;
    [SerializeField] private float minDamp;
    [SerializeField] private float fallingOffset;
    [Header("Falling Interpolation")]
    [SerializeField] private float dampingLerpTime;
    [SerializeField] private float offsetLerpTime;
    
    [Header("Turning Settings")]
    [SerializeField] private float turnLerpTime;
    
    [Header("Interpolation Curve")]
    [SerializeField] private LeanTweenType easeType = LeanTweenType.easeInOutQuad;
    
    private CinemachinePositionComposer cam;
    private int dampTweenId = -1;
    private int offsetTweenId = -1;
    private int horizontalTweenId = -1;

    private float xDirection;
    private bool startTweeningHorizontal;
    
    void Start()
    {
        cam = GetComponent<CinemachinePositionComposer>();
        PlayerMovement.ChangedLookDir += LookDirChanged;
    }

    void Update()
    {
        //If going up
        if (rb.linearVelocityY > 0)
        {
            StopFallingTweens();
            cam.Damping.y = maxDamp; //Player jumps higher than cam
        }
        //If going down
        else if (rb.linearVelocityY < 0)
        {
            TweenDamping(minDamp);
            TweenOffset(-fallingOffset);
        }
        //If horizontal not moving
        else
        {
            StopFallingTweens();
            
            //Default values
            cam.Damping.y = 1f;
            cam.TargetOffset.y = 0f;
        }

        //Turning code
        if (startTweeningHorizontal)
        {
            TweenHorizontalOffset(xDirection);
        }
    }

    //Called when player turns
    private void LookDirChanged(Vector2 moveDirection)
    {
        LeanTween.cancel(horizontalTweenId);
        xDirection = moveDirection.x;
        startTweeningHorizontal = true;
    }

    private void TweenDamping(float targetValue)
    {
        //If not alr tweening
        if (dampTweenId != -1 && LeanTween.descr(dampTweenId) != null)
        {
            return;
        }

        dampTweenId = LeanTween.value(gameObject, cam.Damping.y, targetValue, dampingLerpTime)
            .setEase(easeType)
            .setOnUpdate((float val) => {
                var damping = cam.Damping;
                damping.y = val;
                cam.Damping = damping;
            })
            .setOnComplete(() => dampTweenId = -1)
            .id;
    }

    private void TweenOffset(float targetValue)
    {
        //If not alr tweening
        if (offsetTweenId != -1 && LeanTween.descr(offsetTweenId) != null)
        {
            return;
        }

        offsetTweenId = LeanTween.value(gameObject, cam.TargetOffset.y, targetValue, offsetLerpTime)
            .setEase(easeType)
            .setOnUpdate((float val) => {
                var offset = cam.TargetOffset;
                offset.y = val;
                cam.TargetOffset = offset;
            })
            .setOnComplete(() => offsetTweenId = -1)
            .id;
    }

    private void TweenHorizontalOffset(float direction)
    {
        //If not alr tweening
        if (horizontalTweenId != -1 && LeanTween.descr(horizontalTweenId) != null)
        {
            return;
        }

        horizontalTweenId = LeanTween.value(gameObject, cam.TargetOffset.x, direction, Mathf.Abs(cam.TargetOffset.x - direction) /  (1/turnLerpTime))
            .setEase(easeType)
            .setOnUpdate((float val) => {
                var offset = cam.TargetOffset;
                offset.x = val;
                cam.TargetOffset = offset;
            })
            .setOnComplete(() =>
            {
                horizontalTweenId = -1;
                startTweeningHorizontal = false;
            })
            .id;
    }

    private void StopFallingTweens()
    {
        if (dampTweenId != -1)
        {
            LeanTween.cancel(dampTweenId);
            dampTweenId = -1;
        }
        
        if (offsetTweenId != -1)
        {
            LeanTween.cancel(offsetTweenId);
            offsetTweenId = -1;
        }
    }
}