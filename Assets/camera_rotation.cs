using UnityEngine;
using UnityEngine.InputSystem;

public class camera_rotation : MonoBehaviour
{
    public Transform target;          // объект, вокруг которого вращаемся
    public float distance = 5.0f;     // расстояние до объекта
    public float rotationStep = 90f;  // шаг поворота по нажатию (градусы)
    public float rotationSmooth = 5f; // скорость плавного поворота
    public float zoomSpeed = 2.0f;    // скорость приближения/отдаления
    public float minDistance = 2f;    // минимальная дистанция
    public float maxDistance = 15f;   // максимальная дистанция
    public float followSmooth = 5f;   // скорость сглаживания слежения

    private float currentX;   // текущий угол по горизонтали
    private float targetX;    // целевой угол по горизонтали
    private float y = 20.0f;  // угол по вертикали
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

        // нажали Q — шаг влево
        if (Keyboard.current.qKey.wasPressedThisFrame)
            targetX -= rotationStep;

        // нажали E — шаг вправо
        if (Keyboard.current.eKey.wasPressedThisFrame)
            targetX += rotationStep;

        // плавно двигаем currentX к targetX
        currentX = Mathf.LerpAngle(currentX, targetX, Time.deltaTime * rotationSmooth);

        // сглаживаем позицию цели
        smoothedTargetPos = Vector3.Lerp(smoothedTargetPos, target.position, Time.deltaTime * followSmooth);

        // зум колесиком мыши
        float scroll = Mouse.current.scroll.ReadValue().y;
        distance -= scroll * zoomSpeed * Time.deltaTime;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        // вычисляем позицию и поворот камеры
        Quaternion rotation = Quaternion.Euler(y, currentX, 0);
        Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + smoothedTargetPos;

        transform.rotation = rotation;
        transform.position = position;
    }
}
