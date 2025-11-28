using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraEffector : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private InputActionReference movingInput;
    
    [Header("Falling Positional")]
    [SerializeField] private float maxDamp;
    [SerializeField] private float minDamp;
    [SerializeField] private float fallingOffset;
    [Header("Falling Interpolation")]
    [SerializeField] private float dampingLerpTime;
    [SerializeField] private float offsetLerpTime;
    
    [Header("Turning Settings")]
    [SerializeField] private float turnLerpTime;
    
    [Header("Look Settings")]
    [SerializeField] private float lookOffset;
    [SerializeField] private float lookLerpTime;
    
    [Header("Interpolation Curves")]
    [SerializeField] private LeanTweenType movementLeanType = LeanTweenType.easeInOutQuad;
    [SerializeField] private LeanTweenType lookingLeanType = LeanTweenType.easeInOutElastic;
    
    private CinemachinePositionComposer cam;
    private int dampTweenId = -1;
    private int offsetTweenId = -1;
    private int horizontalTweenId = -1;

    private float xDirection;
    private bool startTweeningHorizontal;

    private Vector2 lookDir;
    
    void Start()
    {
        cam = GetComponent<CinemachinePositionComposer>();
        PlayerMovement.ChangedLookDir += LookDirChanged;
    }

    void Update()
    {
        lookDir = movingInput.action.ReadValue<Vector2>();   
        
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
        //If looking up and not moving
        else if (lookDir.y > 0 && rb.linearVelocity == new Vector2(0, 0))
        {
            TweenLook(lookOffset);
        }
        //If looking down and not moving
        else if (lookDir.y < 0 &&  rb.linearVelocity == new Vector2(0, 0))
        {
            TweenLook(-lookOffset);
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
            .setEase(movementLeanType)
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
            .setEase(movementLeanType)
            .setOnUpdate((float val) => {
                var offset = cam.TargetOffset;
                offset.y = val;
                cam.TargetOffset = offset;
            })
            .setOnComplete(() => offsetTweenId = -1)
            .id;
    }
    
    private void TweenLook(float targetValue)
    {
        //If not alr tweening
        if (offsetTweenId != -1 && LeanTween.descr(offsetTweenId) != null)
        {
            return;
        }

        offsetTweenId = LeanTween.value(gameObject, cam.TargetOffset.y, targetValue, lookLerpTime)
            .setEase(lookingLeanType)
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
            .setEase(movementLeanType)
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