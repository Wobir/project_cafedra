using UnityEngine;
using UnityEngine.InputSystem;

public class rotate_with_buttons : MonoBehaviour
{
    public float rotationSpeed = 100f;      
    public float damping = 5f;           
    
    private float velocityX = 0f;
    private float velocityZ = 0f;

    private float currentX = 0f;
    private float currentZ = 0f;

    public float maxRotateX = 45f;
    public float maxRotateZ = 45f;

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

        velocityX += inputX * rotationSpeed * Time.deltaTime;
        velocityZ += inputZ * rotationSpeed * Time.deltaTime;

        velocityX = Mathf.Lerp(velocityX, 0f, damping * Time.deltaTime);
        velocityZ = Mathf.Lerp(velocityZ, 0f, damping * Time.deltaTime);

        currentX += velocityX * Time.deltaTime;
        currentZ += velocityZ * Time.deltaTime;

        currentX = Mathf.Clamp(currentX, -maxRotateX, maxRotateX);
        currentZ = Mathf.Clamp(currentZ, -maxRotateZ, maxRotateZ);

        transform.localRotation = Quaternion.Euler(currentX, 0f, currentZ);
    }
}