using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishTrigger : MonoBehaviour
{
    [Header("Сцена выбора уровня")]
    public string levelSelectSceneName = "LevelSelector"; // Имя сцены выбора уровня

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Победа!");

            // Перезагружаем текущую сцену
            SceneManager.LoadScene(levelSelectSceneName);

            // Если хочешь сразу перейти на сцену выбора уровня вместо перезагрузки, используй:
            // SceneManager.LoadScene(levelSelectSceneName);
        }
    }
}
