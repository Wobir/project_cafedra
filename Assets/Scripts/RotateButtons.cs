#nullable enable
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public sealed class RotateWithButtons : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField, Min(0f)] private float rotationSpeed = 100f;
    [SerializeField, Min(0f)] private float damping = 5f;
    [SerializeField, Range(0f, 90f)] private float maxRotateX = 45f;
    [SerializeField, Range(0f, 90f)] private float maxRotateZ = 45f;

    [Header("Physics Settings")]
    [SerializeField, Min(0f)] private float extraGravity = 15f;
    [SerializeField] private Rigidbody? ballRigidbody;
    [SerializeField] private CameraRotation? cameraRotationScript;

    private Rigidbody _rigidbody = null!;
    private Vector2 _velocity;
    private Vector2 _rotation;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.isKinematic = true;

        if (ballRigidbody == null)
            ballRigidbody = FindAnyObjectByType<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (cameraRotationScript == null)
            return;

        Vector2 input = GetInput(cameraRotationScript.CurrentSide);

        _velocity = Vector2.Lerp(
            _velocity + rotationSpeed * Time.fixedDeltaTime * input,
            Vector2.zero,
            damping * Time.fixedDeltaTime
        );

        _rotation.x = Mathf.Clamp(_rotation.x + _velocity.x * Time.fixedDeltaTime, -maxRotateX, maxRotateX);
        _rotation.y = Mathf.Clamp(_rotation.y + _velocity.y * Time.fixedDeltaTime, -maxRotateZ, maxRotateZ);

        Quaternion targetRotation = Quaternion.Euler(_rotation.x, 0f, _rotation.y);
        _rigidbody.MoveRotation(targetRotation);

        if (ballRigidbody != null)
            ballRigidbody.AddForce(-transform.up * extraGravity, ForceMode.Acceleration);
    }

    private static Vector2 GetInput(CameraRotation.CameraSide side)
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return Vector2.zero;

        bool w = keyboard.wKey.isPressed;
        bool a = keyboard.aKey.isPressed;
        bool s = keyboard.sKey.isPressed;
        bool d = keyboard.dKey.isPressed;

        return side switch
        {
            CameraRotation.CameraSide.Front => new Vector2((w ? 1f : 0f) - (s ? 1f : 0f), (a ? 1f : 0f) - (d ? 1f : 0f)),
            CameraRotation.CameraSide.Right => new Vector2((a ? 1f : 0f) - (d ? 1f : 0f), (s ? 1f : 0f) - (w ? 1f : 0f)),
            CameraRotation.CameraSide.Back  => new Vector2((s ? 1f : 0f) - (w ? 1f : 0f), (d ? 1f : 0f) - (a ? 1f : 0f)),
            CameraRotation.CameraSide.Left  => new Vector2((d ? 1f : 0f) - (a ? 1f : 0f), (w ? 1f : 0f) - (s ? 1f : 0f)),
            _ => Vector2.zero
        };
    }

    public void SetCameraRotation(CameraRotation cam) => cameraRotationScript = cam;
}
