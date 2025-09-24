using UnityEngine;
using UnityEngine.InputSystem;

public class rotate_with_buttons : MonoBehaviour
{
    public float rotationSpeed = 100f;

    private float currentX = 0f;
    private float currentZ = 0f;
    private float maxRotateX = 45f;
    private float maxRotateZ = 45f;

    void Update()
    {
        if (Keyboard.current == null) return;

        float inputX = 0f;
        float inputZ = 0f;

        if (Keyboard.current.wKey.isPressed)
            inputX = 1f;
        if (Keyboard.current.sKey.isPressed)
            inputX = -1f;
        if (Keyboard.current.aKey.isPressed)
            inputZ = -1f;
        if (Keyboard.current.dKey.isPressed)
            inputZ = 1f;

        // обновляем углы с учётом скорости
        currentX += inputX * rotationSpeed * Time.deltaTime;
        currentZ += inputZ * rotationSpeed * Time.deltaTime;

        // ограничиваем углы
        currentX = Mathf.Clamp(currentX, -maxRotateX, maxRotateX);
        currentZ = Mathf.Clamp(currentZ, -maxRotateZ, maxRotateZ);

        // применяем поворот (локальный, чтобы не "плыл" относительно мира)
        transform.localRotation = Quaternion.Euler(currentX, 0f, currentZ);
    }
}
