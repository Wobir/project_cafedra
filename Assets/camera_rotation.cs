using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRotation : MonoBehaviour
{
    public Transform target;
    public float distance = 5f;
    public float rotationStep = 90f;
    public float rotationSpeed = 5f; 
    public float zoomSpeed = 2f;
    public float minDistance = 2f;
    public float maxDistance = 15f;
    public float followSmooth = 5f;
    public float minXAngle = -80f;
    public float maxXAngle = 80f;

    [Header("Mouse Sensitivity")]
    public float mouseSensitivityX = 200f;
    public float mouseSensitivityY = 150f;

    private float currentYAngle;
    private float targetYAngle;
    private float currentXAngle;
    private Vector3 smoothedTargetPos;

    private bool rotatingWithMouse = false;

    public enum CameraSide { Front, Right, Back, Left }
    public CameraSide currentSide = CameraSide.Front;

    void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraRotation: target не назначен!");
            enabled = false;
            return;
        }

        Vector3 angles = transform.eulerAngles;
        currentYAngle = targetYAngle = angles.y;
        currentXAngle = angles.x;
        smoothedTargetPos = target.position;

        UpdateCameraSide();
    }

    void LateUpdate()
    {
        if (target == null) return;

        var keyboard = Keyboard.current;
        var mouse = Mouse.current;
        if (keyboard == null || mouse == null) return;

        smoothedTargetPos = Vector3.Lerp(smoothedTargetPos, target.position, Time.deltaTime * followSmooth);

        // Поворот по Q/E
        if (keyboard.qKey.wasPressedThisFrame)
            targetYAngle -= rotationStep;
        if (keyboard.eKey.wasPressedThisFrame)
            targetYAngle += rotationStep;

        // Поворот мышью
        if (mouse.leftButton.isPressed)
        {
            rotatingWithMouse = true;
            Vector2 mouseDelta = mouse.delta.ReadValue();
            float deltaY = mouseDelta.x * mouseSensitivityX * Time.deltaTime;
            float deltaX = mouseDelta.y * mouseSensitivityY * Time.deltaTime;

            currentYAngle += deltaY;
            targetYAngle += deltaY;
            currentXAngle -= deltaX;
            currentXAngle = Mathf.Clamp(currentXAngle, minXAngle, maxXAngle);
        }
        else if (rotatingWithMouse)
        {
            // Когда мышь отпущена, привязываем Y к ближайшему 90°
            targetYAngle = Mathf.Round(targetYAngle / 90f) * 90f;
            rotatingWithMouse = false;
        }

        // Плавный Lerp по Y
        currentYAngle = Mathf.LerpAngle(currentYAngle, targetYAngle, Time.deltaTime * rotationSpeed);

        // Зум колесом мыши
        float scroll = mouse.scroll.ReadValue().y;
        distance = Mathf.Clamp(distance - scroll * zoomSpeed * Time.deltaTime, minDistance, maxDistance);

        // Обновление позиции и вращения камеры
        Quaternion rotation = Quaternion.Euler(currentXAngle, currentYAngle, 0);
        Vector3 position = smoothedTargetPos - rotation * Vector3.forward * distance;

        transform.position = position;
        transform.rotation = rotation;

        UpdateCameraSide();
    }

    private void UpdateCameraSide()
    {
        float angle = currentYAngle % 360f;
        if (angle < 0) angle += 360f;

        if (angle < 45f || angle >= 315f)
            currentSide = CameraSide.Front;
        else if (angle >= 45f && angle < 135f)
            currentSide = CameraSide.Right;
        else if (angle >= 135f && angle < 225f)
            currentSide = CameraSide.Back;
        else
            currentSide = CameraSide.Left;
    }
}
