using UnityEngine;
using UnityEngine.InputSystem;

public class camera_rotation : MonoBehaviour
{
    public Transform target;       // объект, вокруг которого вращаемся
    public float distance = 5.0f;  // расстояние до объекта
    public float rotationSpeed = 90.0f; // скорость вращения по Y (градусы/сек)
    public float zoomSpeed = 2.0f; // скорость приближения/отдаления
    public float minDistance = 2f; // минимальная дистанция
    public float maxDistance = 15f; // максимальная дистанция

    private float x = 0.0f; // угол по горизонтали
    private float y = 20.0f; // угол по вертикали (можно менять на W/S)

    void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("camera_rotation: не назначен target!");
            return;
        }

        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    void LateUpdate()
    {
        if (target == null || Keyboard.current == null || Mouse.current == null) return;

        // вращение по Q/E
        float input = 0f;
        if (Keyboard.current.qKey.isPressed)
            input = -1f;
        if (Keyboard.current.eKey.isPressed)
            input = 1f;

        x += input * rotationSpeed * Time.deltaTime;

        // зум колесиком мыши
        float scroll = Mouse.current.scroll.ReadValue().y;
        distance -= scroll * zoomSpeed * Time.deltaTime;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        // считаем позицию и поворот камеры
        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;

        transform.rotation = rotation;
        transform.position = position;
    }
}
