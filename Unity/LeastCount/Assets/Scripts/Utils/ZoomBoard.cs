using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ZoomBoard : MonoBehaviour {

    // The minimum orthographic size value we want to zoom to
    public float MinSizeX = 300.0f;
    // The maximum orthographic size value we want to zoom to
    public float MaxSizeX = 800.0f;
    protected float MinSizeY = 480.0f;
    protected float MaxSizeY = 1280.0f;
    // This stores the layers we want the raycast to hit (make sure this GameObject's layer is included!)
    public LayerMask LayerMask = UnityEngine.Physics.DefaultRaycastLayers;
    // Image Reference
    public Image BoardImage = null;
    // This stores the finger that's currently dragging this GameObject
    private Lean.LeanFinger draggingFinger;
    // Canvas Scaler Reference
    protected CanvasScaler CanvasRef = null;

    void Start() 
    {
        CanvasRef = this.GetComponent<CanvasScaler>();
    }    

    protected virtual void OnEnable()
    {
        // Hook into the OnFingerDown event
        Lean.LeanTouch.OnFingerDown += OnFingerDown;

        // Hook into the OnFingerUp event
        Lean.LeanTouch.OnFingerUp += OnFingerUp;
    }

    protected virtual void OnDisable()
    {
        // Unhook the OnFingerDown event
        Lean.LeanTouch.OnFingerDown -= OnFingerDown;

        // Unhook the OnFingerUp event
        Lean.LeanTouch.OnFingerUp -= OnFingerUp;
    }

    protected virtual void LateUpdate()
    {
        // Does the main camera exist?
        if (CanvasRef != null && BoardImage != null)
        {
            float pinch = Lean.LeanTouch.PinchScale;
            // Make sure the pinch scale is valid
            if (pinch > 0.0f && pinch < 2.0f)
            {
                // Scale the FOV based on the pinch scale
                Vector2 res = CanvasRef.referenceResolution;

                res.x /= pinch;
                res.y /= pinch;
                // Make sure the new FOV is within our min/max
                res.x = Mathf.Clamp(res.x, MinSizeX, MaxSizeX);
                res.y = Mathf.Clamp(res.y, MinSizeY, MaxSizeY);

                CanvasRef.referenceResolution = res;
                //Debug.Log("PinchScale: "+pinch+", ResX: "+res.x);
            }

            // If there is an active finger, move this GameObject based on it
            if (draggingFinger != null && pinch == 1.0f)
            {
                float MaxOffX, MinOffX;
                MaxOffX = (MaxSizeX - CanvasRef.referenceResolution.x)/2.0f;
                MinOffX = MaxOffX * -1.0f;
                float MaxOffY, MinOffY;
                MaxOffY = (MaxSizeY - CanvasRef.referenceResolution.y)/2.0f;
                MinOffY = MaxOffY * -1.0f;
                Lean.LeanTouch.MoveObject(BoardImage.gameObject.transform, draggingFinger.DeltaScreenPosition);

                // clamp it
                RectTransform trans = BoardImage.gameObject.transform as RectTransform;
                Vector2 pos = trans.anchoredPosition;
                pos.x = Mathf.Clamp(pos.x, MinOffX, MaxOffX);
                pos.y = Mathf.Clamp(pos.y, MinOffY, MaxOffY);

                trans.anchoredPosition = pos;
            }

        }
    }

    public void OnFingerDown(Lean.LeanFinger finger)
    {
        if(draggingFinger == null)
        {
            // Set the current finger to this one
            draggingFinger = finger;
        }
    }

    public void OnFingerUp(Lean.LeanFinger finger)
    {
        // Was the current finger lifted from the screen?
        if (finger == draggingFinger)
        {
            // Unset the current finger
            draggingFinger = null;
        }
    }
}
