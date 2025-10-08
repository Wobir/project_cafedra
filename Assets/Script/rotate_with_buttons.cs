using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement; // Добавлено для загрузки сцены

public class RotateWithButtons : MonoBehaviour
{
    public float rotationSpeed = 100f;
    public float damping = 5f;

    private float velocityX = 0f;
    private float velocityZ = 0f;
    private float currentX = 0f;
    private float currentZ = 0f;

    public float maxRotateX = 45f;
    public float maxRotateZ = 45f;

    public CameraRotation cameraRotationScript;

    // Имя сцены, на которую нужно вернуться при нажатии Escape
    public string sceneToReturn = "MainMenu";

    void Update()
    {
        if (Keyboard.current == null || cameraRotationScript == null) 
            return;

        // Проверка нажатия Escape для возврата на указанную сцену
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            SceneManager.LoadScene(sceneToReturn);
            return;
        }

        float inputX = 0f;
        float inputZ = 0f;

        bool w = Keyboard.current.wKey.isPressed;
        bool a = Keyboard.current.aKey.isPressed;
        bool s = Keyboard.current.sKey.isPressed;
        bool d = Keyboard.current.dKey.isPressed;

        // Определяем направление вращения в зависимости от стороны камеры
        switch (cameraRotationScript.currentSide)
        {
            case CameraRotation.CameraSide.Front:
                inputX = (w ? 1f : 0f) + (s ? -1f : 0f);
                inputZ = (a ? 1f : 0f) + (d ? -1f : 0f);
                break;

            case CameraRotation.CameraSide.Right:
                inputX = (a ? 1f : 0f) + (d ? -1f : 0f);
                inputZ = (w ? -1f : 0f) + (s ? 1f : 0f);
                break;

            case CameraRotation.CameraSide.Back:
                inputX = (w ? -1f : 0f) + (s ? 1f : 0f);
                inputZ = (a ? -1f : 0f) + (d ? 1f : 0f);
                break;

            case CameraRotation.CameraSide.Left:
                inputX = (a ? -1f : 0f) + (d ? 1f : 0f);
                inputZ = (w ? 1f : 0f) + (s ? -1f : 0f);
                break;
        }

        // Обновление скорости с учётом ускорения и демпфирования
        velocityX = Mathf.Lerp(velocityX + inputX * rotationSpeed * Time.deltaTime, 0f, damping * Time.deltaTime);
        velocityZ = Mathf.Lerp(velocityZ + inputZ * rotationSpeed * Time.deltaTime, 0f, damping * Time.deltaTime);

        currentX = Mathf.Clamp(currentX + velocityX * Time.deltaTime, -maxRotateX, maxRotateX);
        currentZ = Mathf.Clamp(currentZ + velocityZ * Time.deltaTime, -maxRotateZ, maxRotateZ);

        transform.localRotation = Quaternion.Euler(currentX, 0f, currentZ);
    }
}
