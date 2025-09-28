using UnityEngine;
using UnityEngine.InputSystem;

public class camera_rotation : MonoBehaviour
{
    public Transform target;          
    public float distance = 5.0f;     
    public float rotationStep = 90f;  
    public float rotationSmooth = 5f;
    public float zoomSpeed = 2.0f;
    public float minDistance = 2f;  
    public float maxDistance = 15f;
    public float followSmooth = 5f;

    private float currentX; 
    private float targetX; 
    private float y = 20.0f; 
    private Vector3 smoothedTargetPos; // сглаженное положение цели

    void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("camera_rotation: не назначен target!");
            return;
        }

        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        targetX = currentX;
        y = angles.x;

        smoothedTargetPos = target.position;
    }

    void LateUpdate()
    {
        if (target == null || Keyboard.current == null || Mouse.current == null) return;

        if (Keyboard.current.qKey.wasPressedThisFrame)
            targetX -= rotationStep;

        if (Keyboard.current.eKey.wasPressedThisFrame)
            targetX += rotationStep;

        currentX = Mathf.LerpAngle(currentX, targetX, Time.deltaTime * rotationSmooth);

        smoothedTargetPos = Vector3.Lerp(smoothedTargetPos, target.position, Time.deltaTime * followSmooth);

        float scroll = Mouse.current.scroll.ReadValue().y;
        distance -= scroll * zoomSpeed * Time.deltaTime;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        Quaternion rotation = Quaternion.Euler(y, currentX, 0);
        Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + smoothedTargetPos;

        transform.rotation = rotation;
        transform.position = position;
    }
}
