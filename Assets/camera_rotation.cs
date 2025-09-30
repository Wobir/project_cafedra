using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CameraRotation : MonoBehaviour
{
    [Header("Targets")]
    public List<Transform> targets = new List<Transform>();
    private int currentTargetIndex = 0;
    private Transform target;

    [Header("Camera Settings")]
    public float distance = 5f;
    public float rotationStep = 90f;
    public float rotationSpeed = 5f; // плавность поворота по Y
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
        if (targets.Count == 0)
        {
            Debug.LogWarning("CameraRotation: список целей пуст!");
            enabled = false;
            return;
        }

        SetTarget(0); // по умолчанию первая цель

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

        // 🔹 Переключение целей по клавишам 1–9
        if (keyboard.digit1Key.wasPressedThisFrame) SetTarget(0);
        if (keyboard.digit2Key.wasPressedThisFrame) SetTarget(1);
        if (keyboard.digit3Key.wasPressedThisFrame) SetTarget(2);
        if (keyboard.digit4Key.wasPressedThisFrame) SetTarget(3);
        if (keyboard.digit5Key.wasPressedThisFrame) SetTarget(4);
        if (keyboard.digit6Key.wasPressedThisFrame) SetTarget(5);
        if (keyboard.digit7Key.wasPressedThisFrame) SetTarget(6);
        if (keyboard.digit8Key.wasPressedThisFrame) SetTarget(7);
        if (keyboard.digit9Key.wasPressedThisFrame) SetTarget(8);

        smoothedTargetPos = Vector3.Lerp(smoothedTargetPos, target.position, Time.deltaTime * followSmooth);

        // 🔹 Поворот по Q/E (шаговый)
        if (keyboard.qKey.wasPressedThisFrame)
            targetYAngle -= rotationStep;
        if (keyboard.eKey.wasPressedThisFrame)
            targetYAngle += rotationStep;

        // 🔹 Поворот мышью при зажатой ЛКМ
        if (mouse.leftButton.isPressed)
        {
            rotatingWithMouse = true;
            Vector2 mouseDelta = mouse.delta.ReadValue();
            float deltaY = mouseDelta.x * mouseSensitivityX * Time.deltaTime;
            float deltaX = mouseDelta.y * mouseSensitivityY * Time.deltaTime;

            currentYAngle += deltaY;
            targetYAngle += deltaY; // синхронизация с Q/E
            currentXAngle -= deltaX;
            currentXAngle = Mathf.Clamp(currentXAngle, minXAngle, maxXAngle);
        }
        else if (rotatingWithMouse)
        {
            // 🔹 Привязка к ближайшему 90° после отпускания мыши
            targetYAngle = Mathf.Round(targetYAngle / 90f) * 90f;
            rotatingWithMouse = false;
        }

        // 🔹 Плавный переход по оси Y
        currentYAngle = Mathf.LerpAngle(currentYAngle, targetYAngle, Time.deltaTime * rotationSpeed);

        // 🔹 Зум колесом мыши
        float scroll = mouse.scroll.ReadValue().y;
        distance = Mathf.Clamp(distance - scroll * zoomSpeed * Time.deltaTime, minDistance, maxDistance);

        // 🔹 Применяем позицию и поворот
        Quaternion rotation = Quaternion.Euler(currentXAngle, currentYAngle, 0);
        Vector3 position = smoothedTargetPos - rotation * Vector3.forward * distance;

        transform.position = position;
        transform.rotation = rotation;

        UpdateCameraSide();
    }

    private void SetTarget(int index)
    {
        if (index >= 0 && index < targets.Count)
        {
            currentTargetIndex = index;
            target = targets[index];
            smoothedTargetPos = target.position;
        }
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
