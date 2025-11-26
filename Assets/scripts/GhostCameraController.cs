using System.Collections;
using UnityEngine;

public class GhostCameraController : MonoBehaviour
{
    public static GhostCameraController Instance;

    [Header("Camera")]
    public Transform cameraTransform;

    [Header("Perfect Scare Coordinates")]
    // Enter your values from the image here in the Inspector
    public Vector3 targetLocalPosition = new Vector3(0.45f, 0.18f, 0.7f);
    public Vector3 targetLocalRotation = new Vector3(16f, 160f, 0f);

    void Awake()
    {
        Instance = this;
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    /// <summary>
    /// Moves camera to the ghost and locks it to the specific offset defined above.
    /// </summary>
    /// <param name="parentObject">The Ghost or Model Transform to attach to</param>
    public void MoveCameraToPoint(Transform parentObject, float duration = 1f)
    {
        CameraController camCtrl = cameraTransform.GetComponentInParent<CameraController>();
        if (camCtrl != null)
            camCtrl.freezeCamera = true; 

        StartCoroutine(MoveToScarePointRoutine(parentObject, duration));
    }

    private IEnumerator MoveToScarePointRoutine(Transform parentObject, float duration)
    {
        Vector3 startPos = cameraTransform.position;
        Quaternion startRot = cameraTransform.rotation;

        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            t = Mathf.SmoothStep(0f, 1f, t); 

            // Calculate where the camera SHOULD be in world space right now
            // based on the Ghost's current position + your specific offset
            Vector3 targetWorldPos = parentObject.TransformPoint(targetLocalPosition);
            Quaternion targetWorldRot = parentObject.rotation * Quaternion.Euler(targetLocalRotation);

            // Lerp towards that calculated point
            cameraTransform.position = Vector3.Lerp(startPos, targetWorldPos, t);
            cameraTransform.rotation = Quaternion.Slerp(startRot, targetWorldRot, t);

            yield return null;
        }

        // --- FINAL LOCK ---

        // 1. Parent the camera to the Ghost (so it follows animation/movement)
        cameraTransform.SetParent(parentObject);

        // 2. Set the LOCAL position to exactly what you asked for
        // The camera is now attached to the ghost, but shifted by these numbers
        cameraTransform.localPosition = targetLocalPosition;
        cameraTransform.localRotation = Quaternion.Euler(targetLocalRotation);
    }
}