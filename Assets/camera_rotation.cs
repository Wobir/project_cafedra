using UnityEngine;
using UnityEngine.InputSystem; 
public class camera_rotation : MonoBehaviour
{
        public Transform target;       // объект, вокруг которого вращаемся
    public float distance = 5.0f;  // расстояние до объекта
    public float xSpeed = 120.0f;  // скорость вращения по X (влево-вправо)
    public float ySpeed = 80.0f;   // скорость вращения по Y (вверх-вниз)

    public float yMinLimit = -20f; // ограничение угла по Y
    public float yMaxLimit = 80f;

    private float x = 0.0f;
    private float y = 0.0f;

    void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("OrbitCamera: не назначен target!");
            return;
        }

        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        // отключаем вращение Rigidbody, если есть
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
    }

    void LateUpdate()
    {
        if (target == null || Mouse.current == null) return;

        // читаем движение мыши (дельта с последнего кадра)
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        x += mouseDelta.x * xSpeed * Time.deltaTime;
        y -= mouseDelta.y * ySpeed * Time.deltaTime;

        // ограничиваем угол по вертикали
        y = Mathf.Clamp(y, yMinLimit, yMaxLimit);

        // вычисляем вращение и позицию камеры
        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;

        transform.rotation = rotation;
        transform.position = position;
    }
}
